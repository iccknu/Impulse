using DataTransferObjects.Configurations;
using DataTransferObjects.Social;
using Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Contacts;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSharp.Core.Utils;

namespace Providers
{
    public class TelegramProvider : ISocialProvider
    {
        private readonly TelegramConfigurationsDto _telegramConfigurations;
        private readonly TelegramClient client;
        private const int DileyTime = 10 * 1000;

        public TelegramProvider(IOptions<TelegramConfigurationsDto> telegramConfigurations)
        {
            _telegramConfigurations = telegramConfigurations.Value;
            client = new TelegramClient(_telegramConfigurations.ApiId, _telegramConfigurations.ApiHash);
            client.ConnectAsync().Wait();
        }

        private const string phoneNumberPattern = @"^(\+?380)(50|66|95|99|63|73|93|91|92|94|67|68|96|97|98|39)[0-9]{7}$";

        //public void RegisterService()
        //{
        //    var client = TClient.Client;
        //    string phoneNumber = ConfigurationManager.AppSettings["PhoneNumber"];
        //    if (string.IsNullOrEmpty(phoneNumber))
        //        throw new Exception(string.Format(appConfigMsgWarning, "PhoneNumber"));

        //    //Task only for debug
        //    Task t = Task.Run(async () =>
        //    {
        //        var hash = client.SendCodeRequestAsync(phoneNumber).Result;

        //        string code = "Insert your code here in debug";
        //        await client.MakeAuthAsync(phoneNumber, hash, code);
        //    });

        //    t.Wait();
        //}

        //class TBot
        //{
        //    private readonly TelegramBotClient telegramBot;

        //    public TBot()
        //    {
        //        string botToken = ConfigurationManager.AppSettings["BotToken"];
        //        if (string.IsNullOrEmpty(botToken))
        //            throw new Exception(string.Format(appConfigMsgWarning, "BotToken"));

        //        telegramBot = new TelegramBotClient(botToken);
        //    }

        //    public async Task SendMessageAsync(string chatId, string message)
        //    {
        //        await telegramBot.SendTextMessageAsync(
        //            chatId,
        //            message);
        //    }
        //    public async Task SendPhotoAsync(string chatId, string path, string name, string caption)
        //    {
        //        Stream file = new FileStream(path + name, FileMode.Open);
        //        FileToSend photo = new FileToSend(name, file);

        //        await telegramBot.SendPhotoAsync(
        //            chatId,
        //            photo,
        //            caption);

        //    }
        //    public async Task SendFileAsync(string chatId, string path, string name, string caption)
        //    {
        //        Stream file = new FileStream(path + name, FileMode.Open);
        //        FileToSend document = new FileToSend(name, file);
        //        await telegramBot.SendDocumentAsync(
        //            chatId,
        //            document,
        //            caption);

        //    }
        //}
        #region User Methods
        public async Task SendMessageToUserAsync(MessageToUserDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Message))
                throw new Exception("Message can't be empty");

