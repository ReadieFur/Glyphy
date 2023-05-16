using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java.IO;
using JProcess = Java.Lang.Process;

namespace Glyphy.Platforms.Android.LED
{
    public class Utils
    {
        private const string BASE_PATH = "/sys/devices/platform/soc/984000.i2c/i2c-0/0-0020/leds/aw210xx_led";

        private JProcess? rootProcess = null;
        private DataOutputStream? inputStream = null;
        private DataInputStream? outputStream = null;

        public readonly uint MAX_BRIGHTNESS;

        internal Utils(JProcess rootProcess)
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

        public void SetLED(EGroup ledGroup, uint brightness)
        {
            brightness = ClampBrightness(brightness);

            string deviceID = ledGroup switch
            {
                EGroup.CAMERA => "rear_cam_led_br",
                EGroup.DIAGONAL => "front_cam_led_br",
                EGroup.MIDDLE => "round_leds_br",
                EGroup.LINE => "vline_leds_br",
                EGroup.DOT => "dot_led_br",
                _ => throw new IOException("Invalid device ID.")
            };

            Exec($"echo {brightness} > {BASE_PATH}/{deviceID}");
        }

        public void SetLED(EAddressable ledAddressable, uint brightness)
        {
            brightness = ClampBrightness(brightness);

            uint deviceID = ledAddressable switch
            {
                EAddressable.DIAGONAL => 1,
                EAddressable.CENTER_BOTTOM_LEFT => 2,
                EAddressable.CENTER_BOTTOM_RIGHT => 3,
                EAddressable.CENTER_TOP_LEFT => 5,
                EAddressable.CENTER_TOP_RIGHT => 4,
                EAddressable.CAMERA => 7,
                EAddressable.LINE_1 => 13,
                EAddressable.LINE_2 => 11,
                EAddressable.LINE_3 => 9,
                EAddressable.LINE_4 => 12,
                EAddressable.LINE_5 => 10,
                EAddressable.LINE_6 => 14,
                EAddressable.LINE_7 => 15,
                EAddressable.LINE_8 => 8,
                EAddressable.DOT => 16,
                EAddressable.RECORDING_LED => 17,
                _ => throw new IOException("Invalid device ID.")
            };

            Exec($"echo {deviceID} {brightness} > {BASE_PATH}/single_led_br");
        }
    }
}
