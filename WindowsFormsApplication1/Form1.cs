using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using BirthdayReminder.Properties;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private struct bdList
        {
            public int DaysTo;
            public string Message;
        }

        public Form1(string[] cmdLine)
        {
            InitializeComponent();
            if (cmdLine != null && cmdLine.Length == 1 && cmdLine[0].ToLowerInvariant() == "/email")
                Settings.Default.SendEmail = true;
            else if (cmdLine.Length != 0)
                MessageBox.Show("BirthdayReminder" + Environment.NewLine + "/email - send to e-mail");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FileStream inputFile;
            try
            {
                inputFile = File.OpenRead(Settings.Default.DatabaseFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening file:" + Settings.Default.DatabaseFile + " : " + ex.Message);
                Application.Exit();
                return;
            }

            var bdListStr = new List<bdList>();
            var c = 0; //inputFile.ReadByte();
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            //read CSV content string by string
            while (c != -1)
            {
                var i = 0;
                c = 0;
                var inputStr = "";
                //while (i < colNum && c != -1)
                while (c != '\r' && c != '\n' && c != -1)
                {
                    c = inputFile.ReadByte();
                    var b = new byte[1];
                    b[0] = (byte)c;
                    if (c == Settings.Default.CSVdelimiter) i++;
                    if (c != -1) inputStr += Encoding.GetEncoding(Settings.Default.CodePage).GetString(b);
                }

                //while (c != '\r' && c != '\n' && c != -1) c = inputFile.ReadByte();
                inputStr = inputStr.Trim().Replace("\r", "").Replace("\n", "");
                if (inputStr == "") continue;

                var cells = inputStr.Split(Settings.Default.CSVdelimiter);
                if (cells.Length >= 2)
                {
                    var bdDay = 0;
                    var bdMonth = 0;
                    var strYear = "";
                    //get date
                    var date = cells[0].Split(Settings.Default.DateDelimiter);
                    for (i = 0; i < 3; i++)
                        if (Settings.Default.DateFormat[i] == 'D')
                            bdDay = int.Parse(date[i]);
                        else if (Settings.Default.DateFormat[i] == 'M')
                            bdMonth = int.Parse(date[i]);
                        else if (Settings.Default.DateFormat[i] == 'Y') strYear = date[i];
                    //calculate remaining days
                    var birthday = new DateTime(now.Year, bdMonth, bdDay);
                    var diff = (int)birthday.Subtract(now).TotalDays;
                    if (diff < 0)
                    {
                        birthday = new DateTime(DateTime.Now.Year + 1, bdMonth, bdDay);
                        diff = (int)birthday.Subtract(now).TotalDays;
                    }

                    if (diff >= 0 && diff <= Settings.Default.WarnPeriod)
                    {
                        var daySign = "";
                        var ageSign = "";

                        var diffLast = diff - diff / 10;
                        //it'll be right till the of 410 days
                        if (diffLast == 0 || diffLast == 5 || diffLast == 6 || diffLast == 7 ||
                            diffLast == 8 || diffLast == 9 || diff > 10 && diff < 15 ||
                            diff > 110 && diff < 115 || diff > 210 && diff < 215 ||
                            diff > 310 && diff < 315) daySign = " дней";
                        else if (diffLast == 1) daySign = " день";
                        else if (diffLast == 2 || diffLast == 3 || diffLast == 4) daySign = " дня";

                        var tmpItem = new bdList
                        {
                            DaysTo = diff
                        };
                        if (diff == 0)
                            tmpItem.Message = "Сегодня ДР: " + cells[1] + " " + cells[0];
                        else
                            tmpItem.Message = "Через " + diff + daySign + " ДР: " + cells[1] + " " + cells[0];
                        //  check if year has no '?'and calculate age
                        if (!strYear.Contains("?"))
                        {
                            var age = DateTime.Now.Year - int.Parse(strYear);

                            diffLast = age - age / 10;
                            //it'll be right till the age of 210
                            if (diffLast == 0 || diffLast == 5 || diffLast == 6 || diffLast == 7 ||
                                diffLast == 8 || diffLast == 9 || age > 10 && age < 15 ||
                                age > 110 && age < 115) ageSign = " лет";
                            else if (diffLast == 1) ageSign = " год";
                            else if (diffLast == 2 || diffLast == 3 || diffLast == 4) ageSign = " года";

                            tmpItem.Message += "(" + age + ageSign + ")";
                        }

                        tmpItem.Message += "."+ Environment.NewLine;
                        var n = 0;
                        for (n = 0; n < bdListStr.Count; n++)
                            if (bdListStr[n].DaysTo > tmpItem.DaysTo)
                                break;
                        if (n < bdListStr.Count)
                            bdListStr.Insert(n, tmpItem);
                        else bdListStr.Add(tmpItem);
                    }
                }
            }

            inputFile.Close();
            if (bdListStr.Count > 0)
            {
                if (Settings.Default.SendEmail == false)
                {
                    textBox_bdList.Font = new Font(textBox_bdList.Font.FontFamily, Settings.Default.FontSize);
                    for (var i = 0; i < bdListStr.Count; i++) textBox_bdList.Text += bdListStr[i].Message;
                }
                else if (Settings.Default.SendEmail)
                {
                    var tmp = "";
                    for (var i = 0; i < bdListStr.Count; i++) tmp += bdListStr[i].Message;
                    SendEmail(tmp);
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
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
            var mail = new MailMessage();
            mail.From = new MailAddress(Settings.Default.SmtpLogin);
            mail.Subject = Settings.Default.EmailSubject;
            mail.Body = mailMessage;
            mail.IsBodyHtml = false; // Can set to false, if you are sending pure text.
            var smtp = new SmtpClient(Settings.Default.SmtpAddress, Settings.Default.SmtpPort);
            smtp.Credentials = new NetworkCredential(Settings.Default.SmtpLogin, Settings.Default.SmtpPassword);
            smtp.EnableSsl = Settings.Default.SmtpSSL;

            var mailList = Settings.Default.EmailTo.Split(Settings.Default.CSVdelimiter);

            for (var i = 0; i < mailList.Length; i++)
                if (mailList[i] == "" || mailList[i].Length < 3)
                {
                    ;
                }
                else
                {
                    mail.To.Add(mailList[i]);
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
}
