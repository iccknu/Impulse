using Enums;
using ImpulseAPI.Extensions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class FileToUsersModel
    {
        [ListRegularExpression(@"^(\+?380)(50|66|95|99|63|73|93|91|92|94|67|68|96|97|98|39)[0-9]{7}$", ErrorMessage = "Email or User number is not correct")]
        public IEnumerable<string> EmailOrUserNumbers { get; set; }

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