//#define ALLOW_GROUP_CACHE_BYPASS

using System;
using System.Collections.Generic;
using Java.IO;
using JProcess = Java.Lang.Process;

namespace Glyphy.Platforms.Android.LED
{
    //TODO: Use a more elegant solution.
    //Some "odd" choices have been made here but they were in the intrest of making the API as fast as possible as it will be called very frequently.
    public class API
    {
        private static API? _instance = null;

        public bool Running { get; private set; } = false;

        //Initalize API when called.
        /// <summary>
        /// Will throw if permissions are not granted upon initialization.
        /// </summary>
        public static API Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new API();
                return _instance;
            }
        }

        private const string BASE_PATH = "/sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led";


        public readonly uint MaxBrightness;

        private JProcess? rootProcess = null;
        private DataOutputStream? inputStream = null;
        private DataInputStream? outputStream = null;
        //Yes I could've used a dictionary here but this method is slightly faster.
        private uint[] cachedBrightnessValues = new uint[15];

        //internal API(JProcess rootProcess)
        internal API()
        {
            //TODO: Move the root process creation to an external method (i.e. not the constructor).
            //this.rootProcess = rootProcess;
            if (!Helpers.CreateRootSubProcess(out rootProcess))
                throw new UnauthorizedAccessException("Unable to create root process.");

            inputStream = new DataOutputStream(rootProcess?.OutputStream);
            outputStream = new DataInputStream(rootProcess?.InputStream);

            //TODO: Possibly make this re-callable.
            string? result = Exec("cat " + BASE_PATH + "/brightness", captureOutput: true);
            if (string.IsNullOrEmpty(result))
                throw new NullReferenceException("Unable to get max brightness.");
            MaxBrightness = uint.Parse(result);

            Running = true;
        }

        //TODO: Make this async.
        private string? Exec(string command, bool captureOutput = false)
        {
            if (rootProcess is null || inputStream is null || outputStream is null)
                throw new NullReferenceException("Unable to execute command.");

            inputStream.WriteBytes(command + "\n");
            inputStream.Flush();

#pragma warning disable CS0618 // Type or member is obsolete
            return captureOutput ? outputStream.ReadLine() : null;
#pragma warning restore CS0618
        }

        private string GetGroupSystemName(EGroup ledGroup)
        {
            return ledGroup switch
            {
                EGroup.CAMERA => "rear_cam_led_br",
                EGroup.DIAGONAL => "front_cam_led_br",
                EGroup.MIDDLE => "round_leds_br",
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

        public uint ClampBrightness(int brightness) =>
            (uint)Math.Clamp(brightness, 0, MaxBrightness);

        public uint GetBrightness(EGroup ledGroup)
        {
            string? result = Exec("cat " + BASE_PATH + "/" + GetGroupSystemName(ledGroup), captureOutput: true);
            if (result is null)
                throw new IOException("Failed to get brightness.");
            else if (!uint.TryParse(result, out uint parsedResult))
                throw new FormatException("Brightness value is invalid.");
            else
                return parsedResult;
        }

        //TODO: Figure out how to read individual LEDs.
        //Temporary solution: Cache set values.
        public uint GetBrightness(EAddressable ledAddressable) =>
            cachedBrightnessValues[GetCachedBrightnessIndex(ledAddressable)];

        public void SetBrightness(EGroup ledGroup, uint brightness
#if ALLOW_GROUP_CACHE_BYPASS
            , bool updateCachedValues = true
#endif
            )
        {
#if ALLOW_GROUP_CACHE_BYPASS
            if (updateCachedValues)
#endif
            {
                switch (ledGroup)
                {
                    case EGroup.CAMERA:
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CAMERA)] = brightness;
                        break;
                    case EGroup.DIAGONAL:
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.DIAGONAL)] = brightness;
                        break;
                    case EGroup.MIDDLE:
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CENTER_TOP_LEFT)] = brightness;
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CENTER_TOP_RIGHT)] = brightness;
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CENTER_BOTTOM_LEFT)] = brightness;
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CENTER_BOTTOM_RIGHT)] = brightness;
                        break;
                    case EGroup.LINE:
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_1)] = brightness;
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_2)] = brightness;
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_3)] = brightness;
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_4)] = brightness;
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_5)] = brightness;
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_6)] = brightness;
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_7)] = brightness;
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.LINE_8)] = brightness;
                        break;
                    case EGroup.DOT:
                        cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.DOT)] = brightness;
                        break;
                    default:
                        throw new KeyNotFoundException("Invalid device ID.");
                }
            }

            Exec($"echo {brightness} > {BASE_PATH}/{GetGroupSystemName(ledGroup)}");
        }

        public void SetBrightness(EAddressable addressableLED, uint brightness)
        {
            cachedBrightnessValues[GetCachedBrightnessIndex(addressableLED)] = brightness;
            Exec($"echo {GetAddressableSystemID(addressableLED)} {brightness} > {BASE_PATH}/single_led_br");
        }
    }
}
