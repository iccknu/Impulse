using DataTransferObjects.Configurations;
using DataTransferObjects.Social;
using Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Providers
{
    public class SlackProvider : ISocialProvider
    {
        private readonly SlackConfigurationsDto _slackConfigurations;
        private readonly int DileyTime;

        public SlackProvider(IOptions<SlackConfigurationsDto> slackConfigurations)
        {
            _slackConfigurations = slackConfigurations.Value;
            DileyTime = (int)(1000 / _slackConfigurations.MessagesPerSecond);
        }

        private readonly string _slackApiLink = "https://slack.com/api/";

        #region User Methods
        public async Task SendMessageToUserAsync(MessageToUserDto model)
        {
            string userId = await GetUserIdAsync(model.Login);
            await SendMessage(userId, MessageBuilder(model.SenderName, model.Subject, model.Message));
            Thread.Sleep(DileyTime);
        }

        public async Task SendFileToUserAsync(FileToUserDto model)
        {
            var userId = await GetUserIdAsync(model.Login);
            await SendFile(userId, model.Path, MessageBuilder(model.SenderName, model.Subject, model.Caption), model.MimeType, model.Name);
            Thread.Sleep(DileyTime);
        }

        public async Task SendPhotoToUserAsync(FileToUserDto model)
        {
            var userId = await GetUserIdAsync(model.Login);
            await SendFile(userId, model.Path, MessageBuilder(model.SenderName, model.Subject, model.Caption), model.MimeType, model.Name);
            Thread.Sleep(DileyTime);
        }

        public async Task AddUserToContactsAsync(UserInfoDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Login))
                throw new ArgumentException("Login can't be empty");

            if (!EmailProvider.IsValidEmail(model.Login))
                throw new ArgumentException("Login is not a valid email: " + model.Login);

            var uri = new Uri(_slackApiLink + "users.admin.invite");
            using (HttpClient httpClient = new HttpClient())
            {
                var sendModel = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", _slackConfigurations.UserToken),
                    new KeyValuePair<string, string>("email", model.Login)
                };
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(sendModel))
                {
                    await httpClient.PostAsync(uri, content);
                }
            }
        }
        #endregion

        #region Channel Methods
        public async Task SendMessageToChannelAsync(MessageToChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in Slack");

        public async Task SendFileToChannelAsync(FileToChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in Slack");

        public async Task SendPhotoToChannelAsync(FileToChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in Slack");

        public async Task AddUserToChannelAsync(UserManipulationInChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in Slack");

        public async Task DeleteUserFromChannelAsync(UserManipulationInChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in Slack");

        public async Task CreateChannelAsync(ChannelOrGroupCreationDto model)
            => throw new NotImplementedException("This method is not supported in Slack");

        public async Task RemoveChannelAsync(string title)
            => throw new NotImplementedException("This method is not supported in Slack");
        #endregion

        #region Group Methods
        public async Task SendMessageToGroupAsync(MessageToChannelOrGroupDto model)
        {
            string groupId = await GetGroupIdAsync(model.Title);
            await SendMessage(groupId, MessageBuilder(model.SenderName, model.Subject, model.Message));
            Thread.Sleep(DileyTime);
        }

        public async Task SendFileToGroupAsync(FileToChannelOrGroupDto model)
        {
            var groupId = await GetGroupIdAsync(model.Title);
            await SendFile(groupId, model.Path, MessageBuilder(model.SenderName, model.Subject, model.Caption), model.MimeType, model.Name);
            Thread.Sleep(DileyTime);
        }

        public async Task SendPhotoToGroupAsync(FileToChannelOrGroupDto model)
        {
            var groupId = await GetGroupIdAsync(model.Title);
            await SendFile(groupId, model.Path, MessageBuilder(model.SenderName, model.Subject, model.Caption), model.MimeType, model.Name);
            Thread.Sleep(DileyTime);
        }

        public async Task AddUserToGroupAsync(UserManipulationInChannelOrGroupDto model)
        {
            var userId = await GetUserIdAsync(model.Login);
            var groupId = await GetGroupIdAsync(model.Title);

            var uri = new Uri(_slackApiLink + "channels.invite");
            using (HttpClient httpClient = new HttpClient())
            {
                var sendModel = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", _slackConfigurations.UserToken),
                    new KeyValuePair<string, string>("channel", groupId),
                    new KeyValuePair<string, string>("user", userId)
                };
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(sendModel))
                {
                    await httpClient.PostAsync(uri, content);
                }
            }
        }

        public async Task DeleteUserFromGroupAsync(UserManipulationInChannelOrGroupDto model)
        {
            var userId = await GetUserIdAsync(model.Login);
            var groupId = await GetGroupIdAsync(model.Title);

            var uri = new Uri(_slackApiLink + "channels.kick");
            using (HttpClient httpClient = new HttpClient())
            {
                var sendModel = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", _slackConfigurations.UserToken),
                    new KeyValuePair<string, string>("channel", groupId),
                    new KeyValuePair<string, string>("user", userId)
                };
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(sendModel))
                {
                    await httpClient.PostAsync(uri, content);
                }
            }
        }

        public async Task CreateGroupAsync(ChannelOrGroupCreationDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ArgumentException("Title can't be empty");

            var uri = new Uri(_slackApiLink + "channels.create");
            using (HttpClient httpClient = new HttpClient())
            {
                var sendModel = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", _slackConfigurations.UserToken),
                    new KeyValuePair<string, string>("name", model.Title)
                };
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(sendModel))
                {
                    await httpClient.PostAsync(uri, content);
                }
            }
        }

        public async Task RemoveGroupAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title can't be empty");

            var groupId = await GetGroupIdAsync(title);
            var uri = new Uri(_slackApiLink + "channels.delete");
            using (HttpClient httpClient = new HttpClient())
            {
                var sendModel = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", _slackConfigurations.UserToken),
                    new KeyValuePair<string, string>("channel", groupId)
                };
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(sendModel))
                {
                    await httpClient.PostAsync(uri, content);
                }
            }
        }
        #endregion

        public async Task<UserCheckResultDto> UserCheck(string emailOrUserNumber)
        {
            bool isValid = true;
            string errorMessage = null;
            try
            {
                await GetUserIdAsync(emailOrUserNumber);
            }
            catch (Exception ex)
            {
                isValid = false;
                errorMessage = ex.Message;
            }

            return new UserCheckResultDto
            {
                Login = emailOrUserNumber,
                IsValid = isValid,
                ErrorMessage = errorMessage
            };
        }

        #region Private Methods
        private async Task<string> GetUserIdAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Login can't be empty.");

            JToken user = null;
            var uri = new Uri(_slackApiLink + "users.list");
            using (HttpClient _httpClient = new HttpClient())
            {
                var model = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", _slackConfigurations.UserToken),
                };
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(model))
                {
                    var response = await _httpClient.PostAsync(uri, content);
                    var jo = JObject.Parse(await response.Content.ReadAsStringAsync());
                    user = (jo["members"] as JArray).FirstOrDefault(x => x.Value<string>("name") == login);
                }
            }

            if (user == null)
            {
                throw new Exception("Login '" + login + "' was not found in Team members.");
            }

            return user["id"].ToString();
        }

        private async Task<string> GetGroupIdAsync(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title can't be empty");

            JToken group = null;
            var uri = new Uri(_slackApiLink + "channels.list");
            using (HttpClient _httpClient = new HttpClient())
            {
                var model = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", _slackConfigurations.UserToken),
                };
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(model))
                {
                    var response = await _httpClient.PostAsync(uri, content);
                    var jo = JObject.Parse(await response.Content.ReadAsStringAsync());
                    group = (jo["channels"] as JArray).FirstOrDefault(x => x.Value<string>("name") == title);
                }
            }

            if (group == null)
            {
                throw new Exception("Group '" + title + "' was not found in Team Workspace.");
            }

            return group["id"].ToString();
        }

        private string MessageBuilder(string senderName, string subject, string messageText)
        {
            if (string.IsNullOrWhiteSpace(messageText))
                throw new ArgumentException("Message can't be empty");

            string message = messageText;
            bool isSenderNameExists = !string.IsNullOrEmpty(senderName);
            bool isSubjectExists = !string.IsNullOrEmpty(subject);

            if (isSenderNameExists || isSubjectExists)
            {
                message = "Повідомлення:\n" + message;

                if (isSubjectExists)
                    message = "Тема:\n" + subject + "\n\n" + message;

                if (isSenderNameExists)
                    message = "Від:\n" + senderName + "\n\n" + message;
            }

            return message;
        }

        private async Task SendMessage(string channelId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message can't be empty");

            var uri = new Uri(_slackApiLink + "chat.postMessage");
            using (HttpClient httpClient = new HttpClient())
            {
                var model = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("token", _slackConfigurations.UserToken),
                    new KeyValuePair<string, string>("channel", channelId),
                    new KeyValuePair<string, string>("text", message),
                    new KeyValuePair<string, string>("as_user", "true")
                };
                using (FormUrlEncodedContent content = new FormUrlEncodedContent(model))
                {
                    await httpClient.PostAsync(uri, content);
                }
            }
        }

        private async Task SendFile(string channelId, string filePath, string message, string fileType, string fileName)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("Path can't be empty");
            if (!File.Exists(filePath))
                throw new ArgumentException("Could not find file " + filePath);

            var uri = new Uri(_slackApiLink + "files.upload");
            using (HttpClient _httpClient = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent(_slackConfigurations.UserToken), "token");
                    formData.Add(new StringContent(channelId), "channels");
                    formData.Add(new StreamContent(File.OpenRead(filePath)), "file", fileName);
                    formData.Add(new StringContent(message), "initial_comment");
                    formData.Add(new StringContent(fileName), "filename");
                    await _httpClient.PostAsync(uri, formData);
                }
            }
        }
        #endregion
    }
}
