using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Glyphy.Controls.Bezier
{
    public class BezierEditorViewModel : INotifyPropertyChanged
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
        public double PreviousY { get => _previousY; set => SetProperty(ref _previousY, value, [nameof(PreviousPoint)]); }
        private double _previousY = -1;
        public double PreviousOutX { get => _previousOutX; set => SetProperty(ref _previousOutX, value, [nameof(PreviousOutTangent)]); }
        private double _previousOutX = -0.5;
        public double PreviousOutY { get => _previousOutY; set => SetProperty(ref _previousOutY, value, [nameof(PreviousOutTangent)]); }
        private double _previousOutY = -1;

        public double CurrentInX { get => _currentInX; set => SetProperty(ref _currentInX, value, [nameof(CurrentInTangent)]); }
        private double _currentInX = -0.5;
        public double CurrentInY { get => _currentInY; set => SetProperty(ref _currentInY, value, [nameof(CurrentInTangent)]); }
        private double _currentInY = 0;
        public double CurrentX { get => _currentX; set => SetProperty(ref _currentX, value, [nameof(CurrentPoint), nameof(CurrentTimestamp), nameof(CurrentTimestampText)]); }
        private double _currentX = 0;
        public double CurrentY { get => _currentY; set => SetProperty(ref _currentY, value, [nameof(CurrentPoint)]); }
        private double _currentY = 0;
        public double CurrentOutX { get => _currentOutX; set => SetProperty(ref _currentOutX, value, [nameof(CurrentOutTangent)]); }
        private double _currentOutX = 0.5;
        public double CurrentOutY { get => _currentOutY; set => SetProperty(ref _currentOutY, value, [nameof(CurrentOutTangent)]); }
        private double _currentOutY = 0;

        public double NextInX { get => _nextInX; set => SetProperty(ref _nextInX, value, [nameof(NextInTangent)]); }
        private double _nextInX = 0.5;
        public double NextInY { get => _nextInY; set => SetProperty(ref _nextInY, value, [nameof(NextInTangent)]); }
        private double _nextInY = 1;
        public double NextX { get => _nextX; set => SetProperty(ref _nextX, value, [nameof(NextPoint)]); }
        private double _nextX = 1;
        public double NextY { get => _nextY; set => SetProperty(ref _nextY, value, [nameof(NextPoint)]); }
        private double _nextY = 1;

        public double PlayheadX { get => _playheadX; set => SetProperty(ref _playheadX, value); }
        private double _playheadX = -1;
        public double PlayheadY { get => _playheadY; set => SetProperty(ref _playheadY, value); }
        private double _playheadY = -1;

        //Labels.
        public double PreviousTimestamp { get => _previousTimestamp; set => SetProperty(ref _previousTimestamp, value, [nameof(PreviousTimestampText)]); }
        private double _previousTimestamp = 0;
        public string PreviousTimestampText => $"{Math.Floor(PreviousTimestamp)}ms";
        //public double CurrentTimestamp { get => _currentTimestamp; set => SetProperty(ref _currentTimestamp, value, [nameof(CurrentTimestampText)]); } private double _currentTimestamp = 500;
        public double CurrentTimestamp => Helpers.ConvertNumberRange(CurrentX, -1, 1, PreviousTimestamp, NextTimestamp);
        public string CurrentTimestampText => $"{Math.Floor(CurrentTimestamp)}ms";
        public double NextTimestamp { get => _nextTimestamp; set => SetProperty(ref _nextTimestamp, value, [nameof(NextTimestampText)]); }
        private double _nextTimestamp = 1000;
        public string NextTimestampText => $"{Math.Floor(NextTimestamp)}ms";

        //Points as System.Windows.Point.
        public Point PreviousPoint => new(PreviousX, PreviousY);
        public Point PreviousOutTangent => new(PreviousOutX, PreviousOutY);

        public Point CurrentInTangent => new(CurrentInX, CurrentInY);
        public Point CurrentPoint => new(CurrentX, CurrentY);
        public Point CurrentOutTangent => new(CurrentOutX, CurrentOutY);

        public Point NextInTangent => new(NextInX, NextInY);
        public Point NextPoint => new(NextX, NextY);
    }
}
