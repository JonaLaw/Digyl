using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Digyl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaceTypesDetailsPage : ContentPage
    {
        private bool _initial;
        private readonly ParentPlaceTypeItem parentType;

        public PlaceTypesDetailsPage(ParentPlaceTypeItem item)
        {
            InitializeComponent();
            parentType = item;
            this.Title = parentType.Name + " Type Details";
        }

        protected override async void OnAppearing()
        {
            _initial = true;
            base.OnAppearing();
            listView.ItemsSource = await App.Database.GetPlaceTypesAsync(parentType);

            await Task.Delay(200);
            _initial = false;
        }

        async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

            PlaceTypeItem placeType = e.Item as PlaceTypeItem;

            string placeTypeNameReadable = placeType.NameReadable;
            bool answer = await DisplayAlert(placeTypeNameReadable, placeType.Details, "EDIT", "OK");

            if (answer)
            {
                string action = await DisplayActionSheet($"Select {placeTypeNameReadable} Action Reminder To Edit", "Cancel", null, Constants.actionEnter, Constants.actionExit);

                if (action == null || action.Equals("Cancel")) return;

                string action2;
                if (action.Equals(Constants.actionEnter))
                {
                    if (placeType.OnExit)
                    {
                        action2 = await DisplayActionSheet($"Setting {placeTypeNameReadable} On Enter Reminder", "Cancel", null, "Clear", Constants.wearMask, Constants.washHands, Constants.washHandsAndWearMask);
                    }
                    else
                    {
                        action2 = await DisplayActionSheet($"Setting {placeTypeNameReadable} On Enter Reminder", "Cancel", null, Constants.wearMask, Constants.washHands, Constants.washHandsAndWearMask);
                    }

                    if (action2 == null || action2.Equals("Cancel")) return;

                    if (action2.Equals("Clear"))
                    {
                        placeType.OnEnter = false;
                        placeType.OnEnterReminder = "";
                    }
                    else
                    {
                        placeType.OnEnter = true;
                        placeType.OnEnterReminder = action2;
                    }
                }
                else
                {
                    if (placeType.OnEnter)
                    {
                        action2 = await DisplayActionSheet($"Setting {placeTypeNameReadable} On Exit Reminder", "Cancel", null, "Clear", Constants.wearMask, Constants.washHands, Constants.washHandsAndWearMask);
                    }
                    else
                    {
                        action2 = await DisplayActionSheet($"Setting {placeTypeNameReadable} On Exit Reminder", "Cancel", null, Constants.wearMask, Constants.washHands, Constants.washHandsAndWearMask);
                    }

                    if (action2 == null || action2.Equals("Cancel")) return;

                    if (action2.Equals("Clear"))
                    {
                        placeType.OnExit = false;
                        placeType.OnExitReminder = "";
                    }
                    else
                    {
                        placeType.OnExit = true;
                        placeType.OnExitReminder = action2;
                    }
                }

                await App.Database.SavePlaceTypeAsync(placeType);
            }
        }

        async void OnToggle(object sender, ToggledEventArgs e)
        {
            if (_initial) return;

            Switch sw = sender as Switch;
            PlaceTypeItem placeType = sw.BindingContext as PlaceTypeItem;
            await App.Database.SavePlaceTypeAsync(placeType);
        }

        async void OnAddHistoryButtonClicked(object sender, EventArgs e)
        {
            Button aButton = sender as Button;
            PlaceTypeItem placeTypeItem = aButton.BindingContext as PlaceTypeItem;

            string action;
            if (placeTypeItem.OnEnter)
            {
                if (placeTypeItem.OnExit)
                {
                    action = await DisplayActionSheet("Adding manual history activity, select the movement type.", "Cancel", null, Constants.actionEnter, Constants.actionExit);
                }
                else
                {
                    action = await DisplayActionSheet("Adding manual history activity, select the movement type.", "Cancel", null, Constants.actionEnter);
                }
            }
            else if (placeTypeItem.OnExit)
            {
                action = await DisplayActionSheet("Adding manual history activity, select the movement type.", "Cancel", null, Constants.actionExit);
            }
            else
            {
                throw new System.ArgumentException("All place type items must have an on enter and/or an on exit reminder.", "placeTypeItem.OnEnter, placeTypeItem.OnExit");
            }

            if (action == null || action.Equals("Cancel")) return;

            LocationDetails.AddHistoryItem(placeTypeItem, action);

            //await DisplayAlert("History Item added", $"A history item for {placeTypeItem.Name} was created.", "OK");
        }
    }
}