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

        //POST: api/Social/SendMessageToChannelAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageToChannelAsync([FromBody]MessageToChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _serviceAccessor(model.Provider).SendMessageToChannelAsync(new MessageToChannelDto {
                ChannelTitle = model.ChannelTitle,
                Message = model.Message
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

            await _serviceAccessor(model.Provider).SendPhotoToUsersAsync(new FileToUsersDto {
                UserNumbers = model.UserNumbers,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                Path = path,
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/SendPhotoToChannelAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendPhotoToChannelAsync([FromForm]FileToChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFileAsync(model.File);

            await _serviceAccessor(model.Provider).SendPhotoToChannelAsync(new FileToChannelDto {
                ChannelTitle = model.ChannelTitle,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                Path = path
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

            await _serviceAccessor(model.Provider).SendFileToUsersAsync(new FileToUsersDto {
                UserNumbers = model.UserNumbers,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                MimeType = model.File.ContentType,
                Path = path,
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/SendFileToChannelAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendFileToChannelAsync([FromForm]FileToChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFileAsync(model.File);

            await _serviceAccessor(model.Provider).SendFileToChannelAsync(new FileToChannelDto {
                ChannelTitle = model.ChannelTitle,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                MimeType = model.File.ContentType,
                Path = path
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

        //POST: api/Social/AddUserToChannelAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> AddUserToChannelAsync([FromBody]UserManipulationInChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _serviceAccessor(model.Provider).AddUserToChannelAsync(new UserManipulationInChannelDto {
                UserNumber = model.UserNumber,
                Channel = model.Channel
            });

            return Ok();
        }

        // DELETE: api/Social/DeleteUserFromChannelAsync
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteUserFromChannelAsync([FromBody]UserManipulationInChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _serviceAccessor(model.Provider).DeleteUserFromChannelAsync(new UserManipulationInChannelDto {
                UserNumber = model.UserNumber,
                Channel = model.Channel
            });

            return Ok();
        }

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