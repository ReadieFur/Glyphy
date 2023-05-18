using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glyphy.LED
{
    //This platform is just for testing purposes and so does not need to be super efficient.
    //The whole reason I am making a Windows platform is building the app for Android takes ages and Hot Reload dosent seem to work consistently.
    public partial class API : ALEDAPI
    {
        private Dictionary<EAddressable, uint> brightnessValues;

        public override uint MaxBrightness => 4095;

        public API()
        {
            brightnessValues = new Dictionary<EAddressable, uint>();
            foreach (string key in Enum.GetNames<EAddressable>())
                brightnessValues.Add(Enum.Parse<EAddressable>(key), 0);
        }

        public override Task<uint> GetBrightness(EGroup ledGroup)
        {
            List<uint> addressableLEDValues = new();

            switch (ledGroup)
            {
                case EGroup.CAMERA:
                    addressableLEDValues.Add(brightnessValues[EAddressable.CAMERA]);
                    break;
                case EGroup.DIAGONAL:
                    addressableLEDValues.Add(brightnessValues[EAddressable.DIAGONAL]);
                    break;
                case EGroup.CENTER:
                    addressableLEDValues.Add(brightnessValues[EAddressable.CENTER_TOP_LEFT]);
                    addressableLEDValues.Add(brightnessValues[EAddressable.CENTER_TOP_RIGHT]);
                    addressableLEDValues.Add(brightnessValues[EAddressable.CENTER_BOTTOM_LEFT]);
                    addressableLEDValues.Add(brightnessValues[EAddressable.CENTER_BOTTOM_RIGHT]);
                    break;
                case EGroup.LINE:
                    addressableLEDValues.Add(brightnessValues[EAddressable.LINE_1]);
                    addressableLEDValues.Add(brightnessValues[EAddressable.LINE_2]);
                    addressableLEDValues.Add(brightnessValues[EAddressable.LINE_3]);
                    addressableLEDValues.Add(brightnessValues[EAddressable.LINE_4]);
                    addressableLEDValues.Add(brightnessValues[EAddressable.LINE_5]);
                    addressableLEDValues.Add(brightnessValues[EAddressable.LINE_6]);
                    addressableLEDValues.Add(brightnessValues[EAddressable.LINE_7]);
                    break;
                case EGroup.DOT:
                    addressableLEDValues.Add(brightnessValues[EAddressable.DOT]);
                    break;
            }

            uint averageBrightness = 0;
            foreach (uint brightness in addressableLEDValues)
                averageBrightness += brightness;
            averageBrightness /= (uint)addressableLEDValues.Count;

            return Task.FromResult(averageBrightness);
        }

        public override Task<uint> GetBrightness(EAddressable addressableLED) =>
            Task.FromResult(brightnessValues[addressableLED]);

        public override Task SetBrightness(EGroup ledGroup, uint brightness)
        {
            List<EAddressable> addressableLEDs = new();

            switch (ledGroup)
            {
                case EGroup.CAMERA:
                    addressableLEDs.Add(EAddressable.CAMERA);
                    break;
                case EGroup.DIAGONAL:
                    addressableLEDs.Add(EAddressable.DIAGONAL);
                    break;
                case EGroup.CENTER:
                    addressableLEDs.Add(EAddressable.CENTER_TOP_LEFT);
                    addressableLEDs.Add(EAddressable.CENTER_TOP_RIGHT);
                    addressableLEDs.Add(EAddressable.CENTER_BOTTOM_LEFT);
                    addressableLEDs.Add(EAddressable.CENTER_BOTTOM_RIGHT);
                    break;
                case EGroup.LINE:
                    addressableLEDs.Add(EAddressable.LINE_1);
                    addressableLEDs.Add(EAddressable.LINE_2);
                    addressableLEDs.Add(EAddressable.LINE_3);
                    addressableLEDs.Add(EAddressable.LINE_4);
                    addressableLEDs.Add(EAddressable.LINE_5);
                    addressableLEDs.Add(EAddressable.LINE_6);
                    addressableLEDs.Add(EAddressable.LINE_7);
                    break;
                case EGroup.DOT:
                    addressableLEDs.Add(EAddressable.DOT);
                    break;
            }

            foreach (EAddressable addressableLED in addressableLEDs)
                brightnessValues[addressableLED] = brightness;

            return Task.CompletedTask;
        }

        public override Task SetBrightness(EAddressable addressableLED, uint brightness)
        {
            brightnessValues[addressableLED] = brightness;
            return Task.CompletedTask;
        }
    }
}
