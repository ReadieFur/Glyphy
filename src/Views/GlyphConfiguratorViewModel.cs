using Glyphy.Animation;
using Glyphy.Glyph;
using Glyphy.Glyph.Indexes;
using Glyphy.Misc;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Glyphy.Views
{
    internal class GlyphConfiguratorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool SetProperty<T>(ref T obj, T value, List<string>? additionalTriggers = null, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(obj, value))
                return false;

            obj = value;

            PropertyChanged?.Invoke(this, new(name));
            if (additionalTriggers is not null)
                foreach (string trigger in additionalTriggers)
                    PropertyChanged?.Invoke(this, new(trigger));

            return true;
        }

        public int TotalFrameCount { get => _totalFrameCount; set => SetProperty(ref _totalFrameCount, value, [nameof(ShiftRightButtonEnabled), nameof(InsertFrameRightEnabled)]); }
        private int _totalFrameCount = 1;
        public int CurrentFrameIndex { get => _currentFrameIndex; set => SetProperty(ref _currentFrameIndex, value,
            [nameof(DeleteButtonEnabled), nameof(ShiftLeftButtonEnabled), nameof(ShiftRightButtonEnabled), nameof(InsertFrameLeftEnabled), nameof(InsertFrameRightEnabled)]); }
        private int _currentFrameIndex = 0;

        public bool DeleteButtonEnabled => CurrentFrameIndex != 0;
        public bool ShiftLeftButtonEnabled => CurrentFrameIndex > 1;
        public bool ShiftRightButtonEnabled => CurrentFrameIndex != 0 && CurrentFrameIndex != TotalFrameCount -1;
        public bool InsertFrameLeftEnabled => CurrentFrameIndex != 0;
        public bool InsertFrameRightEnabled => CurrentFrameIndex != TotalFrameCount - 1;

        public ObservableCollection<string> GlyphOptions { get => _glyphOptions; set => SetProperty(ref _glyphOptions, value, [nameof(SelectedGlyphOptionString), nameof(SelectedGlyphOption)]); }
        private ObservableCollection<string> _glyphOptions = new();
        public string SelectedGlyphOptionString
        {
            get => SelectedGlyphOption.Key;
            set => SelectedGlyphOption = new(GlyphAPI.Instance.PhoneType, value);
        }
        public SPhoneIndex SelectedGlyphOption { get => _selectedGlyphOption; set => SetProperty(ref _selectedGlyphOption, value); }
        public SPhoneIndex _selectedGlyphOption = new();

        public ObservableCollection<string> InterpolationOptions { get; } = [.. Enum.GetNames<EInterpolationType>()];
        public string SelectedInterpolationOptionString
        {
            get => Enum.GetName(SelectedInterpolationOption)!;
            set => SelectedInterpolationOption = Enum.Parse<EInterpolationType>(value);
        }
        public EInterpolationType SelectedInterpolationOption { get => _selectedInterpolationOption; set => SetProperty(ref _selectedInterpolationOption, value); }
        public EInterpolationType _selectedInterpolationOption = EInterpolationType.None;

        public double Brightness { get => _brightness; set => SetProperty(ref _brightness, Math.Clamp(Math.Round(value, 0), 0, 100)); }
        private double _brightness = 50;

        private bool _timestampUpdating = false;
        public double PreviousTimestampValue { get => _previousTimestamp; set => SetProperty(ref _previousTimestamp, value, [nameof(TimestampSlider)]); }
        private double _previousTimestamp = 0;
        public double TimestampValue
        {
            get => _timestamp;
            set
            {
                if (!_timestampUpdating && value >= PreviousTimestampValue && value <= NextTimestampValue)
                {
                    _timestampUpdating = true;
                    SetProperty(ref _timestamp, Math.Round(value, 0), [nameof(TimestampSlider)]);
                    _timestampUpdating = false;
                }
            }
        }
        public double TimestampSlider
        {
            get => MathHelpers.ConvertNumberRange(_timestamp, PreviousTimestampValue, NextTimestampValue, 0, 1000);
            set
            {
                if (!_timestampUpdating)
                {
                    _timestampUpdating = true;
                    SetProperty(ref _timestamp, MathHelpers.ConvertNumberRange(Math.Round(value, 0), 0, 1000, PreviousTimestampValue, NextTimestampValue), [nameof(TimestampValue)]);
                    _timestampUpdating = false;
                }
            }
        }
        private double _timestamp = 500;
        public double NextTimestampValue { get => _nextTimestamp; set => SetProperty(ref _nextTimestamp, value, [nameof(TimestampSlider)]); }
        private double _nextTimestamp = 1000;
    }
}
