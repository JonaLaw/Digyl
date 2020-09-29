using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Digyl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoordinatesPage : ContentPage
    {
        private bool _initial;

        public CoordinatesPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            _initial = true;

            base.OnAppearing();
            coordinateSettings.IsToggled = App.Settings.CoordinateTrackingEnabled;

            List<CoordinateItem> items = await App.Database.GetCoordinatesAsync();
            listView.ItemsSource = items;

            if (items.Any())
            {
                noCoordinates.IsVisible = false;
            }
            else
            {
                noCoordinates.IsVisible = true;
            }

            await Task.Delay(200);
            _initial = false;
        }

        void OnSettingsToggle(object sender, ToggledEventArgs e)
        {
            if (_initial) return;
            App.Settings.CoordinateTrackingEnabled = e.Value;
        }

        async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            CoordinateItem tappedCoordinateItem = e.Item as CoordinateItem;

            bool answer = await DisplayAlert(tappedCoordinateItem.Name, tappedCoordinateItem.Details, "EDIT", "OK");

            if (answer)
            {
                await Navigation.PushAsync(new CoordinatesAddPage(tappedCoordinateItem));
            }
        }

        async void OnItemToggle(object sender, ToggledEventArgs e)
        {
            if (_initial) return;

            Switch sw = sender as Switch;
            CoordinateItem coordinateItem = sw.BindingContext as CoordinateItem;
            await App.Database.SaveCoordinateAsync(coordinateItem);
        }

        async void OnAddHistoryButtonClicked(object sender, EventArgs e)
        {
            Button aButton = sender as Button;
            CoordinateItem coordinateItem = aButton.BindingContext as CoordinateItem;

            string action;
            if (coordinateItem.OnEnter)
            {
                if (coordinateItem.OnExit)
                {
                    action = await DisplayActionSheet("Adding manual history activity, select the movement type.", "Cancel", null, Constants.actionEnter, Constants.actionExit);
                }
                else
                {
                    action = await DisplayActionSheet("Adding manual history activity, select the movement type.", "Cancel", null, Constants.actionEnter);
                }
            }
            else if (coordinateItem.OnExit)
            {
                action = await DisplayActionSheet("Adding manual history activity, select the movement type.", "Cancel", null, Constants.actionExit);
            }
            else
            {
                throw new System.ArgumentException("All coordinate Items must have an on enter and/or an on exit reminder.", "coordinateItem.OnEnter, coordinateItem.OnExit");
            }

            if (action == null || action.Equals("Cancel")) return;

            LocationDetails.AddHistoryItem(coordinateItem, action);

            //await DisplayAlert("History Item added", $"A history item for {coordinateItem.Name} was created.", "OK");
        }

        async void OnAddNewButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CoordinatesAddPage());
        }
    }
}