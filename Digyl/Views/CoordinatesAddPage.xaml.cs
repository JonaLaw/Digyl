using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Diagnostics;

namespace Digyl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoordinatesAddPage : ContentPage
    {
        private readonly List<string> pickerList;
        private CoordinateItem editingItem;

        public CoordinatesAddPage(CoordinateItem itemToEdit = null)
        {
            editingItem = itemToEdit;
            pickerList = new List<string>();
            pickerList.Add(Constants.washHands);
            pickerList.Add(Constants.wearMask);
            pickerList.Add(Constants.washHandsAndWearMask);
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            onEnterPicker.ItemsSource = pickerList;
            onExitPicker.ItemsSource = pickerList;

            if (editingItem != null)
            {
                Title = "Editing Coordinate";

                nameEntry.Text = editingItem.Name;
                latitudeEntry.Text = editingItem.Latitude.ToString();
                longitudeEntry.Text = editingItem.Longitude.ToString();
                radiusEntry.Text = editingItem.Radius.ToString();
                //secondsEntry.Text = editingItem.Seconds.ToString();
                onEnterPicker.SelectedItem = editingItem.OnEnterReminder;
                onExitPicker.SelectedItem = editingItem.OnExitReminder;

                deleteButton.IsVisible = true;
                addButton.Text = "Save";
            }
            
            base.OnAppearing();
        }

        async void OnGetCurrentLocationButtonClicked(object sender, EventArgs e)
        {
            UpdateGettingLocation(true);

            Location currentLocation = await LocationDetails.GetCurrentLocation();
            if (currentLocation != null)
            {
                latitudeEntry.Text = currentLocation.Latitude.ToString();
                longitudeEntry.Text = currentLocation.Longitude.ToString();
            }

            UpdateGettingLocation(false);
        }

        async void OnSearchLocationButtonClicked(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Location Search", "Enter an address.");

            if (!String.IsNullOrEmpty(result))
            {
                UpdateGettingLocation(true);

                try
                {
                    IEnumerable<Location> locations = await Geocoding.GetLocationsAsync(result);

                    Location location = locations?.FirstOrDefault();
                    if (location != null)
                    {
                        latitudeEntry.Text = location.Latitude.ToString();
                        longitudeEntry.Text = location.Longitude.ToString();
                    }
                    else
                    {
                        await DisplayAlert("Geolocation Search Request Failure", $"\"{result}\" could not be found.", "OK");
                    }
                }
                catch (FeatureNotSupportedException fnsEx)
                {
                    // Feature not supported on device
                    Debug.WriteLine($"Location searching is not supported on this device: {fnsEx}.");
                    await DisplayAlert("Geolocation Search Request Failure", "Your device does not have this capability.", "OK");
                }
                catch (Exception ex)
                {
                    // Handle exception that may have occurred in geocoding
                    Debug.WriteLine($"Failed to find the location unexpectedly: {ex}.");
                    await DisplayAlert("Geolocation Search Request Failure", "Something unexpected happened.", "OK");
                }

                UpdateGettingLocation(false);
            }
        }

        private void UpdateGettingLocation(bool isRunning)
        {
            getLocationActivityIndicator.IsVisible = isRunning;
            getLocationActivityIndicator.IsRunning = isRunning;

            getLocationCurrentButton.IsVisible = !isRunning;
            getLocationSearchButton.IsVisible = !isRunning;
        }

        async void OnViewInMapButtonClicked(object sender, EventArgs e)
        {
            if (!await LocationDetails.ValidateCoordinates(latitudeEntry.Text, longitudeEntry.Text)) return;

            Location location = new Location(double.Parse(latitudeEntry.Text), double.Parse(longitudeEntry.Text));
            MapLaunchOptions options = new MapLaunchOptions { Name = nameEntry.Text };
            
            try
            {
                await Map.OpenAsync(location, options);
            }
            catch
            {
                // No map application available to open
                await DisplayAlert("View On Map Failure", "There is no map application available to open.", "OK");
            }
        }

        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (editingItem == null)
            {
                throw new System.ArgumentException("Parameter found to be null", "editingItem");
            }
            else
            {
                bool answer = await DisplayAlert("Delete Coordinate?", "Are you sure that you want to permanently delete this coordinate", "Yes", "No");

                if (answer)
                {
                    await App.Database.DeleteCoordinateAsync(editingItem);
                }
                else
                {
                    return;
                }
            }

            await Navigation.PopAsync();
        }

        async void OnAddButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameEntry.Text))
            {
                await DisplayAlert("Incomplete Name Entry", "A name entry is required.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(latitudeEntry.Text) || string.IsNullOrWhiteSpace(longitudeEntry.Text))
            {
                await DisplayAlert("Incomplete Latitude or Longitude Entry", "Both a Latitude and Longitude entry is required.", "OK");
                return;
            }

            if (!await LocationDetails.ValidateCoordinates(latitudeEntry.Text, longitudeEntry.Text)) return;

            // radius
            if (string.IsNullOrWhiteSpace(radiusEntry.Text))
            {
                await DisplayAlert("Incomplete Radius Entry", "A radius entry is required.", "OK");
                return;
            }

            try
            {
                int.Parse(radiusEntry.Text);
            }
            catch
            {
                await DisplayAlert("Invalid Radius Entry", "The radius entry must be a whole number.", "OK");
                return;
            }

            //seconds
            /*if (string.IsNullOrWhiteSpace(secondsEntry.Text))
            {
                await DisplayAlert("Incomplete Seconds Entry", "A seconds entry is required.", "OK");
                return;
            }

            try
            {
                int.Parse(secondsEntry.Text);
            }
            catch
            {
                await DisplayAlert("Invalid Seconds Entry", "The seconds entry must be a whole number.", "OK");
                return;
            }*/

            // reminders
            if (onEnterPicker.SelectedIndex == -1 && onExitPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Incomplete Reminders", "At least one reminder needs to be chosen.", "OK");
                return;
            }

            string onEnterReminder = "";
            if (onEnterPicker.SelectedIndex != -1) onEnterReminder = onEnterPicker.SelectedItem.ToString();

            string onExitReminder = "";
            if (onExitPicker.SelectedIndex != -1) onExitReminder = onExitPicker.SelectedItem.ToString();

            try
            {
                if (editingItem != null)
                {
                    editingItem.Name = nameEntry.Text;
                    editingItem.Latitude = double.Parse(latitudeEntry.Text);
                    editingItem.Longitude = double.Parse(longitudeEntry.Text);
                    editingItem.Radius = int.Parse(radiusEntry.Text);
                    //editingItem.Seconds = int.Parse(secondsEntry.Text);
                    editingItem.OnEnter = onEnterPicker.SelectedIndex != -1;
                    editingItem.OnExit = onExitPicker.SelectedIndex != -1;
                    editingItem.OnEnterReminder = onEnterReminder;
                    editingItem.OnExitReminder = onExitReminder;
                }
                else
                {
                    editingItem = new CoordinateItem
                    {
                        Name = nameEntry.Text,
                        Latitude = double.Parse(latitudeEntry.Text),
                        Longitude = double.Parse(longitudeEntry.Text),
                        Radius = int.Parse(radiusEntry.Text),
                        //Seconds = int.Parse(secondsEntry.Text),
                        Seconds = 0,
                        OnEnter = onEnterPicker.SelectedIndex != -1,
                        OnExit = onExitPicker.SelectedIndex != -1,
                        OnEnterReminder = onEnterReminder,
                        OnExitReminder = onExitReminder,
                        IsOn = false
                    };
                }
            }
            catch
            {
                await DisplayAlert("Invalid Entry", "An entry field contains invalid characters.", "OK");
                return;
            }
                
            await App.Database.SaveCoordinateAsync(editingItem);
            await Navigation.PopAsync();
        }
    }
}