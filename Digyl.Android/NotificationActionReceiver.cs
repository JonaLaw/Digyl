using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Util;

namespace Digyl.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class NotificationActionYesReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            int id = intent.GetIntExtra(Constants.broadcastIntentExtraYes, 0);
            Log.Debug("NotificationActionYesReceiver", $"Received id of {id}");
            _ = App.LocationUpdateHandler.UpdateLastHistoryItem(id, true);
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class NotificationActionNoReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            int id = intent.GetIntExtra(Constants.broadcastIntentExtraNo, 0);
            Log.Debug("NotificationActionNoReceiver", $"Received id of {id}");
            _ = App.LocationUpdateHandler.UpdateLastHistoryItem(id, false);
        }
    }
}