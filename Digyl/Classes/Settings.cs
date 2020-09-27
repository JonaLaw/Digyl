using Xamarin.Essentials;

namespace Digyl
{
    public class Settings
    {
        /*private bool trackingEnabled = Preferences.Get(Constants.trackingPref, false);

        public bool TrackingEnabled
        {
            get { return trackingEnabled; }
            set
            {
                trackingEnabled = value;
                Preferences.Set(Constants.trackingPref, value);
            }
        }*/


        private bool coordinateTrackingEnabled = Preferences.Get(Constants.coordinateTrackingPref, false);
        public bool CoordinateTrackingEnabled
        {
            get { return coordinateTrackingEnabled; }
            set
            {
                coordinateTrackingEnabled = value;
                Preferences.Set(Constants.coordinateTrackingPref, value);
            }
        }

        private bool placeTrackingEnabled = Preferences.Get(Constants.placeTrackingPef, false);
        public bool PlaceTrackingEnabled
        {
            get { return placeTrackingEnabled; }
            set
            {
                placeTrackingEnabled = value;
                Preferences.Set(Constants.placeTrackingPef, value);
            }
        }

        private bool manualHistoryEnabled = Preferences.Get(Constants.manualHistoryPef, false);
        public bool ManualHistoryEnabled
        {
            get { return manualHistoryEnabled; }
            set
            {
                manualHistoryEnabled = value;
                Preferences.Set(Constants.manualHistoryPef, value);
                App.Current.Resources["historyButtonVisible"] = value;
            }
        }
    }
}
