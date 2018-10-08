using System;
using System.IO;
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
        public IActionResult SendMessageToUser([FromBody]MessageToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _serviceAccessor(model.Provider).SendMessageToUserAsync(new MessageToUsersDto {
                Message = model.Message,
                UserNumbers = model.UserNumbers,
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/SendMessageToChannel
        [HttpPost("[action]")]
        public IActionResult SendMessageToChannel([FromBody]MessageToChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            _serviceAccessor(model.Provider).SendMessageToChannelAsync(new MessageToChannelDto {
                ChannelTitle = model.ChannelTitle,
                Message = model.Message
            });

            return Ok();
        }

        //POST: api/Social/SendPhotoToUser
        [HttpPost("[action]")]
        public IActionResult SendPhotoToUser([FromBody]FileToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            _serviceAccessor(model.Provider).SendPhotoToUserAsync(new FileToUsersDto {
                UserNumbers = model.UserNumbers,
                Name = "123",
                Caption = "123",
                Path = "123",
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/SendPhotoToChannel
        [HttpPost("[action]")]
        public IActionResult SendPhotoToChannel([FromBody]FileToChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            _serviceAccessor(model.Provider).SendPhotoToChannelAsync(new FileToChannelDto {
                ChannelTitle = model.ChannelTitle,
                Name = "123",
                Caption = "123",
                Path = "123"
            });

            return Ok();
        }

        //POST: api/Social/SendFileToUser
        [HttpPost("[action]")]
        public IActionResult SendFileToUser([FromBody]FileToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            _serviceAccessor(model.Provider).SendFileToUserAsync(new FileToUsersDto {
                UserNumbers = model.UserNumbers,
                Name = "123",
                Caption = "123",
                Path = "123",
                Priority = model.Priority
            });

            return Ok();
        }

        //POST: api/Social/SendFileToChannel
        [HttpPost("[action]")]
        public IActionResult SendFileToChannel([FromBody]FileToChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            _serviceAccessor(model.Provider).SendFileToChannelAsync(new FileToChannelDto {
                ChannelTitle = model.ChannelTitle,
                Name = "123",
                Caption = "123",
                Path = "123"
            });

            return Ok();
        }

        //POST: api/Social/add
        [HttpPost("add")]
        public IActionResult AddUserToChannel([FromBody]UserManipulationInChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            _serviceAccessor(model.Provider).AddUserToChannelAsync(new UserManipulationInChannelDto {
                UserNumber = model.UserNumber,
                Channel = model.Channel
            });

            return Ok();
        }

        // DELETE: api/Social/delete
        [HttpDelete("delete")]
        public IActionResult DeleteUserFromChannel([FromBody]UserManipulationInChannelModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            _serviceAccessor(model.Provider).DeleteUserFromChannelAsync(new UserManipulationInChannelDto {
                UserNumber = model.UserNumber,
                Channel = model.Channel
            });

            return Ok();
        }

        [NonAction]
        private MemoryStream GetFileFromHttpRequest()
        {
            var files = HttpContext.Request.Form.Files;
            MemoryStream ms = new MemoryStream();
            if (files.Count == 1)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                        file.CopyToAsync(ms);
                }
            }

            return ms;
        }
    }
}