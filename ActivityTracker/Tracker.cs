using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;

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
    
    class ActivityDuration
    {
        public string AppName { get; set; }
        public TimeSpan Duration { get; set; }

        public ActivityDuration(string appName)
        {
            AppName = appName;
            Duration = TimeSpan.Zero;
        }
    }

    class Tracker
    {
        private readonly List<ActivityEntry> m_activity;
        public event EventHandler<EventArgs> TrackerUpdate;
        private object m_lock = new();
        private const int IDLE_TIMEOUT_IN_MINUTES = 2;
        private const int FILE_BACKUP_TIME_IN_MINUTES = 5;
        private DateTime m_lastFileBackupTime;
        
        public Tracker()
        {
            m_lastFileBackupTime = DateTime.Now;
            m_activity = new List<ActivityEntry> {new(ActivityEntry.IdleEntry, DateTime.Now)};
            var workerThread = new Thread(worker) {IsBackground = true, Name = "ActivityTrackerThread"};
            workerThread.Start();
        }

        public List<ActivityEntry> Activity
        {
            get
            {
                List<ActivityEntry> activities;
                lock (m_lock)
                {
                    activities = new List<ActivityEntry>(m_activity);
                }

                return activities;
            }
        }

        private static string getChromeUrl(Process proc)
        {
            try
            {
                // the chrome process must have a window
                if (proc.MainWindowHandle == IntPtr.Zero)
                    return "Unknown Website";
                
                AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);
                
                //Searching by name must be localized and is rather slow as it searches the whole tree
                //var SearchBar = root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Adress und Suchleiste"));

                AutomationElement elmUrlBar;
                try
                {
                    // walking path found using inspect.exe (Windows SDK)
                    var elm1 = root.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
                    if (elm1 == null)
                        return "Unknown Website";
                    var elm2 = TreeWalker.RawViewWalker.GetLastChild(elm1);
                    elmUrlBar = elm2.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                }
                catch
                {
                    // Chrome has probably changed something, and above walking needs to be modified. :(
                    // put an assertion here or something to make sure you don't miss it
                    return "Unknown Website";
                }

                // make sure it's valid
                if (elmUrlBar == null)
                    return "Unknown Website";

                // elmUrlBar is now the URL bar element. we have to make sure that it's out of keyboard focus if we want to get a valid URL
                if ((bool)elmUrlBar.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty))
                    return "Unknown Website";

                // there might not be a valid pattern to use, so we have to make sure we have one
                AutomationPattern[] patterns = elmUrlBar.GetSupportedPatterns();
                if (patterns.Length != 1)
                    return "Unknown Website";
                string ret = "";
                try
                {
                    ret = ((ValuePattern)elmUrlBar.GetCurrentPattern(patterns[0])).Current.Value;
                }
                catch
                {
                    // ignored
                }

                if (ret == "")
                    return "Unknown Website";
                ret = ret.Replace("http://", "").Replace("https://", "");
                if (ret.Contains("/"))
                    ret = ret.Substring(0, ret.IndexOf("/"));
                return ret;
            }
            catch (Exception)
            {
                //ignore when that fails
            }

            return "Unknown Website";
        }

        private void worker()
        {
            var span = new TimeSpan(0, 0, 5);
            while (true)
            {
                Thread.Sleep((int)span.TotalMilliseconds);
                var entryText = checkInteraction() ? getActiveApplicationName() : ActivityEntry.IdleEntry;
                lock (m_lock)
                {
                    var lastEntry = m_activity.Last();
                    if (lastEntry.AppName == entryText)
                        lastEntry.ActivityEnd = DateTime.Now;
                    else
                    {
                        lastEntry.ActivityEnd = DateTime.Now;
                        m_activity.Add(new ActivityEntry(entryText, DateTime.Now));
                    }
                }

                TrackerUpdate?.Invoke(this, new EventArgs());

                if ((DateTime.Now - m_lastFileBackupTime).TotalMinutes > FILE_BACKUP_TIME_IN_MINUTES)
                {
                    m_lastFileBackupTime = DateTime.Now;
                    backupToFile();
                }
            }
        }

        private void backupToFile()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "ActivityTracker");
            var listCombined = new List<ActivityDuration>();
            lock (m_lock)
            {
                foreach (var activity in m_activity)
                {
                    if (!listCombined.Exists(p => p.AppName == activity.AppName))
                        listCombined.Add(new ActivityDuration(activity.AppName));
                    listCombined.First(p => p.AppName == activity.AppName).Duration = listCombined.First(p => p.AppName == activity.AppName).Duration.Add(activity.ActivityEnd - activity.ActivityStart);
                }
            }

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);
            var lines = listCombined.Select(entry => entry.AppName + ";" + entry.Duration.ToString(@"hh\:mm\:ss")).ToList();
            File.WriteAllLines(Path.Combine(tempPath,"Activity.csv"), lines);
        }

        private bool checkInteraction()
        {
            WinAPI.LASTINPUTINFO info = new WinAPI.LASTINPUTINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            if (!WinAPI.GetLastInputInfo(ref info))
                return false;
            
            return (((Environment.TickCount & int.MaxValue) - (info.dwTime & int.MaxValue)) & int.MaxValue) / 1000.0 / 60.0 < IDLE_TIMEOUT_IN_MINUTES;
        }

        private static string getActiveApplicationName()
        {
            IntPtr hWnd = WinAPI.GetForegroundWindow();
            WinAPI.GetWindowThreadProcessId(hWnd, out var procId);
            var proc = Process.GetProcessById((int)procId);
            string name;
            try
            {
                name = proc.MainModule == null ? "Unknown" : proc.MainModule.FileName;
            }
            catch
            {
                name = proc.ProcessName;
            }

            name = toNiceName(Path.GetFileNameWithoutExtension(name));

            if (name == "Chrome")
                name = getChromeUrl(proc);
            
            return name;
        }

        private static string toNiceName(string appName)
        {
            switch (appName.ToLowerInvariant())
            {
                case "devenv":
                    return "Visual Studio";
                case "chrome":
                    return "Chrome";
                case "outlook":
                    return "Outlook";
                case "lync":
                    return "Skype";
                case "mstsc":
                    return "Remote Desktop";
            }

            return appName;
        }
    }
}
