using System;

namespace ActivityTracker
{
    class ActivityEntry
    {
        public string AppName { get; set; }
        public DateTime ActivityStart { get; set; }
        public DateTime ActivityEnd { get; set; }

        public ActivityEntry(string appName, DateTime start)
        {
            AppName = appName;
            ActivityStart = start;
            ActivityEnd = DateTime.Now;
        }

        public const string IdleEntry = "Idle";
    }
}