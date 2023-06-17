namespace Glyphy.LED
{
    /// <summary>
    /// A wapper class for platform specific LED APIs, overengineered just for the sake of testing :)
    /// </summary>
    //We don't need to impliment the abstract method here as they are split off into platform specific partial classes.
    public partial class API : ALEDAPI
    {
        //These two values are used for the API brightness values but will be converted to system values by the platform specific partial classes.
        public const float MIN_BRIGHTNESS = 0.0f;
        public const float MAX_BRIGHTNESS = 1.0f;

        private static readonly object _LOCK = new object();

        private static API? _instance = null;

        public static bool Running => _instance is not null;

        //I am using this to make the API only initalize when called (as opposed to a static constructor).
        public static API Instance
        {
            get
            {
                //It is quicker to check if the instance is null before locking, at which point it will be ok to peform the check again.
                if (_instance is null)
                {
                    lock (_LOCK)
                    {
                        if (_instance is null)
                        {
                            _instance = new();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
