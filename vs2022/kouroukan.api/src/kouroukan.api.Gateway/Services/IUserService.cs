using Kouroukan.Api.Gateway.Models;

namespace Kouroukan.Api.Gateway.Services;

public interface IUserService
{
    Task<CreateUserResultDto> CreateUserAsync(int directorId, CreateUserRequest request, CancellationToken ct = default);
    Task<List<UserListItemDto>> GetUsersForCompanyAsync(int directorId, CancellationToken ct = default);
    Task<UserSearchResultDto?> SearchUserAsync(string query, CancellationToken ct = default);
    Task<List<CompanyDto>> GetCompaniesForUserAsync(int userId, CancellationToken ct = default);
    Task DeleteUserFromCompanyAsync(int directorId, int userId, CancellationToken ct = default);
}
