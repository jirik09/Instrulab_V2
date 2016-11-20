using LEO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace LEO
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static Instrulab leoApp;
        static ApplicationContext ctx;
        
        [STAThread]
        static void Main()
        {
            
            

            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Set the unhandled exception mode to force all Windows Forms errors
            // to go through our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += new
            UnhandledExceptionEventHandler(UnhandledExceptionTrapper);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            leoApp = new Instrulab();
            ctx = new ApplicationContext(leoApp);

            Application.Run(ctx);

            /*
              
    Application.Run(new Form1());
             */
        }
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                Device dd = leoApp.getDevice();
                Reporting report = new Reporting();
                if (dd == null)
                {
                    report.Sendreport("Application has to finish UI error", e.Exception, dd, null, 138351);
                }
                else {
                    report.Sendreport("Application has to finish UI error", e.Exception, dd, leoApp.getDevice().getLogger(), 138352);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("It is really bad\r\nno chance to recover anything\r\nSorry!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Environment.Exit(1);
            }
        }

        static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Device dd = leoApp.getDevice();
                Reporting report = new Reporting();
                if (dd == null)
                {
                    report.Sendreport("Application has to finish", (Exception)e.ExceptionObject, leoApp.getDevice(), null, 138453);
                }
                else
                {
                    report.Sendreport("Application has to finish", (Exception)e.ExceptionObject, leoApp.getDevice(), leoApp.getDevice().getLogger(), 138454);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("It is really bad\r\nno chance to recover anything\r\nSorry!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally {
                Environment.Exit(1);
            }
            
        }
    }
}
