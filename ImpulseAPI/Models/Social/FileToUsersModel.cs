using Enums;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class FileToUsersModel
    {
        public IEnumerable<string> Logins { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; }

        public string SenderName { get; set; }

        public string Subject { get; set; }

        public string Caption { get; set; }

        [EnumDataType(typeof(Priority), ErrorMessage = "This priority not registered yet.")]
        public Priority Priority { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}