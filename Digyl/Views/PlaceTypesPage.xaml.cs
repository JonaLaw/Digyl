using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Digyl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlaceTypesPage : ContentPage
    {
        private bool _initial;

        public PlaceTypesPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            _initial = true;
            base.OnAppearing();
            placeTypeSettings.IsToggled = App.Settings.PlaceTrackingEnabled;
            listView.ItemsSource = await App.Database.GetParentPlaceTypesAsync();

            await Task.Delay(200);
            _initial = false;
        }

        void OnSettingsToggle(object sender, ToggledEventArgs e)
        {
            if (_initial) return;
            App.Settings.PlaceTrackingEnabled = e.Value;
        }

        async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            ParentPlaceTypeItem parentType = e.Item as ParentPlaceTypeItem;
            await Navigation.PushAsync(new PlaceTypesDetailsPage(parentType));
        }

        async void OnItemToggle(object sender, ToggledEventArgs e)
        {
            if (_initial) return;

            Switch sw = sender as Switch;
            ParentPlaceTypeItem parentType = sw.BindingContext as ParentPlaceTypeItem;
            await App.Database.UpdateParentPlaceTypeAsync(parentType);
        }
    }
}
