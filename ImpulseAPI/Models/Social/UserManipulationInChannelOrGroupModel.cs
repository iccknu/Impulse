using Enums;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class UserManipulationInChannelOrGroupModel
    {
        [Required(ErrorMessage = "Login can't be empty")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Title can't be empty")]
        public string Title { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}