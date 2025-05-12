using Android.Content;
using Com.Nothing.Ketchum;
using KetchumGlyph = Com.Nothing.Ketchum.Glyph;
using Application = Android.App.Application;
using Android.Content.PM;
using Android.Provider;

namespace Glyphy.Glyph;

public partial class GlyphAPI : Java.Lang.Object, IGlyphAPI, GlyphManager.ICallback
{
    bool IGlyphAPI.IsReady => _readyEvent?.Task.IsCompletedSuccessfully ?? false;
    EPhoneType IGlyphAPI.PhoneType => _phoneType;

    private readonly object _STATE_LOCK = new();
    private GlyphManager _glyphManager;
    private TaskCompletionSource<bool>? _readyEvent = null;
    private EPhoneType _phoneType = EPhoneType.Unknown;
    private string _codename = string.Empty;
    private Type? _activeZoneMappingType;

    public GlyphAPI()
    {
        if (Application.Context.CheckSelfPermission("com.nothing.ketchum.permission.ENABLE") != Permission.Granted)
            throw new PermissionException("Application does not have permission to access the Ketchum (Glyph) API.");

        _readyEvent = new();

        _glyphManager = GlyphManager.GetInstance(Application.Context) ?? throw new InvalidOperationException("Failed to connect to Glyph API.");
        _glyphManager.Init(this);
    }

    ~GlyphAPI()
    {
        _glyphManager.UnInit();
    }

    async Task<bool> IGlyphAPI.WaitForReadyAsync(CancellationToken cancellationToken)
    {
        if (_readyEvent is not TaskCompletionSource<bool> tcs)
            return false;

        try { await tcs.Task.WaitAsync(cancellationToken); }
        catch (TaskCanceledException) { return false; }

        return tcs.Task.IsCompletedSuccessfully && tcs.Task.Result;
    }

    void GlyphManager.ICallback.OnServiceConnected(ComponentName? p0)
    {
        try
        {
            if (Common.Is20111())
            {
                _phoneType = EPhoneType.PhoneOne;
                _codename = KetchumGlyph.Device20111!;
            }
            else if (Common.Is22111())
            {
                _phoneType = EPhoneType.PhoneTwo;
                _codename = KetchumGlyph.Device22111!;
                //Device22111i Secret/unreleased model?
            }
            else if (Common.Is23111())
            {
                _phoneType = EPhoneType.PhoneTwoA;
                _codename = KetchumGlyph.Device23111!;
            }
            else if (Common.Is23113())
            {
                _phoneType = EPhoneType.PhoneTwoAPlus;
                _codename = KetchumGlyph.Device23113!;
            }
            else if (Common.Is24111())
            {
                _phoneType = EPhoneType.PhoneThreeA;
                _codename = KetchumGlyph.Device24111!;
            }
            else
                throw new IndexOutOfRangeException("Unknown device type.");

            _activeZoneMappingType = Zones.ZoneMapper.GetZoneTypeForDevice(_phoneType);

            //The docs would make it seem like the string to pass is "DEVICE_<ID>" but this is not true and rather the device codename is retrived by using the constant DEVICE_<ID>.
            //TODO: Tidy up this mess.
            if (!_glyphManager.Register(_codename))
            {
                //Attempt to get a reason for failing.
                if (Application.Context is not null && Application.Context.PackageManager is not null && Application.Context.PackageName is not null)
                {
                    ApplicationInfo appInfo = Application.Context.PackageManager.GetApplicationInfo(
                        Application.Context.PackageName,
                        PackageInfoFlags.MetaData
                    );

                    string? apiKey = appInfo.MetaData?.GetString("NothingKey");

                    if (apiKey is null)
                    {
                        throw new ArgumentNullException("No API key specified in app manifest.");
                    }
                    else if (apiKey.ToLower() == "test")
                    {
                        //App is in testing mode, make sure glyph debugging is enabled.
                        if (Settings.Global.GetInt(Application.Context.ContentResolver, "nt_glyph_interface_debug_enable", -1) != 1)
                        {
                            //Check if we have permission to enable this setting.
                            if (Application.Context.CheckSelfPermission("android.permission.WRITE_SECURE_SETTINGS") == Permission.Granted)
                            {
                                Settings.Global.PutInt(Application.Context.ContentResolver, "nt_glyph_interface_debug_enable", 1);

                                //Try to register again.
                                if (!_glyphManager.Register(_codename))
                                {
                                    //Unknown error.
                                    throw new InvalidOperationException("Failed to register device.");
                                }
                            }
                            else
                            {
                                //We don't have permission to enable testing mode.
                                throw new PermissionException("Application is in testing mode and Glyph debugging has not been enabled.");
                            }
                        }
                    }
                }
                else
                {
                    //Unknown error.
                    throw new InvalidOperationException("Failed to register device.");
                }
            }
        }
        catch
        {
            //Shouldn't ever be null here but checking anyway.
            if (_readyEvent is not TaskCompletionSource<bool>)
                _readyEvent = new();

            _readyEvent.SetResult(false);
            
            throw; //Rethrow the error.
        }

        _glyphManager.OpenSession();

        lock (_STATE_LOCK)
        {
            //Shouldn't ever be null here but checking anyway.
            if (_readyEvent is not TaskCompletionSource<bool>)
                _readyEvent = new();

            _readyEvent.SetResult(true);
        }
    }

    void GlyphManager.ICallback.OnServiceDisconnected(ComponentName? p0)
    {
        lock (_STATE_LOCK)
        {
            if (_readyEvent is TaskCompletionSource<bool>)
                _readyEvent.TrySetResult(false);

            _readyEvent = new();
        }

        //Auto restart.
        _glyphManager.Init(this);
    }

    private void PreCheck<TZone>() where TZone : struct, Enum
    {
        if (!((IGlyphAPI)this).IsReady)
            throw new InvalidOperationException("The API is not in a ready state.");

        if (typeof(TZone) != _activeZoneMappingType)
            throw new ArgumentException("Unsupported zone mapping type for this device.");
    }

    void IGlyphAPI.DrawFrame<TZone>(IReadOnlyDictionary<TZone, double> source)
    {
        PreCheck<TZone>();

        int[] ledArray = Zones.ZoneMapper.ZoneToArray(source);

        _glyphManager.SetFrameColors(ledArray);
    }

    //TODO: Fix this not working?
    void IGlyphAPI.SetZone<TZone>(TZone zone, double brightness)
    {
        PreCheck<TZone>();

        int zoneId = Zones.ZoneMapper.ZoneToID(zone);
        int externalBrightness = GlyphHelpers.InternalToExternalBrightness(brightness);

        GlyphFrame frame = _glyphManager.GlyphFrameBuilder!
            .BuildChannel(zoneId, 255)!
            .Build()!;

        _glyphManager.Toggle(frame);
    }
}
