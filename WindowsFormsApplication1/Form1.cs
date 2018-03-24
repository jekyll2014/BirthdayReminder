using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FileStream inputFile;
            try
            {
                inputFile = File.OpenRead(BirthdayReminder.Properties.Settings.Default.DatabaseFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening file:" + BirthdayReminder.Properties.Settings.Default.DatabaseFile + " : " + ex.Message);
                Application.Exit();
                return;
            }
            string inputStr = "";
            List<string> bdListStr = new List<string>();
            int c = 0;//inputFile.ReadByte();

            //read CSV content string by string
            while (c != -1)
            {
                int i = 0;
                c = 0;
                inputStr = "";
                //while (i < colNum && c != -1)
                while (c != '\r' && c != '\n' && c != -1)
                {
                    c = inputFile.ReadByte();
                    byte[] b = new byte[1];
                    b[0] = (byte)c;
                    if (c == BirthdayReminder.Properties.Settings.Default.CSVdelimiter) i++;
                    if (c != -1) inputStr += Encoding.GetEncoding(BirthdayReminder.Properties.Settings.Default.CodePage).GetString(b);
                }
                //while (c != '\r' && c != '\n' && c != -1) c = inputFile.ReadByte();
                inputStr = inputStr.Trim().Replace("\r", "").Replace("\n", "");
                if (inputStr != "")
                {
                    string[] cells = inputStr.Split(BirthdayReminder.Properties.Settings.Default.CSVdelimiter);
                    if (cells.Length >= 2)
                    {
                        int bd_day = 0;
                        int bd_month = 0;
                        int age = 0;
                        string str_year = "";
                        //get date
                        string[] date = cells[0].Split(BirthdayReminder.Properties.Settings.Default.DateDelimiter);
                        for (i = 0; i < 3; i++)
                        {
                            if (BirthdayReminder.Properties.Settings.Default.DateFormat[i] == 'D')
                            {
                                bd_day = int.Parse(date[i]);
                            }
                            else if (BirthdayReminder.Properties.Settings.Default.DateFormat[i] == 'M')
                            {
                                bd_month = int.Parse(date[i]);
                            }
                            else if (BirthdayReminder.Properties.Settings.Default.DateFormat[i] == 'Y')
                            {
                                str_year = date[i];
                            }
                        }
                        //calculate remaining days
                        DateTime now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        DateTime birthday = new DateTime(DateTime.Now.Year, bd_month, bd_day);
                        double diff = birthday.Subtract(now).TotalDays;
                        if (diff < 0)
                        {
                            birthday = new DateTime(DateTime.Now.Year + 1, bd_month, bd_day);
                            diff = birthday.Subtract(now).TotalDays;
                        }
                        if (diff >= 0 && diff <= BirthdayReminder.Properties.Settings.Default.WarnPeriod)
                        {
                            string daySign = "";
                            string ageSign = "";

                            string tmp = diff.ToString();
                            tmp = tmp.Substring(tmp.Length - 1);
                            if (tmp == "0" || tmp == "5" || tmp == "6" || tmp == "7" || tmp == "8" || tmp == "9" || (diff > 10 && diff < 15) || (diff > 110 && diff < 115) || (diff > 210 && diff < 215) || (diff > 310 && diff < 315)) daySign = " дней";
                            else if (tmp == "1") daySign = " день";
                            else if (tmp == "2" || tmp == "3" || tmp == "4") daySign = " дня";

                            if (diff == 0)
                            {
                                bdListStr.Add("Сегодня ДР: " + cells[1] + " " + cells[0]);
                            }
                            else
                            {
                                bdListStr.Add("Через " + diff.ToString() + daySign + " ДР: " + cells[1] + " " + cells[0]);
                            }
                            //  check if year has no '?'and calculate age
                            if (!str_year.Contains("?"))
                            {
                                age = DateTime.Now.Year - int.Parse(str_year);

                                tmp = age.ToString();
                                tmp = tmp.Substring(tmp.Length - 1);
                                if (tmp == "0" || tmp == "5" || tmp == "6" || tmp == "7" || tmp == "8" || tmp == "9" || (diff > 10 && diff < 15) || (diff > 110 && diff < 115) || (diff > 210 && diff < 215) || (diff > 310 && diff < 315)) ageSign = " лет";
                                else if (tmp == "1") ageSign = " год";
                                else if (tmp == "2" || tmp == "3" || tmp == "4") ageSign = " года";

                                bdListStr[bdListStr.Count - 1] += " (" + age.ToString() + ageSign + ")";
                            }
                            bdListStr[bdListStr.Count - 1] += ".\r\n";
                        }
                    }
                }
            }
            inputFile.Close();            
            if (bdListStr.Count>0)
            {
                //sort lines
                bdListStr.Sort();
                if (BirthdayReminder.Properties.Settings.Default.SendEmail == false)
                {
                    textBox_bdList.Font = new Font(textBox_bdList.Font.FontFamily, BirthdayReminder.Properties.Settings.Default.FontSize);
                    for (int i = 0; i < bdListStr.Count; i++) textBox_bdList.Text += bdListStr[i];
                }
                else if (BirthdayReminder.Properties.Settings.Default.SendEmail == true)
                {
                    string tmp = "";
                    for (int i = 0; i < bdListStr.Count; i++) tmp += bdListStr[i];
                    SendEmail(tmp);
                    Application.Exit();
                }
            }
            else Application.Exit();
        }

        public void SendEmail(string mailMessage)
        {
            // Body of the mail
            /*string body = "<div style='border: medium solid grey; width: 500px; height: 266px;font-family: arial,sans-serif; font-size: 17px;'>";
            body += "<h3 style='background-color: blueviolet; margin-top:0px;'>Aspen Reporting Tool</h3>";
            body += "<br />";
            body += "Dear " + userName + ",";
            body += "<br />";
            body += "<p>";
            body += "Thank you for registering </p>";
            body += "<p><a href='" + sURL + "'>Click Here</a>To finalize the registration process</p>";
            body += " <br />";
            body += "Thanks,";
            body += "<br />";
            body += "<b>The Team</b>";
            body += "</div>";*/

            // this is done using  using System.Net.Mail; & using System.Net; 
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(BirthdayReminder.Properties.Settings.Default.SmtpLogin);
            mail.To.Add(BirthdayReminder.Properties.Settings.Default.EmailTo);
            mail.Subject = BirthdayReminder.Properties.Settings.Default.EmailSubject;
            mail.Body = mailMessage;
            mail.IsBodyHtml = false; // Can set to false, if you are sending pure text.

            SmtpClient smtp = new SmtpClient(BirthdayReminder.Properties.Settings.Default.SmtpAddress, BirthdayReminder.Properties.Settings.Default.SmtpPort);
            smtp.Credentials = new NetworkCredential(BirthdayReminder.Properties.Settings.Default.SmtpLogin, BirthdayReminder.Properties.Settings.Default.SmtpPassword);
            smtp.EnableSsl = BirthdayReminder.Properties.Settings.Default.SmtpSSL;
            try
            {
                smtp.Send(mail);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Error sending mail: " + Ex.Message);
            }

        }
    }
}
