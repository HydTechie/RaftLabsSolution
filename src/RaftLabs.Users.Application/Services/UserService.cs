using RaftLabs.Users.Core.Interfaces;
using RaftLabs.Users.Core.Models;
using RaftLabs.Users.Infrastructure.Models;
using RaftLabs.Users.Infrastructure.Services;
namespace RaftLabs.Users.Application;
public class UserService : IUserService
{
    private readonly ExternalUserApiClient _apiClient;

    public UserService(ExternalUserApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var dto = await _apiClient.GetUserByIdAsync(id);
        return dto == null ? null : Map(dto);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var dtos = await _apiClient.GetAllUsersAsync();
        return dtos.Select(Map);
    }

    private User Map(ApiUserDto dto) => new()
    {
        Id = dto.Id,
        Email = dto.Email,
        FirstName = dto.First_Name,
        LastName = dto.Last_Name,
        Avatar = dto.Avatar
    };
}