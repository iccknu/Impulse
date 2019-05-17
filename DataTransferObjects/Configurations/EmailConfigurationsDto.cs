using System;

namespace DataTransferObjects.Configurations
{
    public class EmailConfigurationsDto
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public float MessagesPerSecond { get; set; }

        public void CheckConfigurationData()
        {
            if (string.IsNullOrWhiteSpace(SmtpServer))
                throw new ArgumentException("SmtpServer can't be empty.");

            if (SmtpPort == default(int))
                throw new ArgumentException("SmtpPort can't be empty.");

            if (string.IsNullOrWhiteSpace(UserName))
                throw new ArgumentException("UserName can't be empty.");

            if (string.IsNullOrWhiteSpace(Password))
                throw new ArgumentException("Password can't be empty.");

            if (string.IsNullOrWhiteSpace(Email))
                throw new ArgumentException("Email can't be empty.");

            if (MessagesPerSecond == default(float))
                throw new ArgumentException("MessagesPerSecond can't be empty.");
        }
    }
}
