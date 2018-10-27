using Enums;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class MessageToChannelOrGroupModel
    {
        [Required(ErrorMessage = "Title can't be empty")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Message can't be empty")]
        public string Message { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}