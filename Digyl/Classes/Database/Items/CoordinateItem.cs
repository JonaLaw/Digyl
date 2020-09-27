using SQLite;

namespace Digyl
{
    public class CoordinateItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Radius { get; set; }
        public int Seconds { get; set; }
        public bool OnEnter { get; set; }
        public bool OnExit { get; set; }
        public string OnEnterReminder { get; set; }
        public string OnExitReminder { get; set; }
        public string Details
        {
            get
            {
                string details = $"Latitude: {Latitude}\n" +
                                 $"Longitude: {Longitude}\n" +
                                 $"Radius: {Radius}\n" +
                                 $"Seconds: {Seconds}\n";

                if (OnEnter && OnExit)
                {
                    details += $"On Enter: {OnEnterReminder}\nOn Exit: {OnExitReminder}";
                }
                else if (OnEnter && !OnExit)
                {
                    details += $"On Enter: {OnEnterReminder}\nOn Exit: Nothing";
                }
                else if (!OnEnter && OnExit)
                {
                    details += $"On Enter: Nothing\nOn Exit: {OnExitReminder}";
                }
                else
                {
                    details += $"On Enter: Nothing\nOn Exit: Nothing";
                }

                return details;
            }
        }
        public bool IsOn { get; set; }
    }
}