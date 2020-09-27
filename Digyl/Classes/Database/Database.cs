using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using Xamarin.Essentials;

namespace Digyl
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            bool firstRun = false;
            if (!File.Exists(dbPath))
            {
                firstRun = true;
            }

            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<ParentPlaceTypeItem>().Wait();
            _database.CreateTableAsync<PlaceTypeItem>().Wait();
            _database.CreateTableAsync<CoordinateItem>().Wait();
            _database.CreateTableAsync<HistoryItem>().Wait();

            if (firstRun)
            {
                FillPlaceTypeDataBase().Wait();
            }
        }

        public async Task ClearDataBases ()
        {
            await _database.DropTableAsync<ParentPlaceTypeItem>();
            await _database.DropTableAsync<PlaceTypeItem>();
            await _database.DropTableAsync<CoordinateItem>();
            await _database.DropTableAsync<HistoryItem>();

            await _database.CreateTableAsync<ParentPlaceTypeItem>();
            await _database.CreateTableAsync<PlaceTypeItem>();
            await _database.CreateTableAsync<CoordinateItem>();
            await _database.CreateTableAsync<HistoryItem>();

            await FillPlaceTypeDataBase();
        }


        // place types
        private async Task FillPlaceTypeDataBase()
        {
            //System.Diagnostics.Debug.WriteLine("NEW DB");

            string[] EnterMaskExitWash = { Constants.wearMask, Constants.washHands };

            await FillPlaceType("Food", new string[] { "food", "bakery", "cafe", "meal_delivery", "meal_takeaway", "restaurant" }, new string[] { Constants.washHandsAndWearMask, "" });
            await FillPlaceType("Shopping", new string[] { "store", "supermarket", "grocery_or_supermarket" }, EnterMaskExitWash);
            await FillPlaceType("School", new string[] { "school", "primary_school", "secondary_school", "university" }, EnterMaskExitWash);
            await FillPlaceType("Amusement", new string[] { "amusement_park", "aquarium", "art_gallery", "bowling_alley",
                "casino", "library", "movie_theater", "museum", "night_club", "spa", "stadium", "zoo" }, EnterMaskExitWash);
            await FillPlaceType("Worship", new string[] { "place_of_worship", "church", "hindu_temple", "mosque", "synagogue" }, EnterMaskExitWash);
            await FillPlaceType("Administrative", new string[] { "city_hall", "courthouse", "local_government_office", "police", "post_office" }, EnterMaskExitWash);
            await FillPlaceType("Health", new string[] { "health", "hospital", "pharmacy", "physiotherapist" }, EnterMaskExitWash);
            await FillPlaceType("Travel", new string[] { "airport", "light_rail_station", "subway_station", "train_station", "transit_station", "travel_agency" }, EnterMaskExitWash);
        }

        private async Task FillPlaceType(string parentType, string[] childTypes, string[] reminders)
        {
            await _database.InsertAsync(new ParentPlaceTypeItem
            {
                Name = parentType,
                IsOn = true
            });

            ParentPlaceTypeItem parentTypeItem = await _database.Table<ParentPlaceTypeItem>().Where(p => p.Name.Equals(parentType)).FirstOrDefaultAsync();

            if (parentTypeItem == null)
                throw new System.ArgumentException("Parent type name was not found", "parentType");

            foreach (string childType in childTypes)
            {
                await _database.InsertAsync(new PlaceTypeItem
                {
                    ParentPlaceTypeID = parentTypeItem.ID,
                    Name = childType,
                    OnEnter = !string.IsNullOrWhiteSpace(reminders[0]),
                    OnExit = !string.IsNullOrWhiteSpace(reminders[1]),
                    OnEnterReminder = reminders[0],
                    OnExitReminder = reminders[1],
                    IsOn = true
                });
            }
        }

        // parent place types
        public Task<List<ParentPlaceTypeItem>> GetParentPlaceTypesAsync()
        {
            return _database.Table<ParentPlaceTypeItem>().ToListAsync();
        }

        public Task<int> SaveParentPlaceTypeAsync(ParentPlaceTypeItem item)
        {
            if (item.ID != 0)
            {
                return _database.UpdateAsync(item);
            }
            else
            {
                return _database.InsertAsync(item);
            }
        }

        public async Task UpdateParentPlaceTypeAsync(ParentPlaceTypeItem item)
        {
            await SaveParentPlaceTypeAsync(item);
            await UpdateTypesAsync(item);
        }

        private async Task UpdateTypesAsync(ParentPlaceTypeItem parentType)
        {
            List<PlaceTypeItem> placeTypesToUpdate = await GetPlaceTypesAsync(parentType);

            if (placeTypesToUpdate != null)
            {
                foreach (PlaceTypeItem item in placeTypesToUpdate)
                {
                    item.IsOn = parentType.IsOn;
                    await SavePlaceTypeAsync(item);
                }
            }
            else
            {
                throw new System.ArgumentException("Type name was not found", "typeName");
            }
        }


        // child place types
        public Task<List<PlaceTypeItem>> GetPlaceTypesAsync(ParentPlaceTypeItem parentType)
        {
            return _database.Table<PlaceTypeItem>().Where(t => t.ParentPlaceTypeID == parentType.ID).ToListAsync();
        }
        public Task<PlaceTypeItem> GetPlaceTypeItemAsync(int palceID)
        {
            return _database.Table<PlaceTypeItem>().Where(p => p.ID == palceID).FirstOrDefaultAsync();
        }

        public Task<int> SavePlaceTypeAsync(PlaceTypeItem item)
        {
            if (item.ID != 0)
            {
                return _database.UpdateAsync(item);
            }
            else
            {
                return _database.InsertAsync(item);
            }
        }

        public async Task<PlaceTypeItem> QueryPlaceTypes(List<string> typeNames)
        {
            PlaceTypeItem foundType;

            foreach (string type in typeNames)
            {
                foundType = await _database.Table<PlaceTypeItem>().Where(t => t.IsOn && t.Name.Equals(type)).FirstOrDefaultAsync();
                if (foundType != null) return foundType;
            }

            return null;
        }


        // coordinates
        public Task<List<CoordinateItem>> GetCoordinatesAsync()
        {
            return _database.Table<CoordinateItem>().ToListAsync();
        }

        public Task<CoordinateItem> GetCoordinateItemAsync(int coordinateID)
        {
            return _database.Table<CoordinateItem>().Where(c => c.ID == coordinateID).FirstOrDefaultAsync();
        }

        public Task<int> SaveCoordinateAsync(CoordinateItem coordinate)
        {
            if (coordinate.ID != 0)
            {
                return _database.UpdateAsync(coordinate);
            }
            else
            {
                return _database.InsertAsync(coordinate);
            }
        }

        public Task<int> DeleteCoordinateAsync(CoordinateItem coordinate)
        {
            return _database.DeleteAsync(coordinate);
        }

        public async Task<CoordinateItem> QueryCoordinatesAsync(Location at)
        {
            // this is not optimal, it should be replaced by a custom sqlite function
            List<CoordinateItem> coordinateItems = await _database.Table<CoordinateItem>().Where(c => c.IsOn).ToListAsync();
            foreach (CoordinateItem item in coordinateItems)
            {
                if (LocationDetails.IsInLocation(at, item))
                {
                    return item;
                }
            }

            return null;
            //return await _database.Table<CoordinateItem>().Where(c => c.IsOn &&
            //(Location.CalculateDistance(at, c.Latitude, c.Longitude, DistanceUnits.Kilometers) <= c.Radius)).FirstOrDefaultAsync();
        }

        // history
        public Task<List<HistoryItem>> GetHistoryAsync(string typeFilter = null, string actionFilter = null, string reminderFilter = null, byte responceFilter = 3)
        {

            AsyncTableQuery<HistoryItem> query = _database.Table<HistoryItem>();

            if (typeFilter != null)
            {
                query = query.Where(h => h.TrackedType.Equals(typeFilter));
            }

            if (actionFilter != null)
            {
                query = query.Where(h => h.Action.Equals(actionFilter));
            }

            if (reminderFilter != null)
            {
                query = query.Where(h => h.Reminder.Equals(reminderFilter));
            }

            if (responceFilter != 3)
            {
                query = query.Where(h => h.Responce == responceFilter);
            }

            return query.OrderByDescending(h => h.ID).ToListAsync();
        }

        public Task<HistoryItem> GetFirstHistoryItemAsync()
        {
            return _database.Table<HistoryItem>().OrderByDescending(h => h.ID).FirstOrDefaultAsync();
        }

        public Task<HistoryItem> GetFirstNoResponseHistoryItemAsync()
        {
            return _database.Table<HistoryItem>().Where(h => h.Responce == 0).OrderByDescending(h => h.ID).FirstOrDefaultAsync();
        }

        /*public Task<HistoryItem> GetHistoryItemAsync(HistoryItem historyItem)
        {
            return _database.Table<HistoryItem>().Where(h => h.TrackedID == historyItem.ID).FirstOrDefaultAsync();
        }*/

        public async Task<int> GetNoResponceCountHistoryItemAsync()
        {
            return await _database.Table<HistoryItem>().Where(h => h.Responce == 0).CountAsync();
        }

        public async Task<double[]> GetHistoryBreakdown()
        {
            double historyDidWash = await _database.Table<HistoryItem>().Where(h => h.Responce == 1).CountAsync();
            double historyNoResponce = await _database.Table<HistoryItem>().Where(h => h.Responce == 0).CountAsync();
            double historyDidNotWash = await _database.Table<HistoryItem>().Where(h => h.Responce == 2).CountAsync();

            double historyCountTotal = historyDidWash + historyNoResponce + historyDidNotWash;

            if (historyCountTotal == 0) return null;

            return new double[] {
                historyDidWash / historyCountTotal,
                historyNoResponce / historyCountTotal,
                historyDidNotWash / historyCountTotal };
        }

        public async Task<int> SaveHistoryAsync(HistoryItem history)
        {
            if (history.ID != 0)
            {
                await _database.UpdateAsync(history);
            }
            else
            {
                await _database.InsertAsync(history);
            }
            return history.ID;
        }

        public Task<int> DeleteHistoryAsync(HistoryItem history)
        {
            return _database.DeleteAsync(history);
        }

    }
}
