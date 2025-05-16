using Glyphy.Misc;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Glyphy.Controls.Bezier
{
    public class BezierGraphViewModel : INotifyPropertyChanged
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

        //Styling.
        public double KeyframeDiameter { get => _keyframeDiameter; set => SetProperty(ref _keyframeDiameter, value); }
        private double _keyframeDiameter = 8;
        public double TangentDiameter { get => _tangentDiameter; set => SetProperty(ref _tangentDiameter, value); }
        private double _tangentDiameter = 6;
        public double BezierThickness { get => _bezierThickness; set => SetProperty(ref _bezierThickness, value); }
        private double _bezierThickness = 2;
        public double TangentThickness { get => _tangentThickness; set => SetProperty(ref _tangentThickness, value); }
        private double _tangentThickness = 1;

        //Points.
        public double PreviousX { get => _previousX; set => SetProperty(ref _previousX, value, [nameof(PreviousPoint)]); }
        private double _previousX = -1;
        public double PreviousY { get => _previousY; set => SetProperty(ref _previousY, Math.Clamp(value, -1, 1), [nameof(PreviousPoint)]); }
        private double _previousY = -1;
        public double PreviousOutX { get => _previousOutX; set => SetProperty(ref _previousOutX, value, [nameof(PreviousOutTangent)]); }
        private double _previousOutX = -0.5;
        public double PreviousOutY { get => _previousOutY; set => SetProperty(ref _previousOutY, Math.Clamp(value, -1, 1), [nameof(PreviousOutTangent)]); }
        private double _previousOutY = -1;

        public double CurrentInX
        {
            get => _currentInX;
            set
            {
                if (value > CurrentX)
                    value = CurrentX;
                SetProperty(ref _currentInX, value, [nameof(CurrentInTangent)]);
            }
        }
        private double _currentInX = -0.5;
        public double CurrentInY { get => _currentInY; set => SetProperty(ref _currentInY, Math.Clamp(value, -1, 1), [nameof(CurrentInTangent)]); }
        private double _currentInY = 0;
        public double CurrentX
        {
            get => _currentX;
            set
            {
                if (value < PreviousOutX)
                    value = PreviousOutX;

                if (CurrentInX > value)
                    SetProperty(ref _currentInX, value, [nameof(CurrentInTangent)], nameof(CurrentInX));

                if (value > NextInX)
                    value = NextInX;

                SetProperty(ref _currentX, value, [nameof(CurrentPoint), nameof(CurrentTimestamp), nameof(CurrentTimestampText)]);

                if (CurrentOutX < value)
                    SetProperty(ref _currentOutX, value, [nameof(CurrentOutTangent)], nameof(CurrentOutX));
            }
        }
        private double _currentX = 0;
        public double CurrentY { get => _currentY; set => SetProperty(ref _currentY, Math.Clamp(value, -1, 1), [nameof(CurrentPoint)]); }
        private double _currentY = 0;
        public double CurrentOutX
        {
            get => _currentOutX;
            set
            {
                if (value < CurrentX)
                    value = CurrentX;
                SetProperty(ref _currentOutX, value, [nameof(CurrentOutTangent)]);
            }
        }
        private double _currentOutX = 0.5;
        public double CurrentOutY { get => _currentOutY; set => SetProperty(ref _currentOutY, Math.Clamp(value, -1, 1), [nameof(CurrentOutTangent)]); }
        private double _currentOutY = 0;

        public double NextInX { get => _nextInX; set => SetProperty(ref _nextInX, value, [nameof(NextInTangent)]); }
        private double _nextInX = 0.5;
        public double NextInY { get => _nextInY; set => SetProperty(ref _nextInY, Math.Clamp(value, -1, 1), [nameof(NextInTangent)]); }
        private double _nextInY = 1;
        public double NextX { get => _nextX; set => SetProperty(ref _nextX, value, [nameof(NextPoint)]); }
        private double _nextX = 1;
        public double NextY { get => _nextY; set => SetProperty(ref _nextY, Math.Clamp(value, -1, 1), [nameof(NextPoint)]); }
        private double _nextY = 1;

        public double PlayheadX { get => _playheadX; set => SetProperty(ref _playheadX, value); }
        private double _playheadX = -1;
        public double PlayheadY { get => _playheadY; set => SetProperty(ref _playheadY, Math.Clamp(value, -1, 1)); }
        private double _playheadY = -1;

        //Labels.
        public string TimestampSuffix { get => _timestampSuffix; set => SetProperty(ref _timestampSuffix, value, [nameof(PreviousTimestampText), nameof(CurrentTimestampText), nameof(NextTimestampText)]); }
        private string _timestampSuffix = "ms";
        public double PreviousTimestamp { get => _previousTimestamp; set => SetProperty(ref _previousTimestamp, value, [nameof(PreviousTimestampText)]); }
        private double _previousTimestamp = 0;
        public string PreviousTimestampText => Math.Floor(PreviousTimestamp) + TimestampSuffix;
        //public double CurrentTimestamp { get => _currentTimestamp; set => SetProperty(ref _currentTimestamp, value, [nameof(CurrentTimestampText)]); } private double _currentTimestamp = 500;
        public double CurrentTimestamp => MathHelpers.ConvertNumberRange(CurrentX, -1, 1, PreviousTimestamp, NextTimestamp);
        public string CurrentTimestampText => Math.Floor(CurrentTimestamp) + TimestampSuffix;
        public double NextTimestamp { get => _nextTimestamp; set => SetProperty(ref _nextTimestamp, value, [nameof(NextTimestampText)]); }
        private double _nextTimestamp = 1000;
        public string NextTimestampText => Math.Floor(NextTimestamp) + TimestampSuffix;

        //Points.
        public Point PreviousPoint { get => new(PreviousX, PreviousY); set { PreviousX = value.X; PreviousY = value.Y; } }
        public Point PreviousOutTangent { get => new(PreviousOutX, PreviousOutY); set { PreviousOutX = value.X; PreviousOutY = value.Y; } }

        public Point CurrentInTangent { get => new(CurrentInX, CurrentInY); set { CurrentInX = value.X; CurrentInY = value.Y; } }
        public Point CurrentPoint { get => new(CurrentX, CurrentY); set { CurrentX = value.X; CurrentY = value.Y; } }
        public Point CurrentOutTangent { get => new(CurrentOutX, CurrentOutY); set { CurrentOutX = value.X; CurrentOutY = value.Y; } }

        public Point NextInTangent { get => new(NextInX, NextInY); set { NextInX = value.X; NextInY = value.Y; } }
        public Point NextPoint { get => new(NextX, NextY); set { NextX = value.X; NextY = value.Y; } }
    }
}
