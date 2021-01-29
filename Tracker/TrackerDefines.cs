namespace Tracker
{
    class TrackerDefines
    {
        public const int IDLE_TIMEOUT_IN_MINUTES = 5; //Time in minutes after which user is tracked as inactive if no mouse or keyboard action happened in the meantime
        public const int FILE_BACKUP_TIME_IN_MINUTES = 5; //perform a backup of the data to a csv file after this many minutes
        public const string ActivityrawFile = "ActivityRaw1_";
        public const string ActivityCombinedFile = "ActivityCombined1_";
        public const string ActivityFileExtension = ".csv";
    }
}