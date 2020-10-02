# Digyl
An application for Android and soon iOS that allows users to set handwashing and/or mask wearing reminders for entering or exiting certain locations.  

[Google Drive Android APK](https://drive.google.com/file/d/1wBiDIg5pfoEf0F44Cr3g0o3Hef1MXO6p/view?usp=sharing)

## Features
- Responsive Home Page
- Customizable coordinates list
- Semi-Customizable place type list
- Background location tracking
- Interactive notifications for reminders
- Editable history list of all recorded reminders
- Tweakable settings for location tracking accuracy


## Home Page
The home page will update its various sections based on user response.  
Currently, for updates made via the background service or notification interaction to appear, a page refresh is required.  

If the background location tracking is turned on, it should persist throughout all possible application states except for a system restart.  
Its stability has not been fully tested. If it crashes randomly, try and leave the app open in the background and not close it.  

<p align="left">
 <img src="/Screenshots/home_empty.png" width="250">
 <img src="/Screenshots/home_filled.png" width="250">
 <img src="/Screenshots/notification.png" width="250">
 </p>

## Coordinates Pages
Contains the ability to create, view, edit, and save coordinates. 

The most important part of a coordinates entry is its radius. GPS accuracy on smartphones is fickle and at best is ~10 meters accurate. This means that a low coordinate radius can cause repeated exit and enter reminders when stationary inside the location. I recommend starting off with a larger radius such as 20 meters and reducing as needed. 

Note that due to some inefficiencies in the current program, extraordinarily large coordinate lists can result in poor backgrounding preformance.
<p align="left">
 <img src="/Screenshots/coordinates.png" width="300">
 <img src="/Screenshots/coordinate_add.png" width="300">
 </p>

## Place Type Pages
Contains an orginized list of [Google Place Types](https://developers.google.com/places/web-service/supported_types).
The on enter and on exit reminders for each place type can be changed through pressing on "*more*" and going from there.

The [Place type detection](/Digyl/Classes/LocationDetails.cs) works as follows:
1. Receive a location update or a manual test request
2. Make a [Nearby Search Request](https://developers.google.com/places/web-service/search#PlaceSearchRequests) to get all nearby place types around the location
3. Extract all the place types found in the responce and order them by number of occurrences
4. Search our on-device database for place types that are enabled
5. Return the first one that is found

The search radius for nearby place types can be changed in the apps settings page.  
There is also a setting for a leave radius which will actually trigger another nearby search request to check if the place type is no longer found. The intented effect of increasing this is to reduce the amount of API requests if the user moves around in a large place. This is effectivly overrided by having the location update distance setting be higher than the leave radius.

Note that other overlapping or closeby place types may take priority over the actual place type the device is located in. Addressing this would require exploring using altitude and finer methods of getting device location.

<p align="left">
 <img src="/Screenshots/place_type_1.png" width="300">
 <img src="/Screenshots/place_type_2.png" width="300">
</p>

## History Page
An list of all reminders and their information displayed in text. The filter contains variables for location type, action, reminder, and responce. 

<img src="/Screenshots/history.png" width="300">

## Settings Page
Contains some settings and testing functions.

The manual history buttons allows for the manual addition of history via each individual coordinate or place type item.

Testing detection only works for individual items that are enabled, the entire category doesn't need to be enabled.

**Location Update MinTime:** Changes [requestLocationUpdates(minTimeMs)](https://developer.android.com/reference/android/location/LocationManager#requestLocationUpdates(long,%20float,%20android.location.Criteria,%20android.location.LocationListener,%20android.os.Looper)), which is the minimum time interval between location updates in milliseconds. Leaving this at 0 is best for our application as we don't care about location updates while staying still.

**Location Update MinDistance:** Changes [requestLocationUpdates(minDistanceM)](https://developer.android.com/reference/android/location/LocationManager#requestLocationUpdates(long,%20float,%20android.location.Criteria,%20android.location.LocationListener,%20android.os.Looper)), which is the minimum distance between location updates in meters. Low GPS accuracy can cause location updates even while staying still so best leave this at greater than or equal to 10m.


**Place Type (Leave) Radius:** See the **Place Type Page** section above.

**Reset Databases:** Clears the coordinates and history database, remakes the place types database. If the place types page doesn't look like the screenshot shown earlier, do this.

<img src="/Screenshots/settings.png" width="300">
