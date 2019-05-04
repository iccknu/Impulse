using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DataTransferObjects.Social;
using Enums;
using Hangfire;
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
        private static Dictionary<Provider, string> parentIds;

        public SocialController(Func<Provider, ISocialProvider> serviceAccessor)
        {
            _serviceAccessor = serviceAccessor;
        }

        static SocialController()
        {
            parentIds = new Dictionary<Provider, string>();
            foreach (var provider in (Provider[])Enum.GetValues(typeof(Provider)))
            {
                parentIds[provider] = null;
            }
        }

        #region User Methods
        //POST: api/Social/SendMessageToUsersAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendMessageToUsersAsync([FromBody]MessageToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ISocialProvider provider = _serviceAccessor(model.Provider);
            MessageToUserDto messageToUser = new MessageToUserDto
            {
                SenderName = model.SenderName,
                Subject = model.Subject,
                Message = model.Message,
                Priority = model.Priority
            };

            using (IEnumerator<string> enumer = model.EmailOrUserNumbers.GetEnumerator())
            {
                if (enumer.MoveNext())
                {
                    if (string.IsNullOrEmpty(parentIds[model.Provider]) ||
                        JobStorage.Current.GetMonitoringApi().JobDetails(parentIds[model.Provider]) == null)
                    {
                        messageToUser.EmailOrUserNumber = enumer.Current;
                        parentIds[model.Provider] = BackgroundJob.Enqueue(() => provider.SendMessageToUserAsync(messageToUser));
                        if (!enumer.MoveNext())
                            return Ok();
                    }

                    do
                    {
                        messageToUser.EmailOrUserNumber = enumer.Current;
                        parentIds[model.Provider] = BackgroundJob.ContinueWith(parentIds[model.Provider], () => provider.SendMessageToUserAsync(messageToUser));
                    } while (enumer.MoveNext());
                }
            }

            return Ok();
        }

        //POST: api/Social/SendPhotoToUsersAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendPhotoToUsersAsync([FromForm]FileToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFileAsync(model.File);
            ISocialProvider provider = _serviceAccessor(model.Provider);
            FileToUserDto fileToUser = new FileToUserDto
            {
                Name = model.File.FileName,
                SenderName = model.SenderName,
                Subject = model.Subject,
                Caption = model.Caption ?? "",
                Path = path,
                Priority = model.Priority
            };
            
            using (IEnumerator<string> enumer = model.EmailOrUserNumbers.GetEnumerator())
            {
                if (enumer.MoveNext())
                {
                    if (string.IsNullOrEmpty(parentIds[model.Provider]) ||
                        JobStorage.Current.GetMonitoringApi().JobDetails(parentIds[model.Provider]) == null)
                    {
                        fileToUser.EmailOrUserNumber = enumer.Current;
                        parentIds[model.Provider] = BackgroundJob.Enqueue(() => provider.SendPhotoToUserAsync(fileToUser));
                        if (!enumer.MoveNext())
                            return Ok();
                    }

                    do
                    {
                        fileToUser.EmailOrUserNumber = enumer.Current;
                        parentIds[model.Provider] = BackgroundJob.ContinueWith(parentIds[model.Provider], () => provider.SendPhotoToUserAsync(fileToUser));
                    } while (enumer.MoveNext());
                }
            }

            return Ok();
        }

        //POST: api/Social/SendFileToUsersAsync
        [HttpPost("[action]")]
        public async Task<IActionResult> SendFileToUsersAsync([FromForm]FileToUsersModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string path = await SaveFileAsync(model.File);
            ISocialProvider provider = _serviceAccessor(model.Provider);
            FileToUserDto fileToUser = new FileToUserDto
            {
                Name = model.File.FileName,
                SenderName = model.SenderName,
                Subject = model.Subject,
                Caption = model.Caption ?? "",
                MimeType = model.File.ContentType,
                Path = path,
                Priority = model.Priority
            };

            using (IEnumerator<string> enumer = model.EmailOrUserNumbers.GetEnumerator())
            {
                if (enumer.MoveNext())
                {
                    if (string.IsNullOrEmpty(parentIds[model.Provider]) ||
                        JobStorage.Current.GetMonitoringApi().JobDetails(parentIds[model.Provider]) == null)
                    {
                        fileToUser.EmailOrUserNumber = enumer.Current;
                        parentIds[model.Provider] = BackgroundJob.Enqueue(() => provider.SendFileToUserAsync(fileToUser));
                        if (!enumer.MoveNext())
                            return Ok();
                    }

                    do
                    {
                        fileToUser.EmailOrUserNumber = enumer.Current;
                        parentIds[model.Provider] = BackgroundJob.ContinueWith(parentIds[model.Provider], () => provider.SendFileToUserAsync(fileToUser));
                    } while (enumer.MoveNext());
                }
            }

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
            // 10 messagese per minute
            await _serviceAccessor(model.Provider).SendMessageToChannelAsync(new MessageToChannelOrGroupDto
            {
                Title = model.Title,
                SenderName = model.SenderName,
                Subject = model.Subject,
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

            await _serviceAccessor(model.Provider).SendPhotoToChannelAsync(new FileToChannelOrGroupDto
            {
                Title = model.Title,
                Name = model.File.FileName,
                SenderName = model.SenderName,
                Subject = model.Subject,
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

            await _serviceAccessor(model.Provider).SendFileToChannelAsync(new FileToChannelOrGroupDto
            {
                Title = model.Title,
                Name = model.File.FileName,
                SenderName = model.SenderName,
                Subject = model.Subject,
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

            await _serviceAccessor(model.Provider).AddUserToChannelAsync(new UserManipulationInChannelOrGroupDto
            {
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

            await _serviceAccessor(model.Provider).DeleteUserFromChannelAsync(new UserManipulationInChannelOrGroupDto
            {
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
                SenderName = model.SenderName,
                Subject = model.Subject,
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
                SenderName = model.SenderName,
                Subject = model.Subject,
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
                SenderName = model.SenderName,
                Subject = model.Subject,
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