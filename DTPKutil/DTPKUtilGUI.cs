using System;
using System.IO;
using System.Windows.Forms;

namespace DTPKutil
{
    static class DTPKUtilGUI
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainView());
        }

    }
}
