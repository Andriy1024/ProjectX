using System;

namespace ProjectX.Email
{
    public class EmailOptions
    {
        public class SendGridOptions
        {
            public string API_KEY { get; set; }
        }

        public class SMPTOptions
        {
            public string Password { get; set; }
            public string Host { get; set; }
            public int? Port { get; set; }
        }

        public bool EnableEmailSender { get; set; }

        public string FromEmail { get; set; }

        public string FromName { get; set; }

        public SendGridOptions SendGrid { get; set; }

        public SMPTOptions SMTP { get; set; }

        public static void Validate(EmailOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("Redis options.");

            if (!options.EnableEmailSender)
                return;

            if (string.IsNullOrEmpty(options.FromEmail))
                throw new ArgumentNullException("FromEmail is empty.");

            if (options.SendGrid != null)
            {
                if (string.IsNullOrEmpty(options.SendGrid.API_KEY))
                    throw new ArgumentNullException("SendGrid.API_KEY is empty.");
            }
            else if (options.SMTP != null)
            {
                if (string.IsNullOrEmpty(options.SMTP.Password))
                    throw new ArgumentNullException("SMTP.Password is empty.");

                if (string.IsNullOrEmpty(options.SMTP.Host))
                    throw new ArgumentNullException("SMTP.HOST is empty");

                if (!options.SMTP.Port.HasValue)
                    throw new ArgumentNullException("SMTP.Port is empty");
            }
            else
            {
                throw new Exception("If 'EnableEmailSender' is true, there should be the configuration of the email provider (SMTP or SendGrid).");
            }
        }
    }
}
