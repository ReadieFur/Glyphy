namespace Glyphy.LED
{
    /// <summary>
    /// A wapper class for platform specific LED APIs, overengineered just for the sake of testing :)
    /// </summary>
    //We don't need to impliment the abstract method here as they are split off into platform specific partial classes.
    public partial class API : ALEDAPI
    {
        private static API? _instance = null;

        public static bool Running { get; private set; } = false;

        //I am using this to make the API only initalize when called (as opposed to a static constructor).
        public static API Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new();
                Running = true;
                return _instance;
            }
        }

        public uint ClampBrightness(int brightness) =>
            (uint)Math.Clamp(brightness, 0, MaxBrightness);
    }
}
