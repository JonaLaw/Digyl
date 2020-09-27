using System;
using System.Threading.Tasks;

using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;

using Digyl.Droid.Services;

using Xamarin.Forms;

//[assembly: Dependency(typeof(Digyl.Droid.AndroidServiceHelper))]
/*
namespace Digyl.Droid
{
    public class AndroidServiceHelper : ILocationService
    {
        protected static LocationServiceConnection locationServiceConnection;
        private static Context context = global::Android.App.Application.Context;

        // declarations
        protected readonly string logTag = "App";

        public AndroidServiceHelper()
        {
            // create a new service connection so we can get a binder to the service
            locationServiceConnection = new LocationServiceConnection(null);

            // this event will fire when the Service connectin in the OnServiceConnected call 
            locationServiceConnection.ServiceConnected += (sender, e) =>
            {
                Log.Debug(logTag, "Service Connected");
                // we will use this event to notify MainActivity when to start updating the UI
                //LocationServiceConnected(this, e);
            };
        }

        /*static AndroidServiceHelper()
        {
            Current = new AndroidServiceHelper();
        }

        // properties

        public static AndroidServiceHelper Current { get; }

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
            if (locationServiceConnection != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void StartLocationService()
        {
            new Task(() =>
            {
                // Start our main service
                Log.Debug("App", "Calling StartService");

                var intent = new Intent(context, typeof(LocationService));

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
                var locationServiceIntent = intent;
                Log.Debug("App", "Calling service binding");

                // Finally, we can bind to the Service using our Intent and the ServiceConnection we
                // created in a previous step.
                context.BindService(locationServiceIntent, locationServiceConnection, Bind.AutoCreate);
            }).Start();
        }

        public void StopLocationService()
        {
            /*var intent = new Intent(context, typeof(Service));
            context.StopService(intent);

            // Check for nulls in case StartLocationService task has not yet completed.
            Log.Debug("App", "StopLocationService");

            // Unbind from the LocationService; otherwise, StopSelf (below) will not work:
            if (locationServiceConnection != null)
            {
                Log.Debug("App", "Unbinding from LocationService");
                context.UnbindService(locationServiceConnection);
            }

            // Stop the LocationService:
           /* if (Current.LocationService != null)
            {
                Log.Debug("App", "Stopping the LocationService");
                Current.LocationService.StopSelf();
            }
        }
    }
}
*/