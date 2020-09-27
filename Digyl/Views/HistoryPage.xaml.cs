using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Digyl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentPage
    {
        private readonly List<string> typePickerItems;
        private readonly List<string> actionPickerItems;
        private readonly List<string> reminderPickerItems;

        public HistoryPage()
        {
            typePickerItems = new List<string>();
            actionPickerItems = new List<string>();
            reminderPickerItems = new List<string>();

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            typePickerItems.Add("All");
            typePickerItems.Add(Constants.coordinateName);
            typePickerItems.Add(Constants.placeTypeName);
            filterTypePicker.ItemsSource = typePickerItems;
            filterTypePicker.SelectedIndex = 0;

            actionPickerItems.Add("All");
            actionPickerItems.Add(Constants.actionEnter);
            actionPickerItems.Add(Constants.actionExit);
            filterActionPicker.ItemsSource = actionPickerItems;
            filterActionPicker.SelectedIndex = 0;

            reminderPickerItems.Add("All");
            reminderPickerItems.Add(Constants.washHands);
            reminderPickerItems.Add(Constants.wearMask);
            reminderPickerItems.Add(Constants.washHandsAndWearMask);
            filterReminderPicker.ItemsSource = reminderPickerItems;
            filterReminderPicker.SelectedIndex = 0;

            filterResponsePicker.SelectedIndex = 0;

            base.OnAppearing();
            await RefreshListView();   
        }

        void OnFilterClearButtonClicked(object sender, EventArgs args)
        {
            filterTypePicker.SelectedIndex = 0;
            filterActionPicker.SelectedIndex = 0;
            filterReminderPicker.SelectedIndex = 0;
            filterResponsePicker.SelectedIndex = 0;
        }

        async void OnFilterButtonClicked(object sender, EventArgs args)
        {
            string typeFilter = null;
            if (filterTypePicker.SelectedIndex != 0)
            {
                typeFilter = filterTypePicker.SelectedItem.ToString();
            }

            string actionFilter = null;
            if (filterActionPicker.SelectedIndex != 0)
            {
                actionFilter = filterActionPicker.SelectedItem.ToString();
            }

            string reminderFilter = null;
            if (filterReminderPicker.SelectedIndex != 0)
            {
                reminderFilter = filterReminderPicker.SelectedItem.ToString();
            }

            byte responseFilter;
            if (filterResponsePicker.SelectedIndex != 0)
            {
                if (filterResponsePicker.SelectedIndex == 1)
                {
                    responseFilter = 1;
                }
                else if (filterResponsePicker.SelectedIndex == 2)
                {
                    responseFilter = 0;
                }
                else
                {
                    responseFilter = 2;
                }
            }
            else
            {
                responseFilter = 3;
            }

            await RefreshListView(typeFilter, actionFilter, reminderFilter, responseFilter);
        }

        private async Task RefreshListView(string typeFilter = null, string actionFilter = null, string reminderFilter = null, byte responceFilter = 3)
        {
            var list = await App.Database.GetHistoryAsync(typeFilter, actionFilter, reminderFilter, responceFilter);
            listView.ItemsSource = list;
            
            if (list.Any())
            {
                nothingLabel.IsVisible = false;
            }
            else
            {
                nothingLabel.IsVisible = true;
            }
        }

        //async void OnItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    bool answer = await DisplayAlert("Delete History Entry", $"Do you want to permanently delete this history entry?", "Yes", "No");

        //    if (answer)
        //    {
        //        HistoryItem tappedItem = e.Item as HistoryItem;

        //        await App.Database.DeleteHistoryAsync(tappedItem);
        //        listView.ItemsSource = await App.Database.GetHistoryAsync();
        //    }
        //}

        async void OnButtonClicked(object sender, EventArgs args)
        {
            Button button = (Button)sender;
            HistoryItem historyItem = (HistoryItem)button.BindingContext;

            //bool answer = await DisplayAlert("Response", $"Did you wash your hand upon entering: {historyItem.Name}", "Yes", "No");
            string action = await DisplayActionSheet($"Editing: Did you {historyItem.Reminder}?", "Cancel", "Delete", "Yes", "No", "Clear");

            if (action == null || action.Equals("Cancel")) return;

            if (action.Equals("Delete"))
            {
                await App.Database.DeleteHistoryAsync(historyItem);
                listView.ItemsSource = await App.Database.GetHistoryAsync();
                return;
            }
            else if (action.Equals("Clear"))
            {
                historyItem.Responce = 0;
            }
            else if (action.Equals("Yes"))
            {
                historyItem.Responce = 1;
            }
            else if (action.Equals("No"))
            {
                historyItem.Responce = 2;
            }
            else
            {
                throw new System.ArgumentException("Parameter not found", "action");
            }

            await App.Database.SaveHistoryAsync(historyItem);
            listView.ItemsSource = await App.Database.GetHistoryAsync();
        }
    }
}