﻿namespace OJS.Common
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    public sealed class MailSender
    {
        private static MailSender instance;

        private static readonly object SyncRoot = new object();

        private readonly SmtpClient mailClient;
        const string SendFrom = "bgcoder.com@gmail.com";
        const string SendFromName = "BGCoder.com";

        private MailSender()
        {
            // TODO: Extract user, address and password as app.config settings
            mailClient = new SmtpClient
            {
                Credentials = new NetworkCredential(SendFrom, "ENTER_PASSWORD_HERE"),
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true,
            };
        }

        public static MailSender Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new MailSender();
                        }
                    }    
                }

                return instance;
            }
        }

        private MailMessage PrepareMessage(string recipient, string subject, string messageBody, IEnumerable<string> bccRecipients)
        {
            var mailTo = new MailAddress(recipient);
            var mailFrom = new MailAddress(SendFrom, SendFromName);
            var message = new MailMessage(mailFrom, mailTo)
            {
                Body = messageBody,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
            };

            if (bccRecipients != null)
            {
                foreach (var bccRecipient in bccRecipients)
                {
                    message.Bcc.Add(bccRecipient);
                }
            }

            return message;
        }

        public void SendMailAsync(string recipient, string subject, string messageBody, IEnumerable<string> bccRecipients = null)
        {
            var message = this.PrepareMessage(recipient, subject, messageBody, bccRecipients);
            mailClient.SendAsync(message, null);
        }

        public void SendMail(string recipient, string subject, string messageBody, IEnumerable<string> bccRecipients = null)
        {
            var message = this.PrepareMessage(recipient, subject, messageBody, bccRecipients);
            mailClient.Send(message);
        }
    }
}
