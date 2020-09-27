using System;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;


namespace Digyl.Droid.Services
{
    // https://github.com/xamarin/mobile-samples/blob/master/BackgroundLocationDemo/location.Android/Services/LocationService.cs
    [Service]
    public class LocationService : Service, ILocationListener
    {
        private static readonly Context context = global::Android.App.Application.Context;
        //private readonly LocationUpdateHandler locationUpdateHandler = new LocationUpdateHandler();

        private readonly string logTag = "LocationService";
        private IBinder binder;

        // Set our location manager as the system location service
        protected LocationManager locationManager = Application.Context.GetSystemService("location") as LocationManager;

        // ILocationListener is a way for the Service to subscribe for updates
        // from the System location Service

        public void OnLocationChanged(Android.Locations.Location location)
        {
            LocationChanged(this, new LocationChangedEventArgs(location));
            //_ = App.LocationUpdateHandler.UpdateLocation(location.Latitude, location.Longitude);
            _ = App.LocationUpdateHandler.UpdateLocation(location.Latitude, location.Longitude);

            // This should be updating every time we request new location updates
            // both when teh app is in the background, and in the foreground
            /*Log.Debug(logTag, $"Latitude is {location.Latitude}");
            Log.Debug(logTag, $"Longitude is {location.Longitude}");
            Log.Debug(logTag, $"Altitude is {location.Altitude}");
            Log.Debug(logTag, $"Speed is {location.Speed}");
            Log.Debug(logTag, $"Accuracy is {location.Accuracy}");
            Log.Debug(logTag, $"Bearing is {location.Bearing}");*/
        }

        public void OnProviderDisabled(string provider)
        {
            Log.Debug(logTag, $"Location provider {provider} disabled.");
            ProviderDisabled(this, new ProviderDisabledEventArgs(provider));
        }

        public void OnProviderEnabled(string provider)
        {
            Log.Debug(logTag, $"Location provider {provider} enabled.");
            ProviderEnabled(this, new ProviderEnabledEventArgs(provider));
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Debug(logTag, $"Location provider {provider}, status change: {status}.");
            StatusChanged(this, new StatusChangedEventArgs(provider, status, extras));
        }

