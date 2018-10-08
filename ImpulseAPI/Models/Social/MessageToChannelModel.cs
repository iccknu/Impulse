using Enums;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class MessageToChannelModel
    {
        [Required(ErrorMessage = "Channel tittle can't be empty")]
        public string ChannelTitle { get; set; }

        [Required(ErrorMessage = "Message can't be empty")]
        public string Message { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}