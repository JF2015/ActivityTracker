using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Tracker
{
    public class Tracker
    {
        private readonly List<ActivityEntry> m_activity;
        public event EventHandler<EventArgs> TrackerUpdate;
        private readonly object m_lock = new();
        
        private DateTime m_lastFileBackupTime;
        private readonly CancellationTokenSource m_stopToken; 

        public Tracker()
        {
            m_lastFileBackupTime = DateTime.Now;
            m_stopToken = new CancellationTokenSource();
            m_activity = new List<ActivityEntry> { new(ActivityEntry.IdleEntry, DateTime.Now) };
            readFromBackupFile();
            var workerThread = new Thread(worker) { IsBackground = true, Name = "ActivityTrackerThread" };
            workerThread.Start();
        }

        public void Stop()
        {
            m_stopToken.Cancel();
            writeToBackupFile();
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

        private void worker()
        {
            var span = new TimeSpan(0, 0, 5);
            while (true)
            {
                if (m_stopToken.IsCancellationRequested)
                    break;
                Task.Delay((int) span.TotalMilliseconds, m_stopToken.Token);
                if (m_stopToken.IsCancellationRequested)
                    break;
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

                if (!((DateTime.Now - m_lastFileBackupTime).TotalMinutes > TrackerDefines.FILE_BACKUP_TIME_IN_MINUTES))
                    continue;

                bool resetNextDay = m_lastFileBackupTime.Date != DateTime.Now.Date;
                m_lastFileBackupTime = DateTime.Now;
                writeToBackupFile();
                if (resetNextDay)
                {
                    lock (m_lock)
                    {
                        m_activity.Clear();
                        m_activity.Add(new(ActivityEntry.IdleEntry, DateTime.Now));
                    }
                }
            }
        }

        private void writeToBackupFile()
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
            var today = DateTime.Now.ToString("yyyy-MM-d");
            File.WriteAllLines(Path.Combine(tempPath, TrackerDefines.ActivityCombinedFile + today + TrackerDefines.ActivityFileExtension), lines);

            lines = Activity.Select(entry => entry.AppName + ";" + entry.ActivityStart.ToString(@"HH\:mm\:ss") + ";" + entry.ActivityEnd.ToString(@"HH\:mm\:ss")).ToList();
            
            File.WriteAllLines(Path.Combine(tempPath, TrackerDefines.ActivityrawFile + today + TrackerDefines.ActivityFileExtension), lines);
        }

        private void readFromBackupFile()
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "ActivityTracker");
                var today = DateTime.Now.ToString("yyyy-MM-d");
                string fileName = Path.Combine(tempPath, TrackerDefines.ActivityrawFile + today + TrackerDefines.ActivityFileExtension);
                if (!File.Exists(fileName)) 
                    return;
                var lines = File.ReadAllLines(fileName);
                lock (m_lock)
                {
                    foreach (var line in lines)
                    {
                        var res = line.Split(';');
                        m_activity.Add(new ActivityEntry(res[0], Convert.ToDateTime(res[1]), Convert.ToDateTime(res[2])));
                    }
                }
            }
            catch
            {
                //Obviously cannot read the file
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
                    ret = ret.Substring(0, ret.IndexOf("/", StringComparison.Ordinal));
                return ret;
            }
            catch (Exception)
            {
                //ignore when that fails
            }

            return "Unknown Website";
        }

        private static bool checkInteraction()
        {
            WinAPI.LASTINPUTINFO info = new WinAPI.LASTINPUTINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            if (!WinAPI.GetLastInputInfo(ref info))
                return false;

            return (((Environment.TickCount & int.MaxValue) - (info.dwTime & int.MaxValue)) & int.MaxValue) / 1000.0 / 60.0 < TrackerDefines.IDLE_TIMEOUT_IN_MINUTES;
        }

        private static string getActiveApplicationName()
        {
            IntPtr hWnd = WinAPI.GetForegroundWindow();
            _ = WinAPI.GetWindowThreadProcessId(hWnd, out var procId);
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
            return appName.ToLowerInvariant() switch
            {
                "devenv" => "Visual Studio",
                "chrome" => "Chrome",
                "outlook" => "Outlook",
                "lync" => "Skype",
                "mstsc" => "Remote Desktop",
                "winword" => "Word",
                "explorer" => "Explorer",
                "notepad" => "Notepad",
                "acrord32" => "Acrobat Reader",
                "winmergeu" => "WinMerge",
                "excel" => "Excel",
                _ => appName
            };
        }
    }
}
