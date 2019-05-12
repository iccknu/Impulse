namespace DataTransferObjects.Configurations
{
    public class TelegramConfigurationsDto
    {
        public int ApiId { get; set; }
        public string ApiHash { get; set; }
        public string PhoneNumber { get; set; }
        public float MessagesPerSecond { get; set; }
    }
}
