using Tier2Management.API.DTOs;

namespace Tier2Management.API.Services;

public interface IUserManagementService
{
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<List<string>> GetUserAdGroupsAsync(string username);
    Task<UserAccessResultDto> CheckUserAccessAsync(string username, Guid itemId, string requiredAccess);
    Task<List<UserDto>> SearchUsersAsync(string? searchTerm = null);
    Task<List<GroupDto>> GetUserGroupsAsync(string username);
    Task<List<UserSummaryDto>> GetActiveUsersAsync();
    Task<ADSyncStatusDto> GetAdSyncStatusAsync();
} 