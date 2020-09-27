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

        private async void PlaceTypeDetectionSearch(object sender, EventArgs e)
        {
            Debug.WriteLine("Testing place type search detection.");
            Location location = await LocationDetails.GetLocationFromUser();
            if (location == null) return;
            await PlaceTypeDetection(location);
        }

        private async void PlaceTypeDetectionCurrent(object sender, EventArgs e)
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

        private async void CoordinateDetectionSearch(object sender, EventArgs e)
        {
            Debug.WriteLine("Testing coordinate search detection.");
            Location location = await LocationDetails.GetLocationFromUser();
            if (location == null) return;
            await CoordinateDetection(location);
        }

        private async void CoordinateDetectionCurrent(object sender, EventArgs e)
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

        private async void ClearDataBases(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Clear the databases?", "Would you like to clear the databases", "Yes", "No");
            if (answer)
                await App.Database.ClearDataBases();
        }
    }
}