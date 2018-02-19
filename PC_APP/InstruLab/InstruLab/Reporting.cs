using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace LEO
{
    class Reporting
    {
        private Exception report_exception= new Exception("nothing happened");

        MailMessage mailMsg;
        string LeoPlatform = "none";
        string LeoFWversion = "none";
        const string SWVersion = "2.13.00.002";



        public void Sendreport(string capt, Exception ex, Device dev, List<String> log, int ErrorNumber)
        {

            //if (SystemInformation.Network)
            //{
            //    try
            //    {
            //        report_exception = ex;
            //        DialogResult res = MessageBox.Show("Something went wrong. \r\n" + ex.GetType() + "\r\n\r\nPlease help us to fix this bug and send us the report. \r\nThere are no personal data inside. Sending takes some time \r\n\r\n Send report?", capt, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //        if (res == DialogResult.Yes)
            //        {
            //            mailMsg = new MailMessage("leo.platform.ctu@gmail.com", "leo.platform.ctu@gmail.com");

            //            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            //            smtpClient.EnableSsl = true;

            //            smtpClient.Timeout = 10000;
            //            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //            smtpClient.UseDefaultCredentials = false;
            //            smtpClient.Credentials = new NetworkCredential("leo.platform.ctu@gmail.com", "ItWasInstrulabInThePast");

            //            mailMsg.Subject = "Report: " + Environment.MachineName + ": " + report_exception.Message;

            //            mailMsg.Body = "";

            //            List<string> body_list = new List<string>();

            //            body_list.Add("Report from LEO \r\n");
            //            body_list.Add("Caption: " + capt + " (" + ErrorNumber.ToString() + ")\r\n");
            //            body_list.Add("\r\n");
            //            body_list.Add("PC name: " + Environment.MachineName + "\r\n");
            //            body_list.Add("User name: " + Environment.UserName + "\r\n");
            //            body_list.Add("Time: " + System.DateTime.Now + "\r\n");
            //            body_list.Add("Win version: " + get_os() + "\r\n");
            //            body_list.Add("Win version: " + Environment.OSVersion + "\r\n");
            //            body_list.Add("MCU: " + get_mcu() + "\r\n");
            //            body_list.Add("\r\n");
            //            if (dev != null)
            //            {
            //                body_list.Add("Platform: " + dev.get_name() + "\r\n");
            //                body_list.Add("MCU: " + dev.get_processor() + " (" + dev.get_port() + ")" + "\r\n");
            //                body_list.Add("FW version: " + dev.systemCfg.FW_Version + "\r\n");
            //            }
            //            else {
            //                body_list.Add("No device connected or fatal error\r\n");
            //            }
            //            body_list.Add("SW version: " + SWVersion + "\r\n");

            //            body_list.Add("\r\n");

            //            body_list.Add("Exception:\r\n" + report_exception + "\r\n");
            //            body_list.Add("\r\n");

            //            body_list.Add("Stack:\r\n" + Environment.StackTrace + "\r\n");

            //            if (log != null)
            //            {
            //                body_list.Add("\r\n");
            //                body_list.Add("MCU communication:\r\n");
            //                foreach (var item in log)
            //                {
            //                    body_list.Add(item);
            //                }

            //                body_list.Add("\r\n");
            //            }

            //            foreach (var item in body_list)
            //            {
            //                mailMsg.Body += item;
            //            }
            //            // mailMsg.Attachments.Add(new Attachment(Log));

            //            smtpClient.Send(mailMsg);
            //            MessageBox.Show("We will work on it.", "Thank you!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        }
            //    }
            //    catch (Exception excp)
            //    {
            //        MessageBox.Show("Unhandled exception. report cannot be send.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    finally
            //    {
            //        Environment.Exit(1);
            //    }

            //}
        }


        public void sendFeedback(string mail, string addres)
        {
            /*
            try
            {
                mailMsg = new MailMessage("leo.platform.ctu@gmail.com", "leo.platform.ctu@gmail.com");

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;

                smtpClient.Timeout = 10000;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("leo.platform.ctu@gmail.com", "ItWasInstrulabInThePast");

                mailMsg.Subject = "Feedback: " + Environment.MachineName;

                mailMsg.Body = "";

                List<string> body_list = new List<string>();

                body_list.Add("Feedback from LEO \r\n");
                body_list.Add("from: " + addres + "\r\n");
                body_list.Add("\r\n");
                body_list.Add("PC name: " + Environment.MachineName + "\r\n");
                body_list.Add("User name: " + Environment.UserName + "\r\n");
                body_list.Add("Time: " + System.DateTime.Now + "\r\n");
                body_list.Add("Win version: " + get_os() + "\r\n");
                body_list.Add("Win version: " + Environment.OSVersion + "\r\n");
                body_list.Add("MCU: " + get_mcu() + "\r\n");
                body_list.Add("\r\n");
                body_list.Add("SW version: " + SWVersion + "\r\n");
                body_list.Add("\r\n");
                body_list.Add(mail);

                foreach (var item in body_list)
                {
                    mailMsg.Body += item;
                }

                smtpClient.Send(mailMsg);
                MessageBox.Show("Thank you for your feedback.", "Thank you", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception excp)
            {
                MessageBox.Show("Unhandled exception. feedback cannot be send.\r\n"+ excp.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private string get_os() {
            string operatingSystem = "";
            try
            {
                //Get Operating system information.
                OperatingSystem os = Environment.OSVersion;
                //Get version information about the os.
                Version vs = os.Version;


                if (os.Platform == PlatformID.Win32Windows)
                {
                    //This is a pre-NT version of Windows
                    switch (vs.Minor)
                    {
                        case 0:
                            operatingSystem = "95";
                            break;
                        case 10:
                            if (vs.Revision.ToString() == "2222A")
                                operatingSystem = "98SE";
                            else
                                operatingSystem = "98";
                            break;
                        case 90:
                            operatingSystem = "Me";
                            break;
                        default:
                            break;
                    }
                }
                else if (os.Platform == PlatformID.Win32NT)
                {
                    switch (vs.Major)
                    {
                        case 3:
                            operatingSystem = "NT 3.51";
                            break;
                        case 4:
                            operatingSystem = "NT 4.0";
                            break;
                        case 5:
                            if (vs.Minor == 0)
                                operatingSystem = "Windows 2000";
                            else
                                operatingSystem = "Windows XP";
                            break;
                        case 6:
                            if (vs.Minor == 0)
                                operatingSystem = "Windows Vista";
                            else if (vs.Minor == 1)
                                operatingSystem = "Windows 7";
                            else
                                operatingSystem = "Windows 8 or Above";
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex) {
                operatingSystem = ex.ToString();
            }
            return operatingSystem;
        }

        private string get_mcu()
        {
            string processor_str = "";
            try
            {

                RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);   //This registry entry contains entry for processor info.
                if (processor_name != null)
                {
                    if (processor_name.GetValue("ProcessorNameString") != null)
                    {
                        processor_str = processor_name.GetValue("ProcessorNameString").ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                processor_str = ex.ToString();
            }
            return processor_str;
        }
    }
}
