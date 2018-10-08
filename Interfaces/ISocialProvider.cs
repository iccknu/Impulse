using DataTransferObjects.Social;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface ISocialProvider
    {
        Task SendMessageToUserAsync(MessageToUsersDto model);
        Task SendMessageToChannelAsync(MessageToChannelDto model);
        Task SendPhotoToUserAsync(FileToUsersDto model);
        Task SendPhotoToChannelAsync(FileToChannelDto model);
        Task SendFileToUserAsync(FileToUsersDto model);
        Task SendFileToChannelAsync(FileToChannelDto model);
        Task AddUserToChannelAsync(UserManipulationInChannelDto model);
        Task DeleteUserFromChannelAsync(UserManipulationInChannelDto model);
    }
}
