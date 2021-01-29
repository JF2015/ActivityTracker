using System;

namespace Tracker
{
    public class ActivityDuration
    {
        public string AppName { get; set; }
        public TimeSpan Duration { get; set; }

        public ActivityDuration(string appName)
        {
            AppName = appName;
            Duration = TimeSpan.Zero;
        }
    }
}