﻿//#define USE_ASYNC_STREAM

using SIOException = System.IO.IOException;
using Java.IO;
using JIOException = Java.IO.IOException;
using JProcess = Java.Lang.Process;
using Glyphy.Platforms.Android;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glyphy.LED
{
    //Some "odd" choices have been made here but they were in the intrest of making the API as fast as possible as it will be called very frequently.
    public partial class API : ALEDAPI, IDisposable
    {
        private const string BASE_PATH = "/sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led";

        private JProcess rootProcess;
        private DataOutputStream inputStream;
        private DataInputStream outputStream;
        private uint maxBrightness = 0;
        //Yes I could've used a dictionary here but this method is slightly faster (I think).
        private uint[] cachedBrightnessValues = new uint[Enum.GetNames<EAddressable>().Length];

        public override uint MaxBrightness => maxBrightness;

        public API()
        {
            if (!Helpers.CreateRootSubProcess(out JProcess? _rootProcess)
                || _rootProcess is null
                || _rootProcess.OutputStream is null
                || _rootProcess.InputStream is null)
                throw new UnauthorizedAccessException("Unable to create root process.");
            rootProcess = _rootProcess;

            inputStream = new DataOutputStream(rootProcess.OutputStream);
            outputStream = new DataInputStream(rootProcess.InputStream);

            RefreshMaxBrightness().Wait();
        }

        //I'm not going to impliment a check to see if we have disposed. Just don't call this manually.
        public void Dispose()
        {
            inputStream.Dispose();
            outputStream.Dispose();
            rootProcess.Destroy();
        }

        //MMM lovley #if blocks.
        private
#if USE_ASYNC_STREAM
            async
#endif
        Task<string?> Exec(string command, bool captureOutput = false)
        {
            //The async methods seem to hang indefinitely so I will be using the synchronous method.
            //I find this odd because the synchronous methods are marked as deprecated, yet the comment on all of the the async method say "to be added".

            if (captureOutput)
            {
                try
                {
                    while (outputStream.Available() > 0)
#if USE_ASYNC_STREAM
                        await outputStream.SkipBytesAsync(outputStream.Available());
#else
                        outputStream.SkipBytes(outputStream.Available());
#endif
                }
                catch (JIOException) { /*Ignore*/ }
            }

#if USE_ASYNC_STREAM
            await inputStream.WriteBytesAsync(command + "\n");
            await inputStream.FlushAsync();
#else
            inputStream.WriteBytes(command + "\n");
            inputStream.Flush();
#endif

#if USE_ASYNC_STREAM
            return captureOutput ? await outputStream.ReadLineAsync() : null;
#else
#pragma warning disable CS0618 // Type or member is obsolete
            return Task.FromResult(captureOutput ? outputStream.ReadLine() : null);
#pragma warning restore CS0618
#endif
        }

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

        //Private for now.
        private async Task RefreshMaxBrightness()
        {
            string? result = await Exec("cat " + BASE_PATH + "/brightness", captureOutput: true);
            if (string.IsNullOrEmpty(result))
                throw new NullReferenceException("Unable to get max brightness.");
            maxBrightness = uint.Parse(result);
        }

        public async override Task<uint> GetBrightness(EGroup ledGroup)
        {
            string? result = await Exec("cat " + BASE_PATH + "/" + GetGroupSystemName(ledGroup), true);
            if (result is null)
                throw new SIOException("Failed to get brightness.");
            else if (!uint.TryParse(result, out uint parsedResult))
                throw new FormatException("Brightness value is invalid.");
            else
                return parsedResult;
        }

        public override Task<uint> GetBrightness(EAddressable adressableLED) =>
            Task.FromResult(cachedBrightnessValues[GetCachedBrightnessIndex(adressableLED)]);

        public async override Task SetBrightness(EGroup ledGroup, uint brightness)
        {
            switch (ledGroup)
            {
                case EGroup.CAMERA:
                    cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.CAMERA)] = brightness;
                    break;
                case EGroup.DIAGONAL:
                    cachedBrightnessValues[GetCachedBrightnessIndex(EAddressable.DIAGONAL)] = brightness;
                    break;
                case EGroup.CENTER:
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

            await Exec($"echo {brightness} > {BASE_PATH}/{GetGroupSystemName(ledGroup)}");
        }

        public async override Task SetBrightness(EAddressable addressableLED, uint brightness)
        {
            cachedBrightnessValues[GetCachedBrightnessIndex(addressableLED)] = brightness;
            await Exec($"echo {GetAddressableSystemID(addressableLED)} {brightness} > {BASE_PATH}/single_led_br");
        }
    }
}
