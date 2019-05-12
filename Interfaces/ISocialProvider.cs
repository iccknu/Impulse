using DataTransferObjects.Social;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ISocialProvider
    {
        Task SendMessageToUserAsync(MessageToUserDto model);
        Task SendPhotoToUserAsync(FileToUserDto model);
        Task SendFileToUserAsync(FileToUserDto model);
        Task AddUserToContactsAsync(UserInfoDto model);

        Task SendMessageToChannelAsync(MessageToChannelOrGroupDto model);
        Task SendPhotoToChannelAsync(FileToChannelOrGroupDto model);
        Task SendFileToChannelAsync(FileToChannelOrGroupDto model);
        Task AddUserToChannelAsync(UserManipulationInChannelOrGroupDto model);
        Task DeleteUserFromChannelAsync(UserManipulationInChannelOrGroupDto model);
        Task CreateChannelAsync(ChannelOrGroupCreationDto model);
        Task RemoveChannelAsync(string title);

        Task SendMessageToGroupAsync(MessageToChannelOrGroupDto model);
        Task SendPhotoToGroupAsync(FileToChannelOrGroupDto model);
        Task SendFileToGroupAsync(FileToChannelOrGroupDto model);
        Task AddUserToGroupAsync(UserManipulationInChannelOrGroupDto model);
        Task DeleteUserFromGroupAsync(UserManipulationInChannelOrGroupDto model);
        Task CreateGroupAsync(ChannelOrGroupCreationDto model);
        Task RemoveGroupAsync(string title);

        Task<UserCheckResultDto> UserCheck(string emailOrUserNumber);
    }
}
