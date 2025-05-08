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
using System.Linq;
using Glyphy.LED.Zones;

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
        public string Codename { get; private set; } = string.Empty;

        private GlyphManager _glyphManager;
        private TaskCompletionSource _onAPIConnected = new(false);
        private Type _activeZoneMappingType;

        public API()
        {
            _glyphManager = GlyphManager.GetInstance(Android.App.Application.Context) ?? throw new InvalidOperationException("Failed to connect to Glyph API.");
            _glyphManager.Init(this);
        }

        void IDisposable.Dispose()
        {
            _glyphManager.UnInit();
        }

#if DEBUG
        public void DebugTest()
        {
            /*var builder = _glyphManager.GlyphFrameBuilder!;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var frame = builder
                .BuildChannel(Glyph.Code_20111.B1)
                .BuildPeriod(5000)
                .BuildCycles(20)
                .BuildInterval(1000)
                .Build();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            _glyphManager.Animate(frame);*/

            /*//Create array of 15 with 2048 as values.
            int[] nums = Enumerable.Repeat(500, 15).ToArray();
            _glyphManager.SetFrameColors(nums);*/

            Dictionary<EPhoneOneZones, int> map = new()
            {
                { EPhoneOneZones.A1, 2048 },
                { EPhoneOneZones.E1, 500 },
                { EPhoneOneZones.B1, 4096 }
            };
            var frame = ZoneMapper.ZoneToArray(map);
            _glyphManager.SetFrameColors(frame);
        }
#endif

        public void OnServiceConnected(ComponentName? p0)
        {
            if (Common.Is20111())
            {
                PhoneType = EPhoneType.PhoneOne;
                Codename = Glyph.Device20111!;
            }
            else if (Common.Is22111())
            {
                PhoneType = EPhoneType.PhoneTwo;
                Codename = Glyph.Device22111!;
                //Device22111i Secret/unreleased model?
            }
            else if (Common.Is23111())
            {
                PhoneType = EPhoneType.PhoneTwoA;
                Codename = Glyph.Device23111!;
            }
            else if (Common.Is23113())
            {
                PhoneType = EPhoneType.PhoneTwoAPlus;
                Codename = Glyph.Device23113!;
            }
            else if (Common.Is24111())
            {
                PhoneType = EPhoneType.PhoneThreeA;
                Codename = Glyph.Device24111!;
            }
            else
                throw new IndexOutOfRangeException("Unknown device type.");

            _activeZoneMappingType = ZoneMapper.GetZoneTypeForDevice(PhoneType);

            //The docs would make it seem like the string to pass is "DEVICE_<ID>" but this is not true and rather the device codename is retrived by using the constant DEVICE_<ID>
            if (!_glyphManager.Register(Codename))
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

        public async Task DrawFrame<TZone>(IReadOnlyDictionary<TZone, int> source) where TZone : struct, System.Enum
        {
            if (typeof(TZone) != _activeZoneMappingType)
                throw new ArgumentException("Unsupported zone mapping type for this device.");

            int[] ledArray = ZoneMapper.ZoneToArray(source);

            _glyphManager.SetFrameColors(ledArray);
        }
    }
}
