namespace DataTransferObjects.Social
{
    public class UserCheckResultDto
    {
        public string Login { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}
