namespace Glyphy.Animation
{
    public struct SKeyframe : IComparable<SKeyframe>
    {
        public long Timestamp { get; set; } = 0; //Time in milliseconds
        public double Brightness { get; set; } = 0; //Brightness 0-1.
        public EInterpolationType Interpolation { get; set; } = EInterpolationType.None;
        public Point? InTangent { get; set; } = null; //Coordinate 0-1 relative to timestamp, brightness and sourrounding frames.
        public Point? OutTangent { get; set; } = null; //Same as above.

        public SKeyframe() { }

        public int CompareTo(SKeyframe other) => Timestamp.CompareTo(other.Timestamp);
    }
}
