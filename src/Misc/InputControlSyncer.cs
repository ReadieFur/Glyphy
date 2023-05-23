using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Glyphy.Misc
{
    public class InputControlSyncer<TValue> : IDisposable where TValue : struct
    {
        /// <summary>
        /// This fires with the new value before the stored value is changed.
        /// T1 is the new value, T2 is the sender, the return value if given is the corrected value.
        /// </summary>
        public event Func<TValue, object, TValue?>? ValueChanged;
        public TValue Value { get; private set; } = default;

        private readonly SemaphoreSlim _lock = new(1);
        private readonly Type tType = typeof(TValue);
        private List<Slider> sliders = new();
        private List<Entry> entries = new();
        //private int passedTriggers = 0;

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
            InternalUpdate(e.NewValue, sender);

        private void Entry_TextChanged(object? sender, TextChangedEventArgs e) =>
            InternalUpdate(e.NewTextValue, sender);

        private void InternalUpdate(object newValue, object? sender, bool fireEvent = true)
        {
            //I can't for the life of me figure out or get a resonable solution as to why this event insists on firing twice despite me having the lock.
            //Removing the event handler, and re-adding it dosen't help because the event somehow picks up after I re-add it.
            //That being said however, it does seem to consistently fire twice, though if I skip this second event, the value dosen't get updated. This is EXTREMELY odd to me.
            if (!_lock.Wait(0))
                return;
            /*if (++passedTriggers >= 2)
            {
                passedTriggers = 0;
                return;
            }*/

            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(newValue.GetType());
                double? doubleValueNew = null;

                if (newValue.GetType() == typeof(string)
                    && tType == typeof(double))
                {
                    //Temporary fix for the type converter not being able to convert from string to double even though it is possible.
                    //The proper fix would be to create a custom type converter for this scenario.
                    doubleValueNew = string.IsNullOrEmpty((string)newValue) ? 0 : double.Parse((string)newValue);
                }

                #region TValue
                if (fireEvent)
                {
                    TValue? tValueNew = (TValue?)(doubleValueNew is not null ? doubleValueNew : converter.ConvertTo(newValue, typeof(TValue)));
                    if (tValueNew is null)
                        return;

                    //Call synchronously.
                    foreach (Delegate callback in ValueChanged?.GetInvocationList() ?? Array.Empty<Delegate>())
                    {
                        object? result = callback.DynamicInvoke(tValueNew, sender);
                        if (result is not null && result is TValue tValue)
                        {
                            newValue = tValue;

                            if (doubleValueNew is not null)
                                doubleValueNew = (double)TypeDescriptor.GetConverter(typeof(TValue)).ConvertTo(doubleValueNew, typeof(double))!;
                        }
                    }
                }
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

        public void SetValue(TValue value, object? sender = null, bool fireEvent = false) =>
            InternalUpdate(value, sender ?? this, fireEvent);
    }
}
