using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Glyphy.Misc
{
    public class InputControlSyncer<TValue> : IDisposable
    {
        /// <summary>
        /// T1 is the new value, T2 is the old value.
        /// </summary>
        public event EventHandler<(TValue, TValue)>? ValueChanged;

        private readonly SemaphoreSlim _lock = new(1);
        private readonly Type tType = typeof(TValue);
        private List<Slider> sliders = new();
        private List<Entry> entries = new();

        public InputControlSyncer()
        {
            if (tType != typeof(string)
                && tType != typeof(double))
                throw new InvalidOperationException("Only string and double are supported.");
        }

        public void Dispose() =>
            _lock.Dispose();

        public void AddControl(Slider slider)
        {
            if (typeof(TValue) == typeof(string))
                throw new InvalidOperationException("Cannot use a Slider with a string value.");

            sliders.Add(slider);
            slider.ValueChanged += Slider_ValueChanged;
        }

        public void AddControl(Entry entry)
        {
            entries.Add(entry);
            entry.TextChanged += Entry_TextChanged;
        }

        private void Slider_ValueChanged(object? sender, ValueChangedEventArgs e) =>
            InternalUpdate(e.NewValue, e.OldValue, sender);

        private void Entry_TextChanged(object? sender, TextChangedEventArgs e) =>
            InternalUpdate(e.NewTextValue, e.OldTextValue, sender);

        private void InternalUpdate(object newValue, object oldValue, object? sender, bool fireEvent = true)
        {
            if (!_lock.Wait(0))
                return;

            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(newValue.GetType());

                double? doubleValueNew = null;
                double? doubleValueOld = null;
                if (newValue.GetType() == typeof(string)
                    && tType == typeof(double))
                {
                    //Temporary fix for the type converter not being able to convert from string to double even though it is possible.
                    //The proper fix would be to create a custom type converter for this scenario.
                    doubleValueNew = double.Parse((string)newValue);
                    doubleValueOld = double.Parse((string)oldValue);
                }

                #region TValue
                TValue? tValueNew = (TValue?)(doubleValueNew != null ? doubleValueNew : converter.ConvertTo(newValue, typeof(TValue)));
                if (tValueNew is null)
                    return;

                TValue? tValueOld = (TValue?)(doubleValueNew != null ? doubleValueOld : converter.ConvertTo(oldValue, typeof(TValue)));
                if (tValueOld is null)
                    return;

                ValueChanged?.Invoke(sender, (tValueNew, tValueOld));
                #endregion

                #region Double
                if (tType == typeof(double))
                {
                    double? doubleValue = (double?)(doubleValueNew != null ? doubleValueNew : converter.ConvertTo(newValue, typeof(double)));
                    if (doubleValue is null)
                        return;

                    foreach (Slider slider in sliders)
                        slider.Value = doubleValue.Value;
                }
                #endregion

                #region String
                string? stringValue = (string?)converter.ConvertTo(newValue, typeof(string));
                if (stringValue is null)
                    return;

                foreach (Entry entry in entries)
                    entry.Text = stringValue;
                #endregion
            }
            catch (Exception ex)
            {
                //Invalid value.
            }
            finally
            {
                _lock.Release();
            }
        }

        public void Update(TValue value) =>
            InternalUpdate(value, default(TValue), null, false);
    }
}
