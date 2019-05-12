using Enums;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class UserInfoModel
    {
        [Required(ErrorMessage = "Login can't be empty")]
        public string Login { get; set; }

        [Required(ErrorMessage = "First name can't be empty")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name can't be empty")]
        public string LastName { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}
