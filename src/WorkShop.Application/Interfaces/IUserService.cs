using WorkShop.Application.Models.Response;

namespace WorkShop.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponseModel>> GetAllUsersAsync();
    Task<UserResponseModel> GetUserByIdAsync(int id);
    Task BlockUserAsync(int id);
    Task UnblockUserAsync(int id);
}
