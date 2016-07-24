using LEO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace InstruLab
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Instrulab());
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Reporting report = new Reporting();
                report.Sendreport("Application has to finish", (Exception)e.ExceptionObject,null);
            }
            catch (Exception ex) { }
            finally {
                Environment.Exit(1);
            }
            
        }
    }
}
