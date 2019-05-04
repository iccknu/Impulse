using DataTransferObjects.Configurations;
using DataTransferObjects.Social;
using Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Providers
{
    public class EmailProvider : ISocialProvider
    {
        private readonly EmailConfigurationsDto _emailConfigurations;
        private readonly int DileyTime;

        public EmailProvider(IOptions<EmailConfigurationsDto> emailConfigurations)
        {
            _emailConfigurations = emailConfigurations.Value;
            DileyTime = (int)(1000 / _emailConfigurations.MessagesPerSecond);
        }

        #region User Methods
        public async Task SendMessageToUserAsync(MessageToUserDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Message))
                throw new ArgumentException("Message can't be empty");

            if (!IsValidEmail(model.EmailOrUserNumber))
                throw new ArgumentException("Invalid email");

            MimeMessage message = new MimeMessage
            {
                Subject = model.Subject,
                Body = new TextPart(TextFormat.Plain)
                {
                    Text = model.Message
                }
            };

            message.To.Add(new MailboxAddress("User", model.EmailOrUserNumber));
            message.From.Add(new MailboxAddress(model.SenderName, _emailConfigurations.Email));
            

            using (SmtpClient emailClient = new SmtpClient())
            {
                emailClient.Connect(_emailConfigurations.SmtpServer, _emailConfigurations.SmtpPort, true);
                emailClient.Authenticate(_emailConfigurations.UserName, _emailConfigurations.Password);
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
            Thread.Sleep(DileyTime);
        }

        public async Task SendFileToUserAsync(FileToUserDto model)
        {
            if (!IsValidEmail(model.EmailOrUserNumber))
                throw new ArgumentException("Invalid email");
            if (string.IsNullOrEmpty(model.Path))
                throw new ArgumentException("Path can't be empty");
            if (!File.Exists(model.Path))
                throw new ArgumentException("Could not find file " + model.Path);

            BodyBuilder bodyBuilder = new BodyBuilder
            {
                TextBody = model.Caption
            };
            bodyBuilder.Attachments.Add(model.Path);

            MimeMessage message = new MimeMessage
            {
                Subject = model.Subject,
                Body = bodyBuilder.ToMessageBody()
            };

            message.To.Add(new MailboxAddress("User", model.EmailOrUserNumber));
            message.From.Add(new MailboxAddress(model.SenderName, _emailConfigurations.Email));


            using (SmtpClient emailClient = new SmtpClient())
            {
                emailClient.Connect(_emailConfigurations.SmtpServer, _emailConfigurations.SmtpPort, true);
                emailClient.Authenticate(_emailConfigurations.UserName, _emailConfigurations.Password);
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
            Thread.Sleep(DileyTime);
        }

        public async Task SendPhotoToUserAsync(FileToUserDto model)
        {
            if (!IsValidEmail(model.EmailOrUserNumber))
                throw new ArgumentException("Invalid email");
            if (string.IsNullOrEmpty(model.Path))
                throw new ArgumentException("Path can't be empty");
            if (!File.Exists(model.Path))
                throw new ArgumentException("Could not find file " + model.Path);

            BodyBuilder bodyBuilder = new BodyBuilder
            {
                TextBody = model.Caption
            };
            bodyBuilder.Attachments.Add(model.Path);

            MimeMessage message = new MimeMessage
            {
                Subject = model.Subject,
                Body = bodyBuilder.ToMessageBody()
            };

            message.To.Add(new MailboxAddress("User", model.EmailOrUserNumber));
            message.From.Add(new MailboxAddress(model.SenderName, _emailConfigurations.Email));


            using (SmtpClient emailClient = new SmtpClient())
            {
                emailClient.Connect(_emailConfigurations.SmtpServer, _emailConfigurations.SmtpPort, true);
                emailClient.Authenticate(_emailConfigurations.UserName, _emailConfigurations.Password);
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
            Thread.Sleep(DileyTime);
        }

        public async Task AddUserToContactsAsync(UserInfoDto model)
            => throw new NotImplementedException("This method is not supported in email");
        #endregion

        #region Channel Methods
        public async Task SendMessageToChannelAsync(MessageToChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task SendFileToChannelAsync(FileToChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task SendPhotoToChannelAsync(FileToChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task AddUserToChannelAsync(UserManipulationInChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task DeleteUserFromChannelAsync(UserManipulationInChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task CreateChannelAsync(ChannelOrGroupCreationDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task RemoveChannelAsync(string title)
            => throw new NotImplementedException("This method is not supported in email");
        #endregion

        #region Group Methods
        public async Task SendMessageToGroupAsync(MessageToChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task SendFileToGroupAsync(FileToChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task SendPhotoToGroupAsync(FileToChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task AddUserToGroupAsync(UserManipulationInChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task DeleteUserFromGroupAsync(UserManipulationInChannelOrGroupDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task CreateGroupAsync(ChannelOrGroupCreationDto model)
            => throw new NotImplementedException("This method is not supported in email");

        public async Task RemoveGroupAsync(string title)
            => throw new NotImplementedException("This method is not supported in email");
        #endregion

        #region Static Methods
        // From: https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        #endregion
    }
}
