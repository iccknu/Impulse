namespace DataTransferObjects.Social
{
    public class UserCheckResult
    {
        public string EmailOrUserNumber { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}
