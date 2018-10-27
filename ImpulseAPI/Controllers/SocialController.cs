using System;
using System.IO;
using System.Threading.Tasks;
using DataTransferObjects.Social;
using Enums;
using ImpulseAPI.Models.Social;
using Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImpulseAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Social")]
    public class SocialController : ControllerBase
    {
        private readonly Func<Provider, ISocialProvider> _serviceAccessor;

        public SocialController(Func<Provider, ISocialProvider> serviceAccessor)
        {
            _serviceAccessor = serviceAccessor;
        }

        #region User Methods
        //POST: api/Social/SendMessageToUsersAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageToUsersAsync([FromBody]MessageToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceAccessor(model.Provider).SendMessageToUsersAsync(new MessageToUsersDto {
                Message = model.Message,
                UserNumbers = model.UserNumbers,
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/SendPhotoToUsersAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendPhotoToUsersAsync([FromForm]FileToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFileAsync(model.File);

            await _serviceAccessor(model.Provider).SendPhotoToUsersAsync(new FileToUsersDto
            {
                UserNumbers = model.UserNumbers,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                Path = path,
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/SendFileToUsersAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendFileToUsersAsync([FromForm]FileToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFileAsync(model.File);

            await _serviceAccessor(model.Provider).SendFileToUsersAsync(new FileToUsersDto
            {
                UserNumbers = model.UserNumbers,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                MimeType = model.File.ContentType,
                Path = path,
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/AddUserToContactsAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> AddUserToContactsAsync([FromBody]UserInfoModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceAccessor(model.Provider).AddUserToContactsAsync(new UserInfoDto
            {
                UserNumber = model.UserNumber,
                FirstName = model.FirstName,
                LastName = model.LastName
            });

            return Ok();
        }
        #endregion

        #region Channel Methods
        //POST: api/Social/SendMessageToChannelAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageToChannelAsync([FromBody]MessageToChannelOrGroupModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _serviceAccessor(model.Provider).SendMessageToChannelAsync(new MessageToChannelOrGroupDto {
                Title = model.Title,
                Message = model.Message
            });

            return Ok();
        }

        //POST: api/Social/SendPhotoToChannelAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendPhotoToChannelAsync([FromForm]FileToChannelOrGroupModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFileAsync(model.File);

            await _serviceAccessor(model.Provider).SendPhotoToChannelAsync(new FileToChannelOrGroupDto {
                Title = model.Title,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                Path = path
            });

            return Ok();
        }

        //POST: api/Social/SendFileToChannelAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendFileToChannelAsync([FromForm]FileToChannelOrGroupModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFileAsync(model.File);

            await _serviceAccessor(model.Provider).SendFileToChannelAsync(new FileToChannelOrGroupDto {
                Title = model.Title,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                MimeType = model.File.ContentType,
                Path = path
            });

            return Ok();
        }

        //POST: api/Social/AddUserToChannelAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> AddUserToChannelAsync([FromBody]UserManipulationInChannelOrGroupModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _serviceAccessor(model.Provider).AddUserToChannelAsync(new UserManipulationInChannelOrGroupDto {
                UserNumber = model.UserNumber,
                Title = model.Title
            });

            return Ok();
        }

        // DELETE: api/Social/DeleteUserFromChannelAsync
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteUserFromChannelAsync([FromBody]UserManipulationInChannelOrGroupModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _serviceAccessor(model.Provider).DeleteUserFromChannelAsync(new UserManipulationInChannelOrGroupDto {
                UserNumber = model.UserNumber,
                Title = model.Title
            });

            return Ok();
        }

        //POST: api/Social/CreateChannelAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateChannelAsync([FromBody]ChannelOrGroupCreationModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceAccessor(model.Provider).CreateChannelAsync(new ChannelOrGroupCreationDto
            {
                Title = model.Title,
                Description = model.Description
            });

            return Ok();
        }

        //POST: api/Social/RemoveChannelAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveChannelAsync([FromBody]ChannelOrGroupRemovingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceAccessor(model.Provider).RemoveChannelAsync(model.Title);

            return Ok();
        }
        #endregion

        #region Group Methods
        //POST: api/Social/SendMessageToGroupAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageToGroupAsync([FromBody]MessageToChannelOrGroupModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceAccessor(model.Provider).SendMessageToGroupAsync(new MessageToChannelOrGroupDto
            {
                Title = model.Title,
                Message = model.Message
            });

            return Ok();
        }

        //POST: api/Social/SendPhotoToGroupAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendPhotoToGroupAsync([FromForm]FileToChannelOrGroupModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFileAsync(model.File);

            await _serviceAccessor(model.Provider).SendPhotoToGroupAsync(new FileToChannelOrGroupDto
            {
                Title = model.Title,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                Path = path
            });

            return Ok();
        }

        //POST: api/Social/SendFileToGroupAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendFileToGroupAsync([FromForm]FileToChannelOrGroupModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFileAsync(model.File);

            await _serviceAccessor(model.Provider).SendFileToGroupAsync(new FileToChannelOrGroupDto
            {
                Title = model.Title,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                MimeType = model.File.ContentType,
                Path = path
            });

            return Ok();
        }

        //POST: api/Social/AddUserToGroupAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> AddUserToGroupAsync([FromBody]UserManipulationInChannelOrGroupModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceAccessor(model.Provider).AddUserToGroupAsync(new UserManipulationInChannelOrGroupDto
            {
                UserNumber = model.UserNumber,
                Title = model.Title
            });

            return Ok();
        }

        // DELETE: api/Social/DeleteUserFromGroupAsync
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteUserFromGroupAsync([FromBody]UserManipulationInChannelOrGroupModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceAccessor(model.Provider).DeleteUserFromGroupAsync(new UserManipulationInChannelOrGroupDto
            {
                UserNumber = model.UserNumber,
                Title = model.Title
            });

            return Ok();
        }

        //POST: api/Social/CreateGroupAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateGroupAsync([FromBody]ChannelOrGroupCreationModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceAccessor(model.Provider).CreateGroupAsync(new ChannelOrGroupCreationDto
            {
                Title = model.Title,
                Description = model.Description
            });

            return Ok();
        }

        //POST: api/Social/RemoveGroupAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveGroupAsync([FromBody]ChannelOrGroupRemovingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceAccessor(model.Provider).RemoveGroupAsync(model.Title);

            return Ok();
        }
        #endregion

        [NonAction]
        private async Task<string> SaveFileAsync(IFormFile file)
        {
            string path = "wwwroot/Files/" + file.FileName;
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return path;
        }
    }
}