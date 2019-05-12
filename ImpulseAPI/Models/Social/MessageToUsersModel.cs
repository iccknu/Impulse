using Enums;
using ImpulseAPI.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class MessageToUsersModel
    {
        public IEnumerable<string> Logins { get; set; }

        public string SenderName { get; set; }

        public string Subject { get; set; }

        [Required(ErrorMessage = "Message can't be empty")]
        public string Message { get; set; }

        [EnumDataType(typeof(Priority), ErrorMessage = "This priority not registered yet.")]
        public Priority Priority { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}