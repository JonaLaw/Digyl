using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Globalization;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Runtime.CompilerServices;

namespace Digyl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        ILocationService locationDependencyService;
        private HistoryItem latestHistoryItem;
        private HistoryItem latestNoResponseItem;

        public HomePage()
        {
            locationDependencyService = DependencyService.Get<ILocationService>();
            InitializeComponent();
            this.BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateLocationTrackingButtons();

            new Task(async () =>
            {
                await UpdateLastestHistory();
                await UpdateLastNoResponse();
            }).Start();

            _ = UpdateHistoryBreakdown();
        }

        private async Task UpdateLastestHistory()
        {
            latestHistoryItem = await App.Database.GetFirstHistoryItemAsync();

            if (latestHistoryItem == null)
            {
                latestHistoryNone.IsVisible = true;
                latestHistoryPresent.IsVisible = false;
            }
            else
            {
                latestHistoryNone.IsVisible = false;
                latestHistoryPresent.IsVisible = true;

                latestHistoryBox.BackgroundColor = latestHistoryItem.BoxColor;

                if (latestHistoryItem.Action.Equals(Constants.actionEnter))
                {
                    latestHistoryActionArrowImage.RotationY = 0;
                    latestHistoryActionImage.RotationY = 0;
                }
                else
                {
                    latestHistoryActionArrowImage.RotationY = 180;
                    latestHistoryActionImage.RotationY = 180;
                }

                if (latestHistoryItem.TrackedType.Equals(Constants.placeTypeName))
                {
                    latestHistoryName.Text = latestHistoryItem.TrackedType + ": " + latestHistoryItem.Name;
                }
                else
                {
                    latestHistoryName.Text = latestHistoryItem.Name;
                }

                if (latestHistoryItem.Reminder.Equals(Constants.washHands))
                {
                    latestHistoryReminderHandsImage.IsVisible = true;
                    latestHistoryReminderMaskImage.IsVisible = false;
                }
                else if (latestHistoryItem.Reminder.Equals(Constants.wearMask))
                {
                    latestHistoryReminderHandsImage.IsVisible = false;
                    latestHistoryReminderMaskImage.IsVisible = true;
                }
                else
                {
                    latestHistoryReminderHandsImage.IsVisible = true;
                    latestHistoryReminderMaskImage.IsVisible = true;
                }

                //latestHistoryResponceReadable.Text = latestHistoryItem.ResponceReadable;
                latestHistoryTimeReadable.Text = latestHistoryItem.TimeReadable;
                //latestHistoryTimeDetails.Text = latestHistoryItem.TimeDetails;

                //  if has washed hands, enable edit button and disable the yes/no buttons. else vice versa
                if (latestHistoryItem.Responce != 0)
                {
                    latestHistoryYesButton.IsVisible = false;
                    latestHistoryNoButton.IsVisible = false;
                    latestHistoryEditButton.IsVisible = true;
                }
                else
                {
                    latestHistoryYesButton.IsVisible = true;
                    latestHistoryNoButton.IsVisible = true;
                    latestHistoryEditButton.IsVisible = false;
                }
            }
        }

        private async Task UpdateLastNoResponse()
        {
            latestNoResponseItem = await App.Database.GetFirstNoResponseHistoryItemAsync();

            if (latestNoResponseItem == null)
            {
                latestNoRes.IsVisible = false;
            }
            else if (latestHistoryItem != null && latestNoResponseItem.ID == latestHistoryItem.ID)
            {
                latestNoRes.IsVisible = false;
            }
            else
            {
                latestNoRes.IsVisible = true;

                int noResponceCount = await App.Database.GetNoResponceCountHistoryItemAsync();
                
                if (noResponceCount == 0)
                {
                    throw new System.ArgumentException("Parameter found to be zero even though an item was found.", "noResponceCount");
                }
                else if (noResponceCount == 1)
                {
                    latestNoResLable.Text = noResponceCount.ToString() + " Activity Needs Responding";
                }
                else
                {
                    latestNoResLable.Text = noResponceCount.ToString() + " Activities Need Responding";
                }

                latestNoResBox.BackgroundColor = latestNoResponseItem.BoxColor;
                if (latestNoResponseItem.Action.Equals(Constants.actionEnter))
                {
                    latestNoResActionArrowImage.RotationY = 0;
                    latestNoResActionImage.RotationY = 0;
                }
                else
                {
                    latestNoResActionArrowImage.RotationY = 180;
                    latestNoResActionImage.RotationY = 180;
                }

                if (latestNoResponseItem.TrackedType.Equals(Constants.placeTypeName))
                {
                    latestNoResName.Text = latestNoResponseItem.TrackedType + ": " + latestNoResponseItem.Name;
                }
                else
                {
                    latestNoResName.Text = latestNoResponseItem.Name;
                }

                if (latestNoResponseItem.Reminder.Equals(Constants.washHands))
                {
                    latestNoResReminderHandsImage.IsVisible = true;
                    latestNoResReminderMaskImage.IsVisible = false;
                }
                else if (latestNoResponseItem.Reminder.Equals(Constants.wearMask))
                {
                    latestNoResReminderHandsImage.IsVisible = false;
                    latestNoResReminderMaskImage.IsVisible = true;
                }
                else
                {
                    latestNoResReminderHandsImage.IsVisible = true;
                    latestNoResReminderMaskImage.IsVisible = true;
                }

                //latestNoResResponceReadable.Text = latestNoResponseItem.ResponceReadable;
                latestNoResTimeReadable.Text = latestNoResponseItem.TimeReadable;
                //latestNoResTimeDetails.Text = latestNoResponseItem.TimeDetails;
                
                // the edit button is disabled
            }
        }

        async void OnHistoryButtonClicked(object sender, EventArgs args)
        {
            Button buttonClicked = (Button)sender;

            if (buttonClicked == latestHistoryYesButton)
            {
                await UpdateItem(latestHistoryItem, 1);
            }
            else if (buttonClicked == latestHistoryNoButton)
            {
                await UpdateItem(latestHistoryItem, 2);
            }
            else if (buttonClicked == latestHistoryEditButton)
            {
                await EditHistoryItem(latestHistoryItem);
            }
            else if (buttonClicked == latestNoResYesButton)
            {
                await UpdateItem(latestNoResponseItem, 1);
            }
            else if (buttonClicked == latestNoResNoButton)
            {
                await UpdateItem(latestNoResponseItem, 2);
            }
            //else if (buttonClicked == noResResponseButton)
            //    await EditHistoryItem(latestNoResponseItem);
            else
            {
                throw new System.ArgumentException("Parameter not found", "buttonClicked");
            }

            await UpdateLastestHistory();
            await UpdateLastNoResponse();
            await UpdateHistoryBreakdown();
        }

        private async Task UpdateItem(HistoryItem item, byte responce)
        {
            if (item.Responce == responce) return;

            item.Responce = responce;
            await App.Database.SaveHistoryAsync(item);
        }
        
        public async Task EditHistoryItem (HistoryItem item)
        {
            string action = await DisplayActionSheet($"Editing: Did you {item.Reminder}?", "Cancel", "Delete", "Yes", "No", "Clear");

            if (action == null || action.Equals("Cancel")) return;

            if (action.Equals("Delete"))
            {
                bool answer = await DisplayAlert($"Deleting \"{item.Action} {item.Name}\"", "Are you sure that you want to permanently delete this activity?", "Yes", "Cancel");
                if (answer) await App.Database.DeleteHistoryAsync(item);
                return;
            }
            else if (action.Equals("Clear"))
            {
                await UpdateItem(item, 0);
            }
            else if (action.Equals("Yes"))
            {
                await UpdateItem(item, 1);
            }
            else if (action.Equals("No"))
            {
                await UpdateItem(item, 2);
            }
            else
            {
                throw new System.ArgumentException("Parameter not found", "action");
            }
        }

        public async Task UpdateHistoryBreakdown()
        {
            // good, neutral, bad
            double[] breakdown = await App.Database.GetHistoryBreakdown();

            if (breakdown == null)
            {
                historyBreakdownGood.Text = "0%";
                historyBreakdownNeutral.Text = "0%";
                historyBreakdownBad.Text = "0%";
                standingEncouragement.Text = "There is no recorded history.";
                return;
            }

            historyBreakdownGood.Text = breakdown[0].ToString("P1", CultureInfo.InvariantCulture);
            historyBreakdownNeutral.Text = breakdown[1].ToString("P1", CultureInfo.InvariantCulture);
            historyBreakdownBad.Text = breakdown[2].ToString("P1", CultureInfo.InvariantCulture);

            if (breakdown[2] >= 0.5)
            {
                standingEncouragement.Text = "You need to follow through on your reminders more often.";
            }
            else if (breakdown[0] > 0.5)
            {
                standingEncouragement.Text = "You are doing well, keep it up!";
            } 
            else
            {
                standingEncouragement.Text = "You need to respond to more activity alerts.";
            }
        }

        //public void OnLocationToggle(object sender, ToggledEventArgs e)
        //{
        //    if (e.Value)
        //    {
        //        /*if (DependencyService.Get<ILocationService>().IsLocationServiceOn())
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            DependencyService.Get<ILocationService>().StartLocationService();*/

        //            if (Boolean.Parse(Preferences.Get("coordinate_key", "false")))
        //            {
        //                coordinateSwitch.IsToggled = true;
        //            }
        //            coordinateSwitch.IsEnabled = true;

        //            if (Boolean.Parse(Preferences.Get("category_key", "false")))
        //            {
        //                categorySwitch.IsToggled = true;
        //            }
        //            categorySwitch.IsEnabled = true;
        //        //}
        //    }
        //    else
        //    {
        //        /*if (!DependencyService.Get<ILocationService>().IsLocationServiceOn())
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            DependencyService.Get<ILocationService>().StopLocationService();*/

        //            Preferences.Set("coordinate_key", coordinateSwitch.IsToggled.ToString());
        //            coordinateSwitch.IsToggled = false;
        //            coordinateSwitch.IsEnabled = false;

        //            Preferences.Set("category_key", categorySwitch.IsToggled.ToString());
        //            categorySwitch.IsToggled = false;
        //            categorySwitch.IsEnabled = false;
        //        //}
        //    }
        //}

        //public void OnCoordinateToggle(object sender, ToggledEventArgs e)
        //{

        //}

        //public void OnCategoryToggle(object sender, ToggledEventArgs e)
        //{

        //}

        public async void OnHandwashingStepListButtonClicked(object sender, EventArgs args)
        {
            string instructions = "1. Wet your hands with clean, running water(warm or cold), turn off the tap, and apply soap.\n" +
                "2. Lather your hands by rubbing them together with the soap. Lather the backs of your hands, between your fingers, and under your nails.\n" +
                "3. Scrub your hands for at least 20 seconds.Need a timer ? Hum the “Happy Birthday” song from beginning to end twice.\n" +
                "4. Rinse your hands well under clean, running water.\n" +
                "5. Dry your hands using a clean towel or air dry them.\n\n" +
                "Source: https://www.cdc.gov/handwashing/when-how-handwashing.html";

            await DisplayAlert("Hand Washing Instructions", instructions, "OK");
        }

        public async void OnHandwashingVideoButtonClicked(object sender, EventArgs args)
        {
            await Browser.OpenAsync("https://www.youtube.com/watch?v=IisgnbMfKvI", BrowserLaunchMode.SystemPreferred);
        }

        public async void OnMaskWearingGuidelinesButtonClicked(object sender, EventArgs args)
        {
            string instructions = "- Wear masks with two or more layers to stop the spread of COVID-19\n" +
                "- Wear the mask over your nose and mouth and secure it under your chin\n" +
                "- Masks should be worn by people two years and older\n" +
                "- Masks should NOT be worn by children younger than two, people who have trouble breathing, or people who cannot remove the mask without assistance\n" +
                "- Do NOT wear masks intended for healthcare workers, for example, N95 respirators\n\n" +
                "Source: https://www.cdc.gov/coronavirus/2019-ncov/prevent-getting-sick/about-face-coverings.html.";

            await DisplayAlert("Mask Wearing Guidelines", instructions, "OK");
        }

        public async void OnMaskWearingVideoButtonClicked(object sender, EventArgs args)
        {
            await Browser.OpenAsync("https://www.youtube.com/watch?v=ciUniZGD4tY", BrowserLaunchMode.SystemPreferred);
        }

        private void UpdateLocationTrackingButtons(bool isOn = false)
        {
            if (isOn || locationDependencyService.IsLocationServiceOn())
            {
                ltTurnOnButton.IsEnabled = false;
                //ltTurnOnButton.BackgroundColor = Color.LightGreen;
                ltTurnOffButton.IsEnabled = true;
                //ltTurnOffButton.BackgroundColor = Color.White;
            }
            else
            {
                ltTurnOnButton.IsEnabled = true;
                //ltTurnOnButton.BackgroundColor = Color.White;
                ltTurnOffButton.IsEnabled = false;
                //ltTurnOffButton.BackgroundColor = Color.IndianRed;
            }
        }

        private async void LocationTrackingTurnOnButtonClicked(object sender, EventArgs e)
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


            if (!App.Settings.CoordinateTrackingEnabled && !App.Settings.PlaceTrackingEnabled)
            {
                await DisplayAlert("Tracking Categories Disabled", "At least one of the tracking categories needs to be enabled for location tracking to do anything.\n\nLocation tracking won't be enabled untill a tracking category is enabled in their respective page.", "OK");
                return;
            }

            if (!locationDependencyService.IsLocationServiceOn())
            {
                if (await AttemptGPSAsync())
                {
                    locationDependencyService.StartLocationService();
                    UpdateLocationTrackingButtons(true);
                }
            }
            else
            {
                await DisplayAlert("Location Tracking Already Enabled", "This shouldn't be occuring.", "OK");
            }
        }

        private void LocationTrackingTurnOffButtonClicked(object sender, EventArgs e)
        {
            if (locationDependencyService.IsLocationServiceOn())
            {
                locationDependencyService.StopLocationService();
                UpdateLocationTrackingButtons(false);
            }
        }

        public async Task<bool> AttemptGPSAsync()
        {
            try
            {
                Debug.WriteLine("Getting device GPS capabilities.");

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best);
                Location location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Debug.WriteLine($"Got the device's current location: {location.Latitude}, {location.Longitude}.");
                }
                else
                {
                    Debug.WriteLine("Failed to get the device's current location.");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Debug.WriteLine($"Failed to get the device's current location: {fnsEx}.");
                await App.Current.MainPage.DisplayAlert("GPS Not Supported", "Your device does not have gps capabilities.\n\nLocation tracking can't be enabled on this device possibly.", "OK");
                return false;
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Debug.WriteLine($"Failed to get the device's current location: {fneEx}.");
                await App.Current.MainPage.DisplayAlert("GPS Not Enabled", "Your device does not have its gps enabled.\n\nLocation tracking will still be enabled but it won't receive updates untill the gps is enabled.", "OK");
                return true;
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Debug.WriteLine($"Failed to get the device's current location: {pEx}.");
                await App.Current.MainPage.DisplayAlert("GPS Permission Denied", "This app does not have permission to access your location.\n\nLocation tracking can't be enabled untill permission is granted.", "OK");
                return false;
            }
            catch (Exception ex)
            {
                // Unable to get location
                Debug.WriteLine($"Failed to get the device's current location: {ex}.");
                await App.Current.MainPage.DisplayAlert("Unexpected GPS Failure", "Something unexpected happened.\n\nLocation tracking won't be enabled for now. If this continues to occur then something might have broken in this app.", "OK");
                return false;
            }
            return true;
        }

    }
}