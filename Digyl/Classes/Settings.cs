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

        // milliseconds
        private int locationUpdateMinTime = Preferences.Get(Constants.locationUpdateMinTimePref, 0);
        public int LocationUpdateMinTime
        {
            get { return locationUpdateMinTime; }
            set
            {
                locationUpdateMinTime = value;
                Preferences.Set(Constants.locationUpdateMinTimePref, value);
            }
        }

        // meters
        private int locationUpdateDistance = Preferences.Get(Constants.locationUpdateDistancePref, 10);
        public int LocationUpdateDistance
        {
            get { return locationUpdateDistance; }
            set
            {
                locationUpdateDistance = value;
                Preferences.Set(Constants.locationUpdateDistancePref, value);
            }
        }

        // meters
        private int placeTypeRadius = Preferences.Get(Constants.placeTypeRadiusPef, 10);
        public int PlaceTypeRadius
        {
            get { return placeTypeRadius; }
            set
            {
                placeTypeRadius = value;
                Preferences.Set(Constants.placeTypeRadiusPef, value);
            }
        }

        // meters
        private int placeTypeLeaveRadius = Preferences.Get(Constants.placeTypeLeaveRadiusPef, 10);
        public int PlaceTypeLeaveRadius
        {
            get { return placeTypeLeaveRadius; }
            set
            {
                placeTypeLeaveRadius = value;
                Preferences.Set(Constants.placeTypeLeaveRadiusPef, value);
            }
        }
    }
}
