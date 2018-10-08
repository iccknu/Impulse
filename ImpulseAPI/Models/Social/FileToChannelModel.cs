using Enums;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class FileToChannelModel
    {
        [Required(ErrorMessage = "Channel tittle can't be empty")]
        public string ChannelTitle { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}
