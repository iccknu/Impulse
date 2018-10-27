using Enums;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class ChannelOrGroupCreationModel
    {
        [Required(ErrorMessage = "Title can't be empty")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description can't be empty")]
        public string Description { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}
