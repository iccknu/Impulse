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
    }
}
