using System.Text.Json;
using Tier2Management.API.DTOs;

namespace Tier2Management.API.Services;

public class UserManagementService : IUserManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserManagementService> _logger;
    private readonly string _mockServerBaseUrl;

    public UserManagementService(HttpClient httpClient, IConfiguration configuration, ILogger<UserManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _mockServerBaseUrl = configuration["MockCollaborationServer:BaseUrl"] ?? "https://localhost:7001";
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/users/{username}");
            if (!response.IsSuccessStatusCode) return null;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            return new UserDto
            {
                Id = 0, // Mock doesn't use DB IDs
                UserName = data.GetProperty("username").GetString() ?? "",
                DisplayName = data.GetProperty("displayName").GetString() ?? "",
                Email = data.GetProperty("email").GetString() ?? "",
                Department = data.TryGetProperty("department", out var dept) ? dept.GetString() : null,
                Title = data.TryGetProperty("title", out var title) ? title.GetString() : null,
                IsEnabled = data.GetProperty("isEnabled").GetBoolean(),
                LastLogon = data.TryGetProperty("lastLogon", out var lastLogon) && lastLogon.ValueKind != JsonValueKind.Null 
                    ? lastLogon.GetDateTime() : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {Username}", username);
            return null;
        }
    }

    public async Task<List<string>> GetUserAdGroupsAsync(string username)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/users/{username}/ad-groups");
            if (!response.IsSuccessStatusCode) return new List<string>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            var groups = new List<string>();
            if (data.TryGetProperty("groups", out var groupsElement) && groupsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var group in groupsElement.EnumerateArray())
                {
                    if (group.TryGetProperty("name", out var name))
                    {
                        groups.Add(name.GetString() ?? "");
                    }
                }
            }

            return groups;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving AD groups for user {Username}", username);
            return new List<string>();
        }
    }

    public async Task<UserAccessResultDto> CheckUserAccessAsync(string username, Guid itemId, string requiredAccess)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{_mockServerBaseUrl}/api/users/{username}/permissions/{itemId}?requiredAccess={requiredAccess}");

            var result = new UserAccessResultDto
            {
                UserName = username,
                ItemId = itemId,
                RequestedPermission = Enum.Parse<Models.PermissionType>(requiredAccess, true),
                HasAccess = response.IsSuccessStatusCode
            };

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
                
                result.DisplayName = data.GetProperty("displayName").GetString() ?? "";
                result.HasAccess = data.GetProperty("hasAccess").GetBoolean();
                result.DenialReason = data.TryGetProperty("denialReason", out var denial) ? denial.GetString() : null;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user access for {Username}", username);
            return new UserAccessResultDto
            {
                UserName = username,
                ItemId = itemId,
                HasAccess = false,
                DenialReason = "שגיאה בבדיקת ההרשאות"
            };
        }
    }

    public async Task<List<UserDto>> SearchUsersAsync(string? searchTerm = null)
    {
        try
        {
            var url = $"{_mockServerBaseUrl}/api/users";
            if (!string.IsNullOrEmpty(searchTerm))
                url += $"?search={Uri.EscapeDataString(searchTerm)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<UserDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            var users = new List<UserDto>();
            if (data.TryGetProperty("users", out var usersElement) && usersElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var userElement in usersElement.EnumerateArray())
                {
                    var user = new UserDto
                    {
                        Id = 0,
                        UserName = userElement.GetProperty("username").GetString() ?? "",
                        DisplayName = userElement.GetProperty("displayName").GetString() ?? "",
                        Email = userElement.GetProperty("email").GetString() ?? "",
                        Department = userElement.TryGetProperty("department", out var dept) ? dept.GetString() : null,
                        Title = userElement.TryGetProperty("title", out var title) ? title.GetString() : null,
                        IsEnabled = userElement.GetProperty("isEnabled").GetBoolean()
                    };
                    users.Add(user);
                }
            }

            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            return new List<UserDto>();
        }
    }

    public async Task<List<GroupDto>> GetUserGroupsAsync(string username)
    {
        try
        {
            var groups = await GetUserAdGroupsAsync(username);
            return groups.Select(groupName => new GroupDto
            {
                Id = 0,
                Name = groupName,
                GroupType = Models.GroupType.Security,
                IsEnabled = true
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user groups");
            return new List<GroupDto>();
        }
    }

    public async Task<List<UserSummaryDto>> GetActiveUsersAsync()
    {
        try
        {
            var users = await SearchUsersAsync();
            return users.Where(u => u.IsEnabled)
                       .Select(u => new UserSummaryDto
                       {
                           Id = u.Id,
                           UserName = u.UserName,
                           Email = u.Email,
                           DisplayName = u.DisplayName ?? u.UserName,
                           Department = u.Department,
                           Title = u.Title,
                           IsEnabled = u.IsEnabled,
                           LastLogon = u.LastLogon
                       }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active users");
            return new List<UserSummaryDto>();
        }
    }

    public async Task<ADSyncStatusDto> GetAdSyncStatusAsync()
    {
        // Mock AD sync status
        return new ADSyncStatusDto
        {
            LastSyncTime = DateTime.UtcNow.AddHours(-1),
            IsInProgress = false,
            Status = "הושלם בהצלחה",
            TotalUsers = 150,
            TotalGroups = 25,
            UsersAdded = 2,
            UsersUpdated = 8,
            GroupsAdded = 0,
            GroupsUpdated = 1,
            ErrorCount = 0,
            SyncDuration = TimeSpan.FromMinutes(5)
        };
    }
} 