﻿using Enums;

namespace DataTransferObjects.Social
{
    public class FileToUserDto
    {
        public string Login { get; set; }
        public string SenderName { get; set; }
        public string Subject { get; set; }
        public string Caption { get; set; }
        public string MimeType { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public Priority Priority { get; set; }
    }
}
