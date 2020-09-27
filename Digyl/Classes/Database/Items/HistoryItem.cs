using SQLite;
using System;
using Xamarin.Forms;
using Humanizer;

namespace Digyl
{
    public class HistoryItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [Indexed]
        public int OriginID { get; set; }
        public string TrackedType { get; set; }

        public string Name { get; set; }
        public string Action { get; set; }
        public string Reminder { get; set; }
        public byte Responce { get; set; } = 0;
        public string ResponceReadable
        {
            get
            {
                if (Responce == 0)
                    return $"Did you {Reminder}?";
                else if (Responce == 1)
                    return $"Did {Reminder}.";
                return $"Did not {Reminder}.";

            }
        }
        public Color BoxColor
        {
            get
            {
                if (Responce == 0)
                    return Color.FromHex("#FFFF00");
                else if (Responce == 1)
                    return Color.FromHex("#76FF03");
                return Color.FromHex("#FF5252");
            }
        }

        public DateTime LoggedTime { get; set; } = DateTime.Now;
        public string TimeReadable
        {
            get
            {
                string time = LoggedTime.Humanize(utcDate: false);
                if (string.IsNullOrEmpty(time))
                {
                    return string.Empty;
                }
                return char.ToUpper(time[0]) + time.Substring(1);
            }
        }
        public string TimeDetails { get { return LoggedTime.ToString(); } }
    }
}