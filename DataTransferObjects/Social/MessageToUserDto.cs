using Enums;

namespace DataTransferObjects.Social
{
    public class MessageToUserDto
    {
        public string EmailOrUserNumber { get; set; }
        public string Message { get; set; }
        public Priority Priority { get; set; }
    }
}
