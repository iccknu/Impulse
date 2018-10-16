using Enums;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class UserInfoModel
    {
        [Required(ErrorMessage = "User number can't be empty")]
        [RegularExpression(@"^(\+?380)(50|66|95|99|63|73|93|91|92|94|67|68|96|97|98|39)[0-9]{7}$", ErrorMessage = "User number not correct")]
        public string UserNumber { get; set; }

        [Required(ErrorMessage = "First name can't be empty")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name can't be empty")]
        public string LastName { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}
