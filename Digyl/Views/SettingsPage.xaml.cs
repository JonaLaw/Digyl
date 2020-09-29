using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Digyl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private bool _initial;

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            _initial = true;

            base.OnAppearing();
            manualHistorySwitch.IsToggled = App.Settings.ManualHistoryEnabled;

            await Task.Delay(200);
            _initial = false;
        }


        void OnManualHistoryToggle(object sender, ToggledEventArgs e)
        {
            if (_initial) return;

            App.Settings.ManualHistoryEnabled = e.Value;
        }

        private async void OnPlaceTypeDetectionSearchButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Testing place type search detection.");
            Location location = await LocationDetails.GetLocationFromUser();
            if (location == null) return;
            await PlaceTypeDetection(location);
        }

        private async void OnPlaceTypeDetectionCurrentButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Testing place type current detection.");
            Location location = await LocationDetails.GetCurrentLocation();
            if (location == null) return;
            await PlaceTypeDetection(location);
        }

        private async Task PlaceTypeDetection(Location location)
        {
            PlaceTypeItem foundPTI = await LocationDetails.GetNearbyPlaceTypeItem(location);

            if (foundPTI == null)
            {
                await DisplayAlert("Place Type Detection Results", $"No place types were found near ({location.Latitude}, {location.Longitude}).", "OK");
            }
            else
            {
                await DisplayAlert("Place Type Detection Results", $"The place type named \"{foundPTI.Name}\" was found near ({location.Latitude}, {location.Longitude}).", "OK");
            }
        }

        private async void OnCoordinateDetectionSearchButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Testing coordinate search detection.");
            Location location = await LocationDetails.GetLocationFromUser();
            if (location == null) return;
            await CoordinateDetection(location);
        }

        private async void OnCoordinateDetectionCurrentButtonClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Testing coordinate current detection.");
            Location location = await LocationDetails.GetCurrentLocation();
            if (location == null) return;
            await CoordinateDetection(location);
        }

        private async Task CoordinateDetection(Location location)
        {
            CoordinateItem foundCI = await App.Database.QueryCoordinatesAsync(location);
            if (foundCI == null)
            {
                await DisplayAlert("Coordinate Detection Results", $"No coordinates were found near ({location.Latitude}, {location.Longitude}).", "OK");
            }
            else
            {
                await DisplayAlert("Coordinate Detection Results", $"The coordinate named \"{foundCI.Name}\" was found near ({location.Latitude}, {location.Longitude}).", "OK");
            }
        }

        /*private async void LocationServiceButtonClicked(object sender, EventArgs e)
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationAlways>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Location Always Permision Denied", "This App requires access to your location at all times to function properly.", "OK");
                    return;
                }
            }
            
            ILocationService locationDependencyService = DependencyService.Get<ILocationService>();

            if (locationDependencyService.IsLocationServiceOn())
            {
                locationDependencyService.StopLocationService();
            }
            else
            {
                locationDependencyService.StartLocationService();
            }
        }*/

        private async void OnClearDataBasesButtonClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Clear the databases?", "Would you like to clear the databases", "Yes", "No");
            if (answer)
                await App.Database.ClearDataBases();
        }

        private async void OnEditLocationMinTime(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Editing Location Update MinTime", "Enter a time interval in milliseconds that you want to receive location updates.\nChanging this from the default will mostly just drain the battery at a much higher rate." +
                "Default: 0 mil, to only use Location Update MinDistance for triggering updates.", placeholder: App.Settings.LocationUpdateMinTime.ToString(), maxLength: 10, keyboard: Keyboard.Numeric);

            if (result == null || result.Equals("Cancel")) return;

            int value = await ParseEntry(result);
            if (value == -1) return;

            App.Settings.LocationUpdateMinTime = value;
            await DisplayAlert("Restart Location Tracking", "If location tracking is on, restart it for your changes to have an effect.", "OK");
        }

        private async void OnEditLocationMinDistance(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Editing Location Update MinDistance", "Enter a distance interval in meters that you want to receive location updates.\nLower equals more accurate detction but requires confidence in GPS accuracy." +
                "Default: 10 meters, Smartphone GPS accuracy is about this good.", placeholder: App.Settings.LocationUpdateDistance.ToString(), maxLength: 5, keyboard: Keyboard.Numeric);

            if (result == null || result.Equals("Cancel")) return;

            int value = await ParseEntry(result);
            if (value == -1) return;

            App.Settings.LocationUpdateDistance = value;
            await DisplayAlert("Restart Location Tracking", "If location tracking is on, restart it for your changes to have an effect.", "OK");
        }

        private async void OnEditPlaceTypeRadius(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Editing Place Type Radius", $"Enter a radius for searching around your current location to detect place types.\nLower equals more accurate PT detction but requires confidence in GPS accuracy.\n" +
                "Default: 10 meters, so at least some results are shown, even if they are off.", placeholder: App.Settings.PlaceTypeRadius.ToString(), maxLength: 5, keyboard: Keyboard.Numeric);

            if (result == null || result.Equals("Cancel")) return;

            int value = await ParseEntry(result);
            if (value == -1) return;

            App.Settings.PlaceTypeRadius = value;
        }

        private async void OnEditPlaceTypeLeaveRadius(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Editing Place Type Leave Radius", $"Enter a radius for trying to leave a place type. If the value of Location Update MinDistance is lower than this then the former takes over.\nLower equals more accurate PT detction exiting detection but requires more Google Place API requests.\n" +
                "Default: 10 meters, to match other defaults.", placeholder: App.Settings.PlaceTypeLeaveRadius.ToString(), maxLength:5, keyboard: Keyboard.Numeric);

            if (result == null || result.Equals("Cancel")) return;

            int value = await ParseEntry(result);
            if (value == -1) return;

            App.Settings.PlaceTypeLeaveRadius = value;
        }

        private async Task<int> ParseEntry(string entry)
        {
            try
            {
                int value = int.Parse(entry);
                if (value >= 0)
                {
                    return value;
                }
            }
            catch { }

            await DisplayAlert("Invalid Entry", "The entry must be a positve integer", "OK");
            return -1;
        }
    }
}