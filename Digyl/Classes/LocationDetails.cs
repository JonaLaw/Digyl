using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using System.Diagnostics;

namespace Digyl
{
    public static class LocationDetails
    {
        public static async Task<Location> GetLocationFromUser()
        {
            Debug.WriteLine("Asking for a location from the user.");
            string lat = await App.Current.MainPage.DisplayPromptAsync("Latitude", "Enter a Latitude");
            string lon = await App.Current.MainPage.DisplayPromptAsync("Longitude", "Enter a Longitude");
            if (!await ValidateCoordinates(lat, lon)) return null;

            Debug.WriteLine($"Got a location from the user: {lat}, {lon}.");
            return new Location(double.Parse(lat), double.Parse(lon));
        }

        public static async Task<Location> GetCurrentLocation()
        {
            try
            {
                Debug.WriteLine("Trying to get the device's current location.");

                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best);
                Location location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Debug.WriteLine($"Got the device's current location: {location.Latitude}, {location.Longitude}.");
                    return location;
                }

                Debug.WriteLine("Failed to get the device's current location.");
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Debug.WriteLine($"Failed to get the device's current location: {fnsEx}.");
                await App.Current.MainPage.DisplayAlert("Geolocation Request Failure", "Your device does not have geolocation capabilities.", "OK");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Debug.WriteLine($"Failed to get the device's current location: {fneEx}.");
                await App.Current.MainPage.DisplayAlert("Geolocation Request Failure", "Your device does not have geolocation enabled.", "OK");
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Debug.WriteLine($"Failed to get the device's current location: {pEx}.");
                await App.Current.MainPage.DisplayAlert("Geolocation Request Failure", "This app does not have permission to access your location.", "OK");
            }
            catch (Exception ex)
            {
                // Unable to get location
                Debug.WriteLine($"Failed to get the device's current location: {ex}.");
                await App.Current.MainPage.DisplayAlert("Geolocation Request Failure", "Something unexpected happened.", "OK");
            }
            return null;
        }

        public static async Task<bool> ValidateCoordinates(string latStr, string lonStr)
        {
            try
            {
                Debug.WriteLine("Trying to validate coordinates.");

                double lat = double.Parse(latStr);
                if (lat < -90 || lat > 90)
                {
                    Debug.WriteLine("Latitude is not valid.");
                    await App.Current.MainPage.DisplayAlert("Invalid Latitude Entry", "The Latitude entry is not between -90 and 90.", "OK");
                    return false;
                }

                double lon = double.Parse(lonStr);
                if (lon < -180 || lon > 180)
                {
                    Debug.WriteLine("Longitude is not valid.");
                    await App.Current.MainPage.DisplayAlert("Invalid Longitude Entry", "The Longitude entry is not between -180 and 180.", "OK");
                    return false;
                }

                Debug.WriteLine("Coordinates are valid.");
                return true;
            }
            catch
            {
                Debug.WriteLine("Coordinates are not valid.");
                await App.Current.MainPage.DisplayAlert("Invalid Coordinates Entry", "The Latitude or Longitude fields have invalid characters in them.", "OK");
                return false;
            }
        }

        public static async Task<List<string>> GetNearbyPlaceTypes(Location location, string[] typesToInclude = null)
        {
            Debug.WriteLine("Getting nearby place types.");

            using (var client = new HttpClient())
            {
                string requestURL;
                if (typesToInclude != null)
                {
                    string typesToIncludeURL = string.Join(",", typesToInclude);

                    requestURL = string.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&type={3}&key={4}",
                        location.Latitude,
                        location.Longitude,
                        Constants.placeTypeRadius,
                        typesToIncludeURL,
                        Constants.apiKey);
                }
                else
                {
                    requestURL = string.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius={2}&key={3}",
                        location.Latitude,
                        location.Longitude,
                        Constants.placeTypeRadius,
                        Constants.apiKey);
                }

                Debug.WriteLine($"Requesting url: {requestURL}.");
                string response = await client.GetStringAsync(requestURL);
                JObject googlePlaceSearch = JObject.Parse(response);
                string status = (string)googlePlaceSearch["status"];
                Debug.WriteLine($"Google api place search status: {status}.");

                if (status.Equals("OK"))
                {
                    Debug.WriteLine("parsing JSON results");
                    // https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm
                    var types = from t in googlePlaceSearch["results"].SelectMany(i => i["types"])
                                group t by t
                                into g
                                orderby g.Count() descending
                                select new { Type = g.Key, Count = g.Count() };

                    if (!types.Any())
                    {
                        Debug.WriteLine("Google place search found no place types.");
                        return null;
                    }

                    List<string> typeNames = new List<string>();
                    foreach (var t in types)
                    {
                        //Debug.WriteLine(t.Type + " - Count: " + t.Count);
                        typeNames.Add(t.Type.ToString());
                    }

                    Debug.WriteLine($"Google place search found {typeNames.Count} place types.");
                    return typeNames;
                }

                Debug.WriteLine("Search status was bad, returning null.");
                return null;
            }
        }

        public static async Task<PlaceTypeItem> GetNearbyPlaceTypeItem(Location location, string[] typesToInclude = null)
        {
            Debug.WriteLine("Getting nearby place type item.");
            List<string> placeTypes = await GetNearbyPlaceTypes(location, typesToInclude);
            if (placeTypes == null) return null;

            Debug.WriteLine("Fonud nearby place type item.");
            return await App.Database.QueryPlaceTypes(placeTypes);
        }

        public static bool IsInLocation(Location locationStart, CoordinateItem coordItem)
        {
            if (Location.CalculateDistance(locationStart, coordItem.Latitude, coordItem.Longitude, DistanceUnits.Kilometers) * 1000 <= coordItem.Radius)
            {
                return true;
            }
            return false;
        }

        public static bool IsInLocation(Location locationStart, Location loctionEnd, int radius)
        {
            if (Location.CalculateDistance(locationStart, loctionEnd, DistanceUnits.Kilometers) * 1000 <= radius)
            {
                return true;
            }
            return false;
        }

        public static void AddHistoryItem(CoordinateItem coordItem, string action)
        {
            if (action.Equals(Constants.actionEnter))
            {
                _ = AddHistoryItem(coordItem, true);
            }
            else if (action.Equals(Constants.actionExit))
            {
                _ = AddHistoryItem(coordItem, false);
            }
            else
            {
                throw new ArgumentException($"The action must either be Constants.actionEnter: {Constants.actionEnter}, or Constants.actionExit: {Constants.actionExit}.", "action");
            }
        }

        public static async Task<int> AddHistoryItem(CoordinateItem coordItem, bool action)
        {
            int id = await App.Database.SaveHistoryAsync(new HistoryItem
            {
                OriginID = coordItem.ID,
                TrackedType = Constants.coordinateName,
                Name = coordItem.Name,
                Action = action ? Constants.actionEnter : Constants.actionExit,
                Reminder = action ? coordItem.OnEnterReminder : coordItem.OnExitReminder
            });

            if (action) Debug.WriteLine($"A history item for {Constants.actionEnter} - {Constants.coordinateName}: {coordItem.Name}, action: {coordItem.OnEnterReminder}, was created.");
            else Debug.WriteLine($"A history item for {Constants.actionExit} - {Constants.coordinateName}: {coordItem.Name}, action: {coordItem.OnExitReminder}, was created.");

            return id;
        }

        public static void AddHistoryItem(PlaceTypeItem placeItem, string action)
        {
            if (action.Equals(Constants.actionEnter))
            {
                _ = AddHistoryItem(placeItem, true);
            }
            else if (action.Equals(Constants.actionExit))
            {
                _ = AddHistoryItem(placeItem, false);
            }
            else
            {
                throw new ArgumentException($"The action must either be Constants.actionEnter: {Constants.actionEnter}, or Constants.actionExit: {Constants.actionExit}.", "action");
            }
        }

        public static async Task<int> AddHistoryItem(PlaceTypeItem placeItem, bool action)
        {
            int id = await App.Database.SaveHistoryAsync(new HistoryItem
            {
                OriginID = placeItem.ID,
                TrackedType = Constants.placeTypeName,
                Name = placeItem.NameReadable,
                Action = action ? Constants.actionEnter : Constants.actionExit,
                Reminder = action ? placeItem.OnEnterReminder : placeItem.OnExitReminder
            });

            if (action) Debug.WriteLine($"A history item for {Constants.actionEnter} - {Constants.placeTypeName}: {placeItem.Name}, action: {placeItem.OnEnterReminder}, was created.");
            else Debug.WriteLine($"A history item for {Constants.actionExit} - {Constants.placeTypeName}: {placeItem.Name}, action: {placeItem.OnExitReminder}, was created.");

            return id;
        }
    }
}
