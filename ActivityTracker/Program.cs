using System;
using System.Threading;
using System.Windows.Forms;

namespace ActivityTracker
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            setCurrentThreadCulture();
            Application.Run(new MainForm());
        }

        private static void setCurrentThreadCulture()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US")
            {
                NumberFormat = { CurrencyDecimalSeparator = ".", NumberDecimalSeparator = ".", NumberGroupSeparator = "," }
            };
        }
    }
}
