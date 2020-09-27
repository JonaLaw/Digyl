/*using System;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.Media;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;

namespace Digyl.Droid
{
    [Service]
    public class DataSource : Service
    {
        const int SERVICE_RUNNING_NOTIFICATION_ID = 9000;
        const string NOTIFICATION_CHANNEL_ID = "com.company.app.channel";

        private static string foregroundChannelId = "9001";
        private static Context context = global::Android.App.Application.Context;

        private Notification ReturnNotification()
        {
            // Building intent
            var intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);
            intent.PutExtra("Title", "Message");

            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            var notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
                .SetContentTitle("Your Title")
                .SetContentText("Main Text Body")
                .SetSmallIcon(Resource.Drawable.notification_icon_background)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                .SetOngoing(true)
                .SetContentIntent(pendingIntent);

            // Building channel if API verion is 26 or above
            if (global::Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel notificationChannel = new NotificationChannel(foregroundChannelId, "Title", NotificationImportance.High);
                notificationChannel.Description = "Description";
                notificationChannel.Importance = NotificationImportance.High;
                notificationChannel.EnableLights(true);
                notificationChannel.SetShowBadge(true);
                notificationChannel.EnableVibration(true);
                notificationChannel.SetVibrationPattern(new long[] { 200, 200 });

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                //var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
                if (notificationManager != null)
                {
                    notifBuilder.SetChannelId(foregroundChannelId);
                    notificationManager.CreateNotificationChannel(notificationChannel); ;
                }
            }

            return notifBuilder.Build();
        }


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Notification notif = ReturnNotification();
            StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notif);

            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override bool StopService(Intent name)
        {
            return base.StopService(name);
        }
    }
}*/