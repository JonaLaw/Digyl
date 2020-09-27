using SQLite;
using System;
using System.Globalization;

namespace Digyl
{
    public class PlaceTypeItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [Indexed]
        public int ParentPlaceTypeID { get; set; }
        public string Name { get; set; }
        public string NameReadable
        {
            get
            {
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Name.Replace('_', ' '));
            }
        }
        public bool OnEnter { get; set; }
        public bool OnExit { get; set; }
        public string OnEnterReminder { get; set; }
        public string OnExitReminder { get; set; }
        public string Details
        {
            get
            {
                if (OnEnter && OnExit) return $"On Enter: {OnEnterReminder}\nOn Exit: {OnExitReminder}";
                if (OnEnter && !OnExit) return $"On Enter: {OnEnterReminder}\nOn Exit: Nothing";
                if (!OnEnter && OnExit) return $"On Enter: Nothing\nOn Exit: {OnExitReminder}";
                return $"On Enter: Nothing\nOn Exit: Nothing";
            }
        }
        public bool IsOn { get; set; }
    }

    public class ParentPlaceTypeItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsOn { get; set; }
    }
}