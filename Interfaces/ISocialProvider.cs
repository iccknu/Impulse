using DataTransferObjects.Social;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ISocialProvider
    {
        Task SendMessageToUsersAsync(MessageToUsersDto model);
        Task SendMessageToChannelAsync(MessageToChannelDto model);
        Task SendPhotoToUsersAsync(FileToUsersDto model);
        Task SendPhotoToChannelAsync(FileToChannelDto model);
        Task SendFileToUsersAsync(FileToUsersDto model);
        Task SendFileToChannelAsync(FileToChannelDto model);
        Task AddUserToContactsAsync(UserInfoDto model);
        Task AddUserToChannelAsync(UserManipulationInChannelDto model);
        Task DeleteUserFromChannelAsync(UserManipulationInChannelDto model);
    }
}