        public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };
        public event EventHandler<ProviderDisabledEventArgs> ProviderDisabled = delegate { };
        public event EventHandler<ProviderEnabledEventArgs> ProviderEnabled = delegate { };
        public event EventHandler<StatusChangedEventArgs> StatusChanged = delegate { };

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(logTag, "OnCreate called in the Location Service");
        }

        // This gets called when StartService is called in our App class
        [Obsolete("deprecated in base class")]
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug(logTag, "LocationService started");

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                NotificationManager notificationManager = GetSystemService(NotificationService) as NotificationManager;
                notificationManager.CreateNotificationChannel(CreateCustomNotificationChannel());
            }
            //NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this, Constants.NOTIFICATION_CHANNEL_ID);
            Notification notification = GetNotification();
            StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);

            //StartLocationUpdates();
            return StartCommandResult.Sticky;
        }

        private NotificationChannel CreateCustomNotificationChannel()
        {
            Log.Debug(logTag, "Creating notification channel.");

            NotificationChannel notifChannel = new NotificationChannel(Constants.NOTIFICATION_CHANNEL_ID, "My Notification Service", NotificationImportance.High);
            notifChannel.Description = "Description";
            notifChannel.LockscreenVisibility = NotificationVisibility.Public;
            notifChannel.EnableLights(true);
            notifChannel.SetShowBadge(true);
            notifChannel.EnableVibration(true);
            //notifChannel.SetVibrationPattern(new long[] { 100, 100 });

            return notifChannel;
        }

        public void UpdateNotification(string title, string text, int id)
        {
            Log.Debug(logTag, "Updating notification.");
            NotificationManager notificationManager = GetSystemService(NotificationService) as NotificationManager;

            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                notificationManager.CreateNotificationChannel(CreateCustomNotificationChannel());
            }

            Notification notification = GetNotification(title, text, id);

            notificationManager.Notify(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        private Notification GetNotification(string title = null, string text = null, int id = 0)
        {
            if (title == null)
            {
                Log.Debug(logTag, "Getting normal notification.");

                PendingIntent pIntent = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(LocationService)), 0);

                return new NotificationCompat.Builder(context, Constants.NOTIFICATION_CHANNEL_ID)
                    .SetColor(Android.Resource.Color.HoloBlueLight)
                    .SetContentTitle("Digyl Location Tracking")
                    .SetContentText("The app can be closed whenever.")
                    .SetSmallIcon(Resource.Drawable.notification_icon_background)
                    .SetOngoing(true)
                    //.SetAutoCancel(true)
                    .SetContentIntent(pIntent)
                    //.SetVibrate(new long[] { 100 })
                    .Build();
            }
            else if (id == 0)
            {
                Log.Debug(logTag, "Getting semi-normal notification.");

                PendingIntent pIntent = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(LocationService)), 0);

                return new NotificationCompat.Builder(context, Constants.NOTIFICATION_CHANNEL_ID)
                    .SetColor(Android.Resource.Color.HoloBlueLight)
                    .SetContentTitle(title)
                    .SetContentText(text)
                    .SetSmallIcon(Resource.Drawable.notification_icon_background)
                    .SetOngoing(true)
                    //.SetAutoCancel(true)
                    .SetContentIntent(pIntent)
                    .Build();
            }
            else
            {
                Log.Debug(logTag, "Getting custom notification.");

                PendingIntent pIntent = PendingIntent.GetActivity(context, 0, new Intent(context, typeof(LocationService)), 0);

                Intent broadcastIntentYes = new Intent(context, typeof(NotificationActionYesReceiver));
                broadcastIntentYes.PutExtra(Constants.broadcastIntentExtraYes, id);
                PendingIntent actionIntentYes = PendingIntent.GetBroadcast(context, 0, broadcastIntentYes, PendingIntentFlags.UpdateCurrent);

                Intent broadcastIntentNo = new Intent(context, typeof(NotificationActionNoReceiver));
                broadcastIntentNo.PutExtra(Constants.broadcastIntentExtraNo, id);
                PendingIntent actionIntentNo = PendingIntent.GetBroadcast(context, 0, broadcastIntentNo, PendingIntentFlags.UpdateCurrent);

                return new NotificationCompat.Builder(context, Constants.NOTIFICATION_CHANNEL_ID)
                    .SetColor(Android.Resource.Color.HoloBlueLight)
                    .SetContentTitle(title)
                    .SetContentText(text)
                    .SetSmallIcon(Resource.Drawable.notification_icon_background)
                    //.SetColor(Color.LightYellow)
                    .SetOngoing(true)
                    .SetContentIntent(pIntent) 
                    .AddAction(Resource.Drawable.arrow, "Yes", actionIntentYes)
                    .AddAction(Resource.Drawable.arrow, "No", actionIntentNo)
                    .SetVibrate(new long[] { 100, 100, 100 })
                    .Build();
            }
            
        }

        // This gets called once, the first time any client bind to the Service
        // and returns an instance of the LocationServiceBinder. All future clients will
        // reuse the same instance of the binder
        public override IBinder OnBind(Intent intent)
        {
            Log.Debug(logTag, "Client now bound to service");

            binder = new LocationServiceBinder(this);
            return binder;
        }

        // Handle location updates from the location manager
        public void StartLocationUpdates()
        {
            //we can set different location criteria based on requirements for our app -
            //for example, we might want to preserve power, or get extreme accuracy
            Criteria locationCriteria = new Criteria();

            // https://stackoverflow.com/questions/17874454/which-is-a-higher-accuracy-criteria-accuracy-high-or-accuracy-fine
            locationCriteria.Accuracy = Accuracy.Fine;
            locationCriteria.PowerRequirement = Power.NoRequirement;

            // get provider: GPS, Network, etc.
            /*string locationProvider = locationManager.GetBestProvider(locationCriteria, true);
            if (locationProvider == null)
            {
                Log.Debug(logTag, "Could not find a location provider.");
            }*/

            string locationProvider = "gps";
            Log.Debug(logTag, string.Format("You are about to get location updates via {0}", locationProvider));

            // Get an initial fix on location
            // every x milliseconds, location change every x meter
            locationManager.RequestLocationUpdates(locationProvider, Constants.locationUpdateMinTime, Constants.locationUpdateDistance, this);

            Log.Debug(logTag, "Now sending location updates");
        }

        public override void OnDestroy()
        {
            binder = null;

            // Stop getting updates from the location manager:
            locationManager.RemoveUpdates(this);

            base.OnDestroy();
            Log.Debug(logTag, "Service has been terminated");
        }
    }
}