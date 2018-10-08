using Enums;
using System.Collections.Generic;

namespace DataTransferObjects.Social
{
    public class MessageToUsersDto
    {
        public IEnumerable<string> UserNumbers { get; set; }
        public string Message { get; set; }
        public Priority Priority { get; set; }
    }
}
