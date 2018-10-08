using Enums;
using ImpulseAPI.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class MessageToUsersModel
    {
        [ListRegularExpression(@"^(\+?380)(50|66|95|99|63|73|93|91|92|94|67|68|96|97|98|39)[0-9]{7}$", ErrorMessage = "User number not correct")]
        public IEnumerable<string> UserNumbers { get; set; }

        [Required(ErrorMessage = "Message can't be empty")]
        public string Message { get; set; }

        [EnumDataType(typeof(Priority), ErrorMessage = "This priority not registered yet.")]
        public Priority Priority { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}