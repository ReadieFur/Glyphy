using System;
using Console = System.Console;
using Java.Lang;
using Java.IO;
using JProcess = Java.Lang.Process;
using Microsoft.Maui.ApplicationModel;
using Android.Util;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using AndroidX.Core.App;
using System.Threading.Tasks;
using System.Threading;
using Android.Service.Notification;

namespace Glyphy.Platforms.Android
{
    internal static class Helpers
    {
        #region Notification Access
        public static bool HasNotificationAccess
        {
            get
            {
                if (Platform.AppContext.PackageName is null)
                    throw new NullReferenceException(nameof(Platform.AppContext.PackageName));
                return NotificationManagerCompat.GetEnabledListenerPackages(Platform.AppContext).Contains(Platform.AppContext.PackageName);
            }
        }

        public static async Task<bool> RequestNotificationAccess() => await RequestNotificationAccess(null);

        public static async Task<bool> RequestNotificationAccess(CancellationToken? cancellationToken = null)
        {
            const int NOTIFICATION_ACCESS_REQUEST_CODE = 1001;

            if (Platform.CurrentActivity is null)
                throw new NullReferenceException(nameof(Platform.CurrentActivity));

            if (HasNotificationAccess)
                return true;

            ManualResetEventSlim activityResultResetEvent = new(false);

            Intent intent = new("android.settings.ACTION_NOTIFICATION_LISTENER_SETTINGS");
            Platform.CurrentActivity.StartActivityForResult(intent, NOTIFICATION_ACCESS_REQUEST_CODE);

            MainActivity.OnActivityResultEvent += (requestCode, resultCode, data) =>
            {
                if (requestCode == NOTIFICATION_ACCESS_REQUEST_CODE)
                    activityResultResetEvent.Set();
            };

            await Task.Run(() =>
            {
                if (cancellationToken is null)
                    activityResultResetEvent.Wait();
                else
                    activityResultResetEvent.Wait(cancellationToken.Value);
            });

            return HasNotificationAccess;
        }
        #endregion

        #region System UI
        public static int StatusBarHeight => GetBarDimention("status_bar_height");

        public static int NavigationBarHeight => GetBarDimention("navigation_bar_height");

        private static int GetBarDimention(string resourceName)
        {
            int? statusBarHeight = 0;
            int? resourceId = Platform.CurrentActivity?.Resources?.GetIdentifier(resourceName, "dimen", "android");
            if (resourceId is not null && resourceId > 0)
                statusBarHeight = Platform.CurrentActivity?.Resources?.GetDimensionPixelSize(resourceId.Value);

            if (statusBarHeight is null)
                throw new NullReferenceException("Unable to get height.");

            DisplayMetrics? displayMetrics = Platform.CurrentActivity?.Resources?.DisplayMetrics;
            if (displayMetrics is null)
                throw new NullReferenceException("Unable to get display metrics.");

            return (int)(statusBarHeight.Value / displayMetrics.Density);
        }
        #endregion

        #region Root Process
        public static bool CreateRootSubProcess(out JProcess? subProcess)
        {
            subProcess = null;
            try
            {
                subProcess = Runtime.GetRuntime()?.Exec("su");
                if (subProcess is null)
                    throw new NullReferenceException("Unable to create sub-process.");

                DataOutputStream inputStream = new DataOutputStream(subProcess?.OutputStream);
                DataInputStream outputStream = new DataInputStream(subProcess?.InputStream);

                if (outputStream is null || inputStream is null)
                    throw new NullReferenceException("Unable to open sub-process streams.");

                inputStream.WriteBytes("id\n");
                inputStream.Flush();
#pragma warning disable CS0618 // Type or member is obsolete
                string? result = outputStream.ReadLine();
#pragma warning restore CS0618

                if (string.IsNullOrEmpty(result) || !result!.Contains("uid=0"))
                    throw new PermissionException("Root access denied.");

                outputStream.Dispose();
                inputStream.Dispose();

                return true;
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Root access rejected: " + e.Message);

                if (subProcess != null)
                    subProcess.Dispose();

                return false;
            }
        }
        #endregion

