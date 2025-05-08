using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Glyphy.LED.Zones
{
    public class ZoneMapper
    {
        /// <summary>
        /// Converts an input zone map to the desired platform.
        /// </summary>
        /// <typeparam name="TTargetMapping">The zone map to convert to.</typeparam>
        /// <typeparam name="TIn">The zone map to convert from, either a string or variant of EPhoneZones.</typeparam>
        /// <param name="source">The zone map to convert from.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Dictionary<TTargetMapping, int> ZoneToZone<TTargetMapping, TIn>(IReadOnlyDictionary<TIn, int> source) where TTargetMapping : struct, Enum where TIn : class, Enum
        {
            Type tOutType = typeof(TTargetMapping);
            if (!IsEnumZone<TTargetMapping>())
                throw new ArgumentException("Unsupported TTargetMapping type");

            Type tInType = typeof(TIn);
            if (tInType != typeof(string) &&
                IsEnumZone<TIn>())
                throw new ArgumentException("Unsupported TIn type");

            Dictionary<TTargetMapping, int> outMapping = new();

            foreach (var kvp in source)
            {
                string? key = tInType.IsEnum ? tInType.GetEnumName(kvp.Key) : kvp.Key as string;

                if (!Enum.TryParse(key, true, out TTargetMapping outKey))
                    continue;

                outMapping.Add(outKey, kvp.Value);
            }

            return outMapping;
        }

        /// <summary>
        /// Converts a typed zone to an LED array.
        /// </summary>
        /// <typeparam name="TZone">A variant of EPhoneZones</typeparam>
        /// <returns>A list of LED brightness values to be passed to GlyphManager.SetFrameColors()</returns>
        public static int[] ZoneToArray<TZone>(IReadOnlyDictionary<TZone, int> source) where TZone : struct, Enum
        {
            Type tZoneType = typeof(TZone);
            if (!IsEnumZone<TZone>())
                throw new ArgumentException("Unsupported TZoneType type");

            Dictionary<TZone, int> copy = new(source);

            foreach (TZone value in tZoneType.GetEnumValues())
                if (!copy.ContainsKey(value))
                    copy.Add(value, 0);

            return copy
                .OrderBy(kvp => (ushort)(object)kvp.Key)
                .Select(kvp => kvp.Value)
                .ToArray();
        }

        public static Dictionary<TZone, int> ArrayToZone<TZone>(int[] source) where TZone : struct, Enum
        {
            Type tZoneType = typeof(TZone);
            if (!IsEnumZone<TZone>())
                throw new ArgumentException("Unsupported TZoneType type");

            if (source.Length != tZoneType.GetEnumValues().Length)
                throw new ArgumentOutOfRangeException("Source length does not match the expected length for the given zone type.");

            Dictionary<TZone, int> mapping = new();
            for (ushort i = 0; i < source.Length; i++)
                mapping.Add((TZone)Enum.ToObject(typeof(TZone), i), source[i]);
            return mapping;
        }

        private static bool IsEnumZone<TZone>()
        {
            Type tZoneType = typeof(TZone);
            return tZoneType == typeof(EPhoneOneZones) ||
                tZoneType != typeof(EPhoneTwoZones) ||
                tZoneType != typeof(EPhoneTwoAZones) ||
                tZoneType != typeof(EPhoneThreeAZones);
        }
    }
}
