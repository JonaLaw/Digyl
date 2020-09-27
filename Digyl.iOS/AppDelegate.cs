using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace Digyl.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            /*this.ShinyFinishedLaunching(new MyStartup(), platformBuild: services =>
            {
                services.UseNotifications();
            });*/

            // and add this guy - if you don't use jobs, you won't need it
            /*public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
                => JobManager.OnBackgroundFetch(completionHandler);
            */
            global::Xamarin.Forms.Forms.SetFlags("Expander_Experimental");
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            /*if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Ask the user for permission to get notifications on iOS 10.0+
                UNUserNotificationCenter.Current.RequestAuthorization(
                        UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                        (approved, error) => { });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // Ask the user for permission to get notifications on iOS 8.0+
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                        new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }*/

            return base.FinishedLaunching(app, options);
        }

        /*public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            this.ShinyPerformFetch(completionHandler);
        }*/

        /*public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, Action completionHandler)
        {
            this.ShinyHandleEventsForBackgroundUrl(sessionIdentifier, completionHandler);
        }*/
    }
}
