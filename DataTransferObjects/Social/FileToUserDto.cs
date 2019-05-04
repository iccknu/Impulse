using Enums;

namespace DataTransferObjects.Social
{
    public class FileToUserDto
    {
        public string EmailOrUserNumber { get; set; }
        public string Caption { get; set; }
        public string MimeType { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public Priority Priority { get; set; }
    }
}
