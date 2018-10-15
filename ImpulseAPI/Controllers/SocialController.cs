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

        //POST: api/Social/SendMessageToUser
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageToUser([FromBody]MessageToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _serviceAccessor(model.Provider).SendMessageToUserAsync(new MessageToUsersDto {
                Message = model.Message,
                UserNumbers = model.UserNumbers,
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/SendMessageToChannel
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageToChannel([FromBody]MessageToChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _serviceAccessor(model.Provider).SendMessageToChannelAsync(new MessageToChannelDto {
                ChannelTitle = model.ChannelTitle,
                Message = model.Message
            });

            return Ok();
        }

        //POST: api/Social/SendPhotoToUser
        [HttpPost("[action]")]
        public async Task<IActionResult> SendPhotoToUser([FromForm]FileToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFile(model.File);

            await _serviceAccessor(model.Provider).SendPhotoToUserAsync(new FileToUsersDto {
                UserNumbers = model.UserNumbers,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                Path = path,
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/SendPhotoToChannel
        [HttpPost("[action]")]
        public async Task<IActionResult> SendPhotoToChannel([FromForm]FileToChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFile(model.File);

            await _serviceAccessor(model.Provider).SendPhotoToChannelAsync(new FileToChannelDto {
                ChannelTitle = model.ChannelTitle,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                Path = path
            });

            return Ok();
        }

        //POST: api/Social/SendFileToUser
        [HttpPost("[action]")]
        public async Task<IActionResult> SendFileToUser([FromForm]FileToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFile(model.File);

            await _serviceAccessor(model.Provider).SendFileToUserAsync(new FileToUsersDto {
                UserNumbers = model.UserNumbers,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                MimeType = model.File.ContentType,
                Path = path,
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/SendFileToChannel
        [HttpPost("[action]")]
        public async Task<IActionResult> SendFileToChannel([FromForm]FileToChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFile(model.File);

            await _serviceAccessor(model.Provider).SendFileToChannelAsync(new FileToChannelDto {
                ChannelTitle = model.ChannelTitle,
                Name = model.File.FileName,
                Caption = model.Caption ?? "",
                MimeType = model.File.ContentType,
                Path = path
            });

            return Ok();
        }

        //POST: api/Social/add
        [HttpPost("add")]
        public async Task<IActionResult> AddUserToChannel([FromBody]UserManipulationInChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await _serviceAccessor(model.Provider).AddUserToChannelAsync(new UserManipulationInChannelDto {
                UserNumber = model.UserNumber,
                Channel = model.Channel
            });

            return Ok();
        }

        // DELETE: api/Social/delete
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUserFromChannel([FromBody]UserManipulationInChannelModel model)
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
        private async Task<string> SaveFile(IFormFile file)
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