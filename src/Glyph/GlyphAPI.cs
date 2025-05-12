namespace Glyphy.Glyph;

public partial class GlyphAPI : IGlyphAPI
{
    private static readonly object _LOCK = new();

    private static IGlyphAPI? _instance = null;
    //I am using this to make the API only initalize when called (as opposed to a static constructor).
    public static IGlyphAPI Instance
    {
        get
        {
            //It is quicker to check if the instance is null before locking, at which point it will be ok to peform the check again.
            if (_instance is null)
                lock (_LOCK)
                    if (_instance is null)
                        _instance = new GlyphAPI();
            return _instance;
        }
    }
}