            TLUser user = await GetUserAsync(model.UserNumber);
            await client.SendMessageAsync(new TLInputPeerUser() { UserId = user.Id }, model.Message);
            Thread.Sleep(DileyTime);
        }

        public async Task SendFileToUserAsync(FileToUserDto model)
        {
            TLUser user = await GetUserAsync(model.UserNumber);
            TLAbsInputFile fileResult = await UpLoadFileAsync(model.Path, model.Name);
            await client.SendUploadedDocument(
                    new TLInputPeerUser() { UserId = user.Id },
                    fileResult,
                    model.Caption,
                    model.MimeType,
                    new TLVector<TLAbsDocumentAttribute>()
                    {
                        new TLDocumentAttributeFilename
                        {
                            FileName = model.Name
                        }
                    });
            Thread.Sleep(DileyTime);
        }

        public async Task SendPhotoToUserAsync(FileToUserDto model)
        {
            TLUser user = await GetUserAsync(model.UserNumber);
            TLAbsInputFile fileResult = await UpLoadFileAsync(model.Path, model.Name);
            await client.SendUploadedPhoto(new TLInputPeerUser() { UserId = user.Id }, fileResult, model.Caption);
            Thread.Sleep(DileyTime);
        }

        public async Task AddUserToContactsAsync(UserInfoDto model)
        {
            if (string.IsNullOrWhiteSpace(model.UserNumber))
                throw new Exception("User number can't be empty");

            if (!Regex.Match(model.UserNumber, phoneNumberPattern).Success)
                throw new Exception("User number not correct: " + model.UserNumber);

            // this is because the contacts in the address come without the "+" prefix
            var normalizedNumber = model.UserNumber.StartsWith("+") ?
                model.UserNumber.Substring(1, model.UserNumber.Length - 1) :
                model.UserNumber;

            if (string.IsNullOrWhiteSpace(model.FirstName))
                throw new Exception("First name can't be empty");

            if (string.IsNullOrWhiteSpace(model.LastName))
                throw new Exception("Last name can't be empty");

            TLVector<TLInputPhoneContact> contacts = new TLVector<TLInputPhoneContact>
            {
                new TLInputPhoneContact() { Phone = normalizedNumber, FirstName = model.FirstName, LastName = model.LastName }
            };

            var request = new TLRequestImportContacts() { Contacts = contacts };

            await client.SendRequestAsync<TLImportedContacts>(request);
        }
        #endregion

        #region Channel Methods
        public async Task SendMessageToChannelAsync(MessageToChannelOrGroupDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Message))
                throw new Exception("Message can't be empty");

            var channel = await GetChannelAsync(model.Title);
            await client.SendMessageAsync(new TLInputPeerChannel() { ChannelId = channel.Id, AccessHash = channel.AccessHash.Value }, model.Message);
            Thread.Sleep(DileyTime);
        }

        public async Task SendFileToChannelAsync(FileToChannelOrGroupDto model)
        {
            var channel = await GetChannelAsync(model.Title);

            var fileResult = await UpLoadFileAsync(model.Path, model.Name);
            await client.SendUploadedDocument(
                new TLInputPeerChannel() { ChannelId = channel.Id, AccessHash = channel.AccessHash.Value },
                fileResult,
                model.Caption,
                model.MimeType,
                new TLVector<TLAbsDocumentAttribute>()
                {
                    new TLDocumentAttributeFilename
                    {
                        FileName = model.Name
                    }
                });
            Thread.Sleep(DileyTime);
        }

        public async Task SendPhotoToChannelAsync(FileToChannelOrGroupDto model)
        {
            var channel = await GetChannelAsync(model.Title);

            var fileResult = await UpLoadFileAsync(model.Path, model.Name);
            await client.SendUploadedPhoto(new TLInputPeerChannel() { ChannelId = channel.Id, AccessHash = channel.AccessHash.Value },
                fileResult, model.Caption);
            Thread.Sleep(DileyTime);
        }

        public async Task AddUserToChannelAsync(UserManipulationInChannelOrGroupDto model)
        {
            var user = await GetUserAsync(model.UserNumber);
            var channel = await GetChannelAsync(model.Title);

            //generate request of adding
            var request = new TLRequestInviteToChannel
            {
                Channel = new TLInputChannel
                {
                    ChannelId = channel.Id,
                    AccessHash = channel.AccessHash.Value
                },
                Users = new TLVector<TLAbsInputUser>
                {
                    new TLInputUser
                    {
                        UserId = user.Id,
                        AccessHash = user.AccessHash.Value
                    }
                }
            };

            await client.SendRequestAsync<object>(request);
        }

        public async Task DeleteUserFromChannelAsync(UserManipulationInChannelOrGroupDto model)
        {
            var user = await GetUserAsync(model.UserNumber);
            var channel = await GetChannelAsync(model.Title);

            //generate request of deleting
            var request = new TLRequestKickFromChannel
            {
                Channel = new TLInputChannel
                {
                    ChannelId = channel.Id,
                    AccessHash = channel.AccessHash.Value
                },
                UserId = new TLInputUser
                {
                    UserId = user.Id,
                    AccessHash = user.AccessHash.Value
                },
                Kicked = true
            };

            await client.SendRequestAsync<object>(request);
        }

        public async Task CreateChannelAsync(ChannelOrGroupCreationDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new Exception("Title can't be empty");

            if (string.IsNullOrWhiteSpace(model.Description))
                throw new Exception("Description can't be empty");

            var request = new TLRequestCreateChannel()
            {
                Title = model.Title,
                About = model.Description
            };

            await client.SendRequestAsync<object>(request);
        }

        public async Task RemoveChannelAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Title can't be empty");

            var channel = await GetChannelAsync(title);
            var request = new TLRequestDeleteChannel()
            {
                Channel = new TLInputChannel
                {
                    ChannelId = channel.Id,
                    AccessHash = channel.AccessHash.Value
                }
            };

            await client.SendRequestAsync<object>(request);
        }
        #endregion

        #region Group Methods
        public async Task SendMessageToGroupAsync(MessageToChannelOrGroupDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Message))
                throw new Exception("Message can't be empty");

            var group = await GetGroupAsync(model.Title);
            await client.SendMessageAsync(new TLInputPeerChannel() { ChannelId = group.Id, AccessHash = group.AccessHash.Value }, model.Message);
            Thread.Sleep(DileyTime);
        }

        public async Task SendFileToGroupAsync(FileToChannelOrGroupDto model)
        {
            var group = await GetGroupAsync(model.Title);

            var fileResult = await UpLoadFileAsync(model.Path, model.Name);
            await client.SendUploadedDocument(
                new TLInputPeerChannel() { ChannelId = group.Id, AccessHash = group.AccessHash.Value },
                fileResult,
                model.Caption,
                model.MimeType,
                new TLVector<TLAbsDocumentAttribute>()
                {
                    new TLDocumentAttributeFilename
                    {
                        FileName = model.Name
                    }
                });
            Thread.Sleep(DileyTime);
        }

        public async Task SendPhotoToGroupAsync(FileToChannelOrGroupDto model)
        {
            var group = await GetGroupAsync(model.Title);

            var fileResult = await UpLoadFileAsync(model.Path, model.Name);
            await client.SendUploadedPhoto(new TLInputPeerChannel() { ChannelId = group.Id, AccessHash = group.AccessHash.Value },
                fileResult, model.Caption);
            Thread.Sleep(DileyTime);
        }

        public async Task AddUserToGroupAsync(UserManipulationInChannelOrGroupDto model)
        {
            var user = await GetUserAsync(model.UserNumber);
            var group = await GetGroupAsync(model.Title);

            //generate request of adding
            var request = new TLRequestInviteToChannel
            {
                Channel = new TLInputChannel
                {
                    ChannelId = group.Id,
                    AccessHash = group.AccessHash.Value
                },
                Users = new TLVector<TLAbsInputUser>
                {
                    new TLInputUser
                    {
                        UserId = user.Id,
                        AccessHash = user.AccessHash.Value
                    }
                }
            };

            await client.SendRequestAsync<object>(request);
        }

        public async Task DeleteUserFromGroupAsync(UserManipulationInChannelOrGroupDto model)
        {
            var user = await GetUserAsync(model.UserNumber);
            var group = await GetGroupAsync(model.Title);

            //generate request of deleting
            var request = new TLRequestKickFromChannel
            {
                Channel = new TLInputChannel
                {
                    ChannelId = group.Id,
                    AccessHash = group.AccessHash.Value
                },
                UserId = new TLInputUser
                {
                    UserId = user.Id,
                    AccessHash = user.AccessHash.Value
                },
                Kicked = true
            };

            await client.SendRequestAsync<object>(request);
        }

        public async Task CreateGroupAsync(ChannelOrGroupCreationDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new Exception("Title can't be empty");

            if (string.IsNullOrWhiteSpace(model.Description))
                throw new Exception("Description can't be empty");

            // using TLRequestCreateChannel we can create SuperGroup/Channel
            var request = new TLRequestCreateChannel()
            {
                Title = model.Title,
                About = model.Description,
                Megagroup = true
            };

            await client.SendRequestAsync<object>(request);
        }

        public async Task RemoveGroupAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Title can't be empty");

            var group = await GetGroupAsync(title);
            var request = new TLRequestDeleteChannel()
            {
                Channel = new TLInputChannel
                {
                    ChannelId = group.Id,
                    AccessHash = group.AccessHash.Value
                }
            };

            await client.SendRequestAsync<object>(request);
        }
        #endregion

        #region Private Methods
        private async Task<TLUser> GetUserAsync(string userNumber)
        {
            if (string.IsNullOrWhiteSpace(userNumber))
                throw new Exception("User number can't be empty.");

            if (!Regex.Match(userNumber, phoneNumberPattern).Success)
                throw new Exception("User number not correct: " + userNumber);

            // this is because the contacts in the address come without the "+" prefix
            var normalizedNumber = userNumber.StartsWith("+") ?
                userNumber.Substring(1, userNumber.Length - 1) :
                userNumber;

            // get available contacts
            var result = await client.GetContactsAsync();

            var user = result.Users
                .OfType<TLUser>()
                .FirstOrDefault(x => x.Phone == normalizedNumber);

            if (user == null)
            {
                throw new Exception("Number '" + userNumber + "' was not found in Contacts List.");
            }

            return user;
        }

        private async Task<TLChannel> GetChannelAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Title can't be empty");

            // get available dialogs
            var dialogs = (TLDialogs)await client.GetUserDialogsAsync();
            var channel = dialogs.Chats
                .OfType<TLChannel>()
                .FirstOrDefault(c => c.Title == title);

            if (channel == null)
            {
                throw new Exception("Channel '" + title + "' was not found in Channel List.");
            }

            return channel;
        }

        private async Task<TLChannel> GetGroupAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Title can't be empty");

            // get available dialogs
            var dialogs = (TLDialogs)await client.GetUserDialogsAsync();
            var group = dialogs.Chats
                .OfType<TLChannel>()
                .FirstOrDefault(c => c.Megagroup && c.Title == title);

            if (group == null)
            {
                throw new Exception("Group '" + title + "' was not found in Group List.");
            }

            return group;
        }

        private async Task<TLAbsInputFile> UpLoadFileAsync(string path, string name)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Path can't be empty");
            if (string.IsNullOrEmpty(name))
                throw new Exception("Name can't be empty");
            if (!File.Exists(path))
                throw new Exception("Could not find file " + path);

            return await client.UploadFile(name, new StreamReader(path));
        }
        #endregion
    }
}
