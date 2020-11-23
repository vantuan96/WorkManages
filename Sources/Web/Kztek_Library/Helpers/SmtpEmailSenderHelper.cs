using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Kztek_Library.Helpers
{
    public class SmtpEmailSenderHelper
    {
        public string SMTPServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnabledSSL { get; set; }
        public bool UseDefaultCredential { get; set; }
        public bool IsBodyHTML { set; get; }
        public string Signalture { get; set; }

        public bool SendSingleMail(string subject, string mailFrom, string mailTo, string bodyHtml)
        {
            try
            {
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = SMTPServer;
                    smtp.Port = Port;
                    smtp.Credentials = new NetworkCredential(Username, Password);
                    smtp.EnableSsl = EnabledSSL;
                    using (var mail = new MailMessage(mailFrom, mailTo, subject, bodyHtml))
                    {
                        mail.BodyEncoding = Encoding.UTF8;
                        mail.IsBodyHtml = IsBodyHTML;
                        smtp.Send(mail);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Can't send email From: {0} To: {1}!", mailFrom, mailTo) + ex.Message);
            }
        }

        public bool SendMailToList(string subject, string mailFrom, List<string> mailToList, string bodyHtml)
        {
            try
            {
                if (mailToList.Any())
                {
                    if ((mailToList ?? new List<string>()).Any())
                    {
                        using (var smtp = new SmtpClient())
                        {
                            smtp.Port = Port;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.UseDefaultCredentials = UseDefaultCredential;
                            smtp.Credentials = new NetworkCredential(Username, Password);
                            smtp.Host = SMTPServer;

                            using (var mail = new MailMessage())
                            {
                                mail.From = new MailAddress(mailFrom);
                                mail.Subject = subject;
                                mail.Body = bodyHtml;

                                mail.BodyEncoding = Encoding.UTF8;
                                mail.IsBodyHtml = IsBodyHTML;
                                foreach (var toMail in mailToList ?? new List<string>())
                                {
                                    mail.To.Add(new MailAddress(toMail));
                                }
                                smtp.Send(mail);
                                return true;
                            }
                        }
                    }
                }
                return false;
                //throw new Exception("Don't have any email address to send!");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can't send email. Error: " + ex.Message);
            }
        }
    }
}