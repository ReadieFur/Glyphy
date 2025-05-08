using Glyphy.Configuration;
using Java.Interop;
using Java.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using JIOException = Java.IO.IOException;
using JProcess = Java.Lang.Process;
using SIOException = System.IO.IOException;
using Com.Nothing.Ketchum;
using Android.Content;
using System.Threading;
using Java.Lang;

namespace Glyphy.LED
{
    //Some "odd" choices have been made here but they were in the intrest of making the API as fast as possible as it will be called very frequently.
    public class API : Java.Lang.Object, IDisposable, GlyphManager.ICallback
    {
        //These two values are used for the API brightness values but will be converted to system values by the platform specific partial classes.
        public const float MIN_BRIGHTNESS = 0.0f;
        public const float MAX_BRIGHTNESS = 1.0f;

        private static readonly object _LOCK = new object();

        private static API? _instance = null;
        //I am using this to make the API only initalize when called (as opposed to a static constructor).
        public static API Instance
        {
            get
            {
                //It is quicker to check if the instance is null before locking, at which point it will be ok to peform the check again.
                if (_instance is null)
                {
                    lock (_LOCK)
                    {
                        if (_instance is null)
                        {
                            _instance = new();
                        }
                    }
                }
                return _instance;
            }
        }

        public static bool IsReady => Instance._onAPIConnected.Task.IsCompleted;

        private const string BASE_PATH = "/sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led";

        public EPhoneType PhoneType { get; private set; } = EPhoneType.Unknown;

        private GlyphManager _glyphManager;
        private TaskCompletionSource _onAPIConnected = new(false);
        private uint _maxBrightness = 0;
        //Yes I could've used a dictionary here but this method is slightly faster (I think).
        private uint[] _cachedBrightnessValues = new uint[System.Enum.GetValues<EAddressable>().Length];

        public API()
        {
            _glyphManager = GlyphManager.GetInstance(Android.App.Application.Context) ?? throw new InvalidOperationException("Failed to connect to Glyph API.");
            _glyphManager.Init(this);
        }

        void IDisposable.Dispose()
        {
            _glyphManager.UnInit();
        }

        public void OnServiceConnected(ComponentName? p0)
        {
            if (Common.Is20111())
                PhoneType = EPhoneType.PhoneOne;
            else if (Common.Is22111())
                PhoneType = EPhoneType.PhoneTwo;
            else if (Common.Is23111())
                PhoneType = EPhoneType.PhoneTwoA;
            else if (Common.Is23113())
                PhoneType = EPhoneType.PhoneTwoAPlus;
            else if (Common.Is24111())
                PhoneType = EPhoneType.PhoneThreeA;
            else
                throw new IndexOutOfRangeException("Unknown device type.");

            if (!_glyphManager.Register($"DEVICE_{(uint)PhoneType}"))
                throw new InvalidOperationException("Failed to register device.");

            _glyphManager.OpenSession();

            _onAPIConnected.SetResult();
        }

        public void OnServiceDisconnected(ComponentName? p0)
        {
            _glyphManager.CloseSession();
            _onAPIConnected.TrySetCanceled();
        }

        public async Task WaitForReady() => await Instance._onAPIConnected.Task;

        private string GetGroupSystemName(EGroup ledGroup)
        {
            return ledGroup switch
            {
                EGroup.CAMERA => "rear_cam_led_br",
                EGroup.DIAGONAL => "front_cam_led_br",
                EGroup.CENTER => "round_leds_br",
                EGroup.LINE => "vline_leds_br",
                EGroup.DOT => "dot_led_br",
                _ => throw new KeyNotFoundException("Invalid device ID.")
            };
        }

        private uint GetAddressableSystemID(EAddressable addressableLED)
        {
            return addressableLED switch
            {
                EAddressable.CAMERA => 7,
                EAddressable.DIAGONAL => 1,
                EAddressable.RECORDING_LED => 17,
                EAddressable.CENTER_BOTTOM_LEFT => 2,
                EAddressable.CENTER_BOTTOM_RIGHT => 3,
                EAddressable.CENTER_TOP_LEFT => 5,
                EAddressable.CENTER_TOP_RIGHT => 4,
                EAddressable.LINE_1 => 13,
                EAddressable.LINE_2 => 11,
                EAddressable.LINE_3 => 9,
                EAddressable.LINE_4 => 12,
                EAddressable.LINE_5 => 10,
                EAddressable.LINE_6 => 14,
                EAddressable.LINE_7 => 15,
                EAddressable.LINE_8 => 8,
                EAddressable.DOT => 16,
                _ => throw new KeyNotFoundException("Invalid device ID.")
            };
        }

