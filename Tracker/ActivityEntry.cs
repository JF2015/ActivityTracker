using System;

namespace Tracker
{
    public class ActivityEntry
    {
        public string AppName { get; set; }
        public DateTime ActivityStart { get; set; }
        public DateTime ActivityEnd { get; set; }

        public ActivityEntry(string appName, DateTime start) : this(appName, start, DateTime.Now)
        {}

        public ActivityEntry(string appName, DateTime start, DateTime end)
        {
            AppName = appName;
            ActivityStart = start;
            ActivityEnd = end;
        }

        public const string IdleEntry = "Idle";
    }
}