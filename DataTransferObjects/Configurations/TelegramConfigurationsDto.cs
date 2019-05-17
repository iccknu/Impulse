using System;

namespace DataTransferObjects.Configurations
{
    public class TelegramConfigurationsDto
    {
        public int ApiId { get; set; }
        public string ApiHash { get; set; }
        public string PhoneNumber { get; set; }
        public float MessagesPerSecond { get; set; }

        public void CheckConfigurationData()
        {
            if (ApiId == default(int))
                throw new ArgumentException("ApiId can't be empty.");

            if (string.IsNullOrWhiteSpace(ApiHash))
                throw new ArgumentException("ApiHash can't be empty.");

            if (string.IsNullOrWhiteSpace(PhoneNumber))
                throw new ArgumentException("PhoneNumber can't be empty.");

            if (MessagesPerSecond == default(float))
                throw new ArgumentException("MessagesPerSecond can't be empty.");
        }
    }
}
