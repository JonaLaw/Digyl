using Android;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Util;
using Digyl.Droid.Services;
using System;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(Digyl.Droid.AndroidServiceHelper))]

namespace Digyl.Droid
{
    // https://github.com/xamarin/mobile-samples/blob/master/BackgroundLocationDemo/location.Android/MainActivity.cs
    class AndroidServiceHelper : ILocationService
    {
        private readonly LocationServiceConnection locationServiceConnection;
        private static Context context;
        private bool isOn;

        protected readonly string logTag = "AndroidServiceHelper";

        public AndroidServiceHelper()
        {
            // create a new service connection so we can get a binder to the service
            locationServiceConnection = new LocationServiceConnection(null);
            locationServiceConnection.ServiceConnected += (sender, e) =>
            {
                Log.Debug(logTag, "Service Connected");
                // we will use this event to notify MainActivity when to start updating the UI
                LocationServiceConnected(this, e);
            };

            context = global::Android.App.Application.Context;
            isOn = false;
        }

        public LocationService LocationService
        {
            get
            {
                if (locationServiceConnection.Binder == null)
                {
                    throw new Exception("Service not bound yet");
                }

                // note that we use the ServiceConnection to get the Binder, and the Binder to get the Service here
                return locationServiceConnection.Binder.Service;
            }
        }

        // events
        public event EventHandler<ServiceConnectedEventArgs> LocationServiceConnected = delegate { };

        public bool IsLocationServiceOn()
        {
            return isOn;
        }

        public void StartLocationService()
        {
            if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
            {
                Log.Debug(logTag, "User already has granted permission.");
            }
            else
            {
                Log.Debug(logTag, "Have to request permission from the user. ");
                return;
            }

            new Task(() =>
            {
                // Start our main service
                Log.Debug(logTag, "Calling StartService");

                Intent intent = new Intent(context, typeof(LocationService));

                // Check if device is running Android 8.0 or higher and if so, use the newer StartForegroundService() method
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    context.StartForegroundService(intent);
                }
                else // For older versions, use the traditional StartService() method
                {
                    context.StartService(intent);
                }

                // bind our service (Android goes and finds the running service by type, and puts a reference
                // on the binder to that service)
                // The Intent tells the OS where to find our Service (the Context) and the Type of Service
                // we're looking for (LocationService)
                Intent locationServiceIntent = new Intent(context, typeof(LocationService));
                Log.Debug(logTag, "Calling service binding");

                // Finally, we can bind to the Service using our Intent and the ServiceConnection we
                // created in a previous step.
                context.BindService(locationServiceIntent, locationServiceConnection, Bind.AutoCreate);

                isOn = true;
            }).Start();
        }

        public void StopLocationService()
        {
            // Check for nulls in case StartLocationService task has not yet completed.
            Log.Debug(logTag, "StopLocationService");

            // Unbind from the LocationService; otherwise, StopSelf (below) will not work:
            if (locationServiceConnection != null)
            {
                Log.Debug(logTag, "Unbinding from LocationService");
                context.UnbindService(locationServiceConnection);
            }

            // Stop the LocationService:
            if (LocationService != null)
            {
                Log.Debug(logTag, "Stopping the LocationService");
                LocationService.StopSelf();
            }

            //var intent = new Intent(context, typeof(LocationService));
            //context.StopService(intent);

            isOn = false;
        }

        public void UpdateNotification(string title, string text, int id)
        {
            LocationService.UpdateNotification(title, text, id);
        }
    }
}