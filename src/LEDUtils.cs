using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JProcess = Java.Lang.Process;

#nullable enable
namespace Glyphy
{
    public class LEDUtils
    {
        private const string BASE_PATH = "/sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led";

        private JProcess? rootProcess = null;
        private DataOutputStream? inputStream = null;
        private DataInputStream? outputStream = null;

        public readonly uint MAX_BRIGHTNESS;

        internal LEDUtils(JProcess rootProcess)
        {
            //TODO: Move the root process creation to an external method (i.e. not the constructor).
            this.rootProcess = rootProcess;
            inputStream = new DataOutputStream(rootProcess?.OutputStream);
            outputStream = new DataInputStream(rootProcess?.InputStream);

            //TODO: Possibly make this re-callable.
            string? result = Exec("cat " + BASE_PATH + "/brightness");
            if (string.IsNullOrEmpty(result))
                throw new NullReferenceException("Unable to get max brightness.");
            MAX_BRIGHTNESS = uint.Parse(result);
        }

        private string? Exec(string command)
        {
            if (rootProcess is null || inputStream is null || outputStream is null)
                throw new NullReferenceException("Unable to execute command.");

            inputStream.WriteBytes(command + "\n");
            inputStream.Flush();

            #pragma warning disable CS0618 // Type or member is obsolete
            return outputStream.ReadLine();
            #pragma warning restore CS0618
        }

        private uint ClampBrightness(uint brightness) =>
            Math.Clamp(brightness, 0, MAX_BRIGHTNESS);

        public void SetLED(ELEDGroup ledGroup, uint brightness)
        {
            brightness = ClampBrightness(brightness);

            string deviceID = ledGroup switch
            {
                ELEDGroup.CAMERA => "rear_cam_led_br",
                ELEDGroup.DIAGONAL => "front_cam_led_br",
                ELEDGroup.MIDDLE => "round_leds_br",
                ELEDGroup.LINE => "vline_leds_br",
                ELEDGroup.DOT => "dot_led_br",
                _ => throw new IOException("Invalid device ID.")
            };

            Exec($"echo {brightness} > {BASE_PATH}/{deviceID}");
        }

        public void SetLED(ELEDAddressable ledAddressable, uint brightness)
        {
            brightness = ClampBrightness(brightness);

            uint deviceID = ledAddressable switch
            {
                ELEDAddressable.DIAGONAL => 1,
                ELEDAddressable.CENTER_BOTTOM_LEFT => 2,
                ELEDAddressable.CENTER_BOTTOM_RIGHT => 3,
                ELEDAddressable.CENTER_TOP_LEFT => 5,
                ELEDAddressable.CENTER_TOP_RIGHT => 4,
                ELEDAddressable.CAMERA => 7,
                ELEDAddressable.LINE_1 => 13,
                ELEDAddressable.LINE_2 => 11,
                ELEDAddressable.LINE_3 => 9,
                ELEDAddressable.LINE_4 => 12,
                ELEDAddressable.LINE_5 => 10,
                ELEDAddressable.LINE_6 => 14,
                ELEDAddressable.LINE_7 => 15,
                ELEDAddressable.LINE_8 => 8,
                ELEDAddressable.DOT => 16,
                ELEDAddressable.RECORDING_LED => 17,
                _ => throw new IOException("Invalid device ID.")
            };

            Exec($"echo {deviceID} {brightness} > {BASE_PATH}/single_led_br");
        }
    }
}
