using System;

namespace DataTransferObjects.Configurations
{
    public class SlackConfigurationsDto
    {
        public string UserToken { get; set; }
        public float MessagesPerSecond { get; set; }

        public void CheckConfigurationData()
        {
            if (string.IsNullOrWhiteSpace(UserToken))
                throw new ArgumentException("UserToken can't be empty.");

            if (MessagesPerSecond == default(float))
                throw new ArgumentException("MessagesPerSecond can't be empty.");
        }
    }
}