        #region Services
        public static void StartService<TService>(bool asForeground) where TService : Service
        {
            if (Platform.CurrentActivity is null)
                throw new NullReferenceException($"Unable to start service. {nameof(Platform.CurrentActivity)} is null.");

            Intent serviceIntent = new(Platform.AppContext, typeof(TService));

            if (asForeground && Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                serviceIntent.SetAction(nameof(Platform.AppContext.StartForegroundService));

                //Configure the notification channel to be silent and not show a badge.
                //The naming of the following is only temporary as it creates a channel per service.
                NotificationChannel channel = new NotificationChannel($"glyphy_{typeof(TService).Name}", $"Glyphy {typeof(TService).Name}", NotificationImportance.Min);
                channel.SetSound(null, null);
                channel.EnableVibration(false);
                channel.SetShowBadge(false);
                //channel.LockscreenVisibility = NotificationVisibility.Secret;

                //Create the notification channel.
                NotificationManager? notificationManager = Platform.CurrentActivity.GetSystemService(Context.NotificationService) as NotificationManager;
                if (notificationManager is null)
                    throw new NullReferenceException($"Unable to start service. {nameof(notificationManager)} is null.");
                notificationManager.CreateNotificationChannel(channel);

                //Start the service.
                Platform.AppContext.StartForegroundService(serviceIntent);
#pragma warning restore CA1416 // Validate platform compatibility
            }
            else
            {
                serviceIntent.SetAction(nameof(Platform.AppContext.StartService));

                Platform.AppContext.StartService(serviceIntent);
            }
        }

        public static void StopService<TService>(bool isForeground) where TService : Service
        {
            if (Platform.CurrentActivity is null)
                throw new NullReferenceException($"Unable to stop service. {nameof(Platform.CurrentActivity)} is null.");

            if (isForeground && Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationManager? notificationManager = Platform.CurrentActivity.GetSystemService(Context.NotificationService) as NotificationManager;
                if (notificationManager is null)
                    throw new NullReferenceException($"Unable to clear service. {nameof(notificationManager)} is null.");

                StatusBarNotification[]? activeNotifications = notificationManager.GetActiveNotifications();
                if (activeNotifications is null)
                    return;

                //TODO: Get the notification ID from the service to then cancel.
                //notificationManager.CancelAll();
                //Temporary workaround (I think).
                foreach (StatusBarNotification notification in activeNotifications)
                {
                    if (notification.PackageName != Platform.AppContext.PackageName
                        || (notification.Notification?.ChannelId ?? string.Empty) != $"glyphy_{typeof(TService).Name}")
                        continue;

                    notificationManager.Cancel(notification.Id);

                    break;
                }
            }

            Intent serviceIntent = new(Platform.AppContext, typeof(TService));
            serviceIntent.SetAction(nameof(Platform.AppContext.StopService));
            Platform.AppContext.StopService(serviceIntent);
        }

        public static void RegisterService<TService>(bool register = true) where TService : Service
        {
            if (Platform.CurrentActivity is null)
                throw new NullReferenceException("Unable to " + (register ? string.Empty : "un") + $"register service. {nameof(Platform.CurrentActivity)} is null.");

            if (Platform.CurrentActivity.PackageManager is null)
                throw new NullReferenceException("Unable to " + (register ? string.Empty : "un") + $"register service. {nameof(Platform.CurrentActivity.PackageManager)} is null.");

            Platform.CurrentActivity.PackageManager.SetComponentEnabledSetting(
                    new(Platform.AppContext, Class.FromType(typeof(TService))),
                    register ? ComponentEnabledState.Enabled : ComponentEnabledState.Disabled,
                    ComponentEnableOption.DontKillApp);
        }

        public static bool IsServiceRunning<TService>() where TService : Service
        {
            if (Platform.CurrentActivity is null)
                throw new NullReferenceException($"Unable to check if service is running. {nameof(Platform.CurrentActivity)} is null.");

            if (Platform.CurrentActivity.PackageManager is null)
                throw new NullReferenceException($"Unable to check if service is running. {nameof(Platform.CurrentActivity.PackageManager)} is null.");

            ComponentName componentName = new(Platform.AppContext, Class.FromType(typeof(TService)));
            ComponentName? serviceComponentName = Platform.CurrentActivity.PackageManager.GetServiceInfo(componentName, PackageInfoFlags.Services) is null ? null : componentName;

            if (serviceComponentName is null)
                throw new NullReferenceException($"Unable to check if service is running. {nameof(serviceComponentName)} is null.");

            ActivityManager? activityManager = Platform.CurrentActivity.GetSystemService(Context.ActivityService) as ActivityManager;
            if (activityManager is null)
                throw new NullReferenceException($"Unable to check if service is running. {nameof(activityManager)} is null.");

            foreach (ActivityManager.RunningServiceInfo service in activityManager.GetRunningServices(int.MaxValue))
                if (service.Service?.ClassName == serviceComponentName.ClassName && service.Service?.PackageName == serviceComponentName.PackageName)
                    return true;

            return false;
        }
        #endregion
    }
}
