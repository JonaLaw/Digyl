namespace Digyl
{
    public static class Constants
    {
        public const string apiKey = "AIzaSyCfg_ouJNQFJFLerC8XhhbXAjq0jU3D25E";

        public const int SERVICE_RUNNING_NOTIFICATION_ID = 123;
        public const string NOTIFICATION_CHANNEL_ID = "com.company.app.channel";

        public const string trackingPref = "tracking_key";
        public const string coordinateTrackingPref = "coordinates_key";
        public const string placeTrackingPef = "placeTracking_key";
        public const string manualHistoryPef = "manualHistory_key";

        public const string locationUpdateMinTimePref = "locationUpdateMinTime_key";
        public const string locationUpdateDistancePref = "locationUpdateDistance_key";
        public const string placeTypeRadiusPef = "placeTypeRadius_key";
        public const string placeTypeLeaveRadiusPef = "placeTypeLeaveRadius_key";

        public const string coordinateName = "Coordinate";
        public const string placeTypeName = "Place Type";

        public const string actionEnter = "Entered";
        public const string actionExit = "Exited";

        public const string washHands = "Wash Hands";
        public const string wearMask = "Wear Mask";
        public const string washHandsAndWearMask = washHands + " and " + wearMask;

        public const string broadcastIntentExtraYes = "response_yes_key";
        public const string broadcastIntentExtraNo = "response_no_key";
    }
}
