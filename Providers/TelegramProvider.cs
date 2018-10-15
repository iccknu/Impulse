using DataTransferObjects.Configurations;
using DataTransferObjects.Social;
using Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Messages;
using TLSharp.Core;
using TLSharp.Core.Utils;

namespace Providers
{
    public class TelegramProvider : ISocialProvider
    {
        private readonly TelegramConfigurationsDto _telegramConfigurations;
        private readonly TelegramClient client;

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

        public async Task SendMessageToUserAsync(MessageToUsersDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Message))
                throw new Exception("Message can't be empty");

            TLUser user;
            foreach (var number in model.UserNumbers)
            {
                user = await GetUserAsync(number);
                await client.SendMessageAsync(new TLInputPeerUser() { UserId = user.Id }, model.Message);
            }
        }

        public async Task SendMessageToChannelAsync(MessageToChannelDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Message))
                throw new Exception("Message can't be empty");

            var channel = await GetChannelAsync(model.ChannelTitle);
            await client.SendMessageAsync(new TLInputPeerChannel() { ChannelId = channel.Id, AccessHash = channel.AccessHash.Value }, model.Message);
        }

        public async Task SendFileToUserAsync(FileToUsersDto model)
        {
            TLUser user;
            TLAbsInputFile fileResult = await UpLoadFileAsync(model.Path, model.Name);
            foreach (var number in model.UserNumbers)
            {
                user = await GetUserAsync(number);

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
            }
        }

        public async Task SendFileToChannelAsync(FileToChannelDto model)
        {
            var channel = await GetChannelAsync(model.ChannelTitle);

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
        }

        public async Task SendPhotoToUserAsync(FileToUsersDto model)
        {
            TLUser user;
            TLAbsInputFile fileResult = await UpLoadFileAsync(model.Path, model.Name);
            foreach (var number in model.UserNumbers)
            {
                user = await GetUserAsync(number);
                await client.SendUploadedPhoto(new TLInputPeerUser() { UserId = user.Id }, fileResult, model.Caption);
            }
        }

        public async Task SendPhotoToChannelAsync(FileToChannelDto model)
        {
            var channel = await GetChannelAsync(model.ChannelTitle);

            var fileResult = await UpLoadFileAsync(model.Path, model.Name);
            await client.SendUploadedPhoto(new TLInputPeerChannel() { ChannelId = channel.Id, AccessHash = channel.AccessHash.Value },
                fileResult, model.Caption);
        }

        public async Task AddUserToChannelAsync(UserManipulationInChannelDto model)
        {
            var user = await GetUserAsync(model.UserNumber);
            var channel = await GetChannelAsync(model.Channel);

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

        public async Task DeleteUserFromChannelAsync(UserManipulationInChannelDto model)
        {
            var user = await GetUserAsync(model.UserNumber);
            var channel = await GetChannelAsync(model.Channel);

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

        public async Task AddUserToContactsAsync(string userNumber, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(userNumber))
                throw new Exception("User number can't be empty");

            if (!Regex.Match(userNumber, phoneNumberPattern).Success)
                throw new Exception("User number not correct: " + userNumber);

            // this is because the contacts in the address come without the "+" prefix
            var normalizedNumber = userNumber.StartsWith("+") ?
                userNumber.Substring(1, userNumber.Length - 1) :
                userNumber;

            if (string.IsNullOrWhiteSpace(firstName))
                throw new Exception("First name can't be empty");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new Exception("Last name can't be empty");

            TLVector<TLInputPhoneContact> contacts = new TLVector<TLInputPhoneContact>
            {
                new TLInputPhoneContact() { Phone = normalizedNumber, FirstName = firstName, LastName = lastName }
            };

            var req = new TeleSharp.TL.Contacts.TLRequestImportContacts() { Contacts = contacts };

            var result = await client.SendRequestAsync<TeleSharp.TL.Contacts.TLImportedContacts>(req).ConfigureAwait(false);
        }

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

            //get available contacts
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

        private async Task<TLChannel> GetChannelAsync(string channelTittle)
        {
            if (string.IsNullOrWhiteSpace(channelTittle))
                throw new Exception("Channel tittle can't be empty");

            //get available dialogs
            var dialogs = (TLDialogs)await client.GetUserDialogsAsync();
            var channel = dialogs.Chats
                .OfType<TLChannel>()
                .FirstOrDefault(c => c.Title == channelTittle);

            if (channel == null)
            {
                throw new Exception("Channel '" + channelTittle + "' was not found in Channel List.");
            }

            return channel;
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
    }
}
