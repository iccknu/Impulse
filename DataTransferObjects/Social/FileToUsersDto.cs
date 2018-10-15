using Enums;
using System.Collections.Generic;

namespace DataTransferObjects.Social
{
    public class FileToUsersDto
    {
        public IEnumerable<string> UserNumbers { get; set; }
        public string Caption { get; set; }
        public string MimeType { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public Priority Priority { get; set; }
    }
}