        private int GetCachedBrightnessIndex(EAddressable addressableLED)
        {
            return addressableLED switch
            {
                EAddressable.CAMERA => 0,
                EAddressable.DIAGONAL => 1,
                EAddressable.RECORDING_LED => 2,
                EAddressable.CENTER_BOTTOM_LEFT => 3,
                EAddressable.CENTER_BOTTOM_RIGHT => 4,
                EAddressable.CENTER_TOP_LEFT => 5,
                EAddressable.CENTER_TOP_RIGHT => 6,
                EAddressable.LINE_1 => 7,
                EAddressable.LINE_2 => 8,
                EAddressable.LINE_3 => 9,
                EAddressable.LINE_4 => 10,
                EAddressable.LINE_5 => 11,
                EAddressable.LINE_6 => 12,
                EAddressable.LINE_7 => 13,
                EAddressable.LINE_8 => 14,
                EAddressable.DOT => 15,
                _ => throw new KeyNotFoundException("Invalid device ID.")
            };
        }

        public async Task<float> GetBrightness(EGroup ledGroup)
        {
            throw new NotImplementedException();
        }

        public Task<float> GetBrightness(EAddressable adressableLED) =>
            Task.FromResult(ToNormalisedRange(_cachedBrightnessValues[GetCachedBrightnessIndex(adressableLED)]));

        public async Task SetBrightness(EGroup ledGroup, float brightness)
        {
            uint systemBrightness = ToSystemRange(brightness);

            switch (ledGroup)
            {
                case EGroup.CAMERA:
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CAMERA)] = systemBrightness;
                    break;
                case EGroup.DIAGONAL:
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.DIAGONAL)] = systemBrightness;
                    break;
                case EGroup.CENTER:
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CENTER_TOP_LEFT)] = systemBrightness;
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CENTER_TOP_RIGHT)] = systemBrightness;
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CENTER_BOTTOM_LEFT)] = systemBrightness;
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CENTER_BOTTOM_RIGHT)] = systemBrightness;
                    break;
                case EGroup.LINE:
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_1)] = systemBrightness;
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_2)] = systemBrightness;
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_3)] = systemBrightness;
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_4)] = systemBrightness;
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_5)] = systemBrightness;
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_6)] = systemBrightness;
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_7)] = systemBrightness;
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_8)] = systemBrightness;
                    break;
                case EGroup.DOT:
                    _cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.DOT)] = systemBrightness;
                    break;
                default:
                    throw new KeyNotFoundException("Invalid device ID.");
            }

            throw new NotImplementedException();
        }

        public async Task SetBrightness(EAddressable addressableLED, float brightness)
        {
            _cachedBrightnessValues[GetCachedBrightnessIndex(addressableLED)] = ToSystemRange(brightness);

            uint systemBrightness = ToSystemRange(
                MathF.Pow(
                    brightness //Base brightness
                    * (await Storage.Settings.GetCached()).BrightnessMultiplier, //Brightness multiplier
                    2f) //Exponential curve (I've added this as the visual change in brightness gets smaller as the brightness increases. The value I have set here is arbitrary).
                );

            throw new NotImplementedException();
        }

        private float ToNormalisedRange(uint value)
        {
            //return Math.Clamp(Misc.Helpers.ConvertNumberRange(value, 0, maxBrightness, 0, 1), 0f, 1f);
            //Optimized //(assumes the value is within a valid range, which it should be within this class):
            return System.Math.Clamp((float)value / _maxBrightness, 0f, 1f);
        }

        private uint ToSystemRange(float value)
        {
            //(uint)Math.Clamp(Misc.Helpers.ConvertNumberRange(value, 0f, 1f, 0f, maxBrightness), 0, maxBrightness);
            //Optimized:
            return System.Math.Clamp(Convert.ToUInt32(value * _maxBrightness), 0u, _maxBrightness);
        }
    }
}
