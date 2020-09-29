using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Digyl
{
    public class LocationUpdateHandler
    {
        private readonly ILocationService locationDependencyService;
        private Location startLocation;
        private CoordinateItem trackedCoordinateItem;
        private PlaceTypeItem trackedPlaceTypeItem;

        public LocationUpdateHandler()
        {
            locationDependencyService = DependencyService.Get<ILocationService>();
            startLocation = null;
            trackedCoordinateItem = null;
            trackedPlaceTypeItem = null;
        }

        /*public async void StartLocationHandler()
        {

        }*/

        public async Task UpdateLocation(double latitude, double longitude)
        {
            Location newLocation = new Location(latitude, longitude);
            Debug.WriteLine($"Updated location: ({latitude}, {longitude})");

            if (App.Settings.CoordinateTrackingEnabled)
            {
                if (trackedCoordinateItem != null)
                {
                    if (!LocationDetails.IsInLocation(newLocation, trackedCoordinateItem))
                    {
                        if (trackedCoordinateItem.OnExit)
                        {
                            _ = TriggerAlert(trackedCoordinateItem, false);
                        }
                        trackedCoordinateItem = null;
                    }
                    return;
                }

                CoordinateItem foundCoordinateItem = await App.Database.QueryCoordinatesAsync(newLocation);
                if (foundCoordinateItem != null)
                {
                    trackedCoordinateItem = foundCoordinateItem;
                    if (trackedCoordinateItem.OnEnter)
                    {
                        _ = TriggerAlert(trackedCoordinateItem, true);
                    }
                    return;
                }
            }
            /*else
            {
                trackedCoordinateItem = null;
            }*/
            
            if (App.Settings.PlaceTrackingEnabled)
            {
                if (trackedPlaceTypeItem != null)
                {
                    if (!LocationDetails.IsInLocation(newLocation, startLocation, App.Settings.PlaceTypeLeaveRadius))
                    {
                        List<string> foundPlaceTypes = await LocationDetails.GetNearbyPlaceTypes(newLocation);

                        if (foundPlaceTypes.Exists(x => x.Equals(trackedPlaceTypeItem.Name)))
                        {
                            startLocation = newLocation;
                        }
                        else
                        {
                            if (trackedPlaceTypeItem.OnExit)
                            {
                                _ = TriggerAlert(trackedPlaceTypeItem, false);
                            }
                            trackedPlaceTypeItem = null;
                        }
                    }
                    return;
                }
                else
                {
                    List<string> foundPlaceTypes = await LocationDetails.GetNearbyPlaceTypes(newLocation);
                    if (foundPlaceTypes.Any())
                    {
                        PlaceTypeItem foundPlaceTypeItem = await App.Database.QueryPlaceTypes(foundPlaceTypes);
                        if (foundPlaceTypeItem != null)
                        {
                            trackedPlaceTypeItem = foundPlaceTypeItem;
                            if (trackedPlaceTypeItem.OnEnter)
                            {
                                _ = TriggerAlert(trackedPlaceTypeItem, true);
                            }
                            startLocation = newLocation;
                            return;
                        }
                    }
                }
            }
            /*else
            {
                trackedPlaceTypeItem = null;
            }*/
        }

        private async Task TriggerAlert(CoordinateItem item, bool action)
        {
            Debug.WriteLine($"Triggered Alter for {item.Name}, {action}");
            int id = await LocationDetails.AddHistoryItem(item, action);

            string notifTitle;
            string notifText;
            if (action)
            {
                notifTitle = $"{Constants.actionEnter} {Constants.coordinateName}: {item.Name}";
                notifText = $"Reminder to {item.OnEnterReminder}";
            }
            else
            {
                notifTitle = $"{Constants.actionExit} {Constants.coordinateName}: {item.Name}";
                notifText = $"Reminder to {item.OnExitReminder}";
            }

            locationDependencyService.UpdateNotification(notifTitle, notifText, id);
        }

        private async Task TriggerAlert(PlaceTypeItem item, bool action)
        {
            Debug.WriteLine($"Triggered Alter for {item.Name}, {action}");
            int id = await LocationDetails.AddHistoryItem(item, action);

            string notifTitle;
            string notifText;
            if (action)
            {
                notifTitle = $"{Constants.actionEnter} {Constants.placeTypeName}: {item.NameReadable}";
                notifText = $"Reminder to {item.OnEnterReminder}";
            }
            else
            {
                notifTitle = $"{Constants.actionExit} {Constants.placeTypeName}: {item.NameReadable}";
                notifText = $"Reminder to {item.OnExitReminder}";
            }

            locationDependencyService.UpdateNotification(notifTitle, notifText, id);
        }

        public async Task UpdateLastHistoryItem(int id, bool response)
        {
            Debug.WriteLine("Trying to update the last history item");
            HistoryItem item = await App.Database.GetFirstHistoryItemAsync();
            if (item == null)
            {
                Debug.WriteLine("Did not find any history items");
                return;
            }

            if (item.ID != id)
            {
                Debug.WriteLine($"The last history item's id of {item.ID} did not match {id}.");
                return;
            }

            string notifTitle = $"Responded to {item.Action} {item.TrackedType}: {item.Name}";
            string notifText;
            if (response)
            {
                item.Responce = 1;
                notifText = $"Did {item.Reminder}";
            }
            else
            {
                item.Responce = 2;
                notifText = $"Didn't {item.Reminder}";
            }

            await App.Database.SaveHistoryAsync(item);
            Debug.WriteLine($"{notifTitle} with {notifText}");

            locationDependencyService.UpdateNotification(notifTitle, notifText, 0);
        }
    }
}
