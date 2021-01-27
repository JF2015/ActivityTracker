using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ActivityTracker
{
    public partial class MainForm : Form
    {
        private Tracker m_tracker;
        
        public MainForm()
        {
            InitializeComponent();
            m_tracker = new Tracker();
            m_tracker.TrackerUpdate += m_trackerUpdated;
            lstViewEntries
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                .SetValue(lstViewEntries, true, null);
        }
        
        private void m_trackerUpdated(object sender, EventArgs e)
        {
            BeginInvoke((Action)(() =>
            {
                var listCombined = new List<ActivityDuration>();
                var activities = m_tracker.Activity;
                TimeSpan busy = TimeSpan.Zero;
                TimeSpan idle = TimeSpan.Zero;
                foreach (var activity in activities)
                {
                    if (!listCombined.Exists(p => p.AppName == activity.AppName))
                        listCombined.Add(new ActivityDuration(activity.AppName));
                    listCombined.First(p => p.AppName == activity.AppName).Duration = listCombined.First(p => p.AppName == activity.AppName).Duration.Add(activity.ActivityEnd - activity.ActivityStart);

                    if (activity.AppName == ActivityEntry.IdleEntry)
                        idle = idle.Add(activity.ActivityEnd - activity.ActivityStart);
                    else
                        busy = busy.Add(activity.ActivityEnd - activity.ActivityStart);
                }

                Text = $@"Activity Tracker - Idle: {idle:hh\:mm\:ss}  Busy: {busy:hh\:mm\:ss}";

                listCombined.Sort((c1, c2) => (c2.Duration.CompareTo(c1.Duration)));
                lstViewEntries.BeginUpdate();
                lstViewEntries.Items.Clear();
                foreach (var entry in listCombined)
                    _ = lstViewEntries.Items.Add(entry.AppName).SubItems.Add(entry.Duration.ToString(@"hh\:mm\:ss"));

                lstViewEntries.EndUpdate();
            }));
        }

        private void lstViewEntries_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            e.Item.BackColor = e.ItemIndex % 2 == 0 ? Color.FromArgb(234, 244, 255) : Color.FromArgb(202, 224, 255);
        }

        private void lstViewEntries_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
