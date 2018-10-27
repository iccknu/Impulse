using Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ImpulseAPI.Models.Social
{
    public class FileToChannelOrGroupModel
    {
        [Required(ErrorMessage = "Title can't be empty")]
        public string Title { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile File { get; set; }

        public string Caption { get; set; }

        [EnumDataType(typeof(Provider), ErrorMessage = "This social provider not registered yet.")]
        public Provider Provider { get; set; }
    }
}
