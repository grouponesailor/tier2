using System.Text;
using System.Text.Json;
using Tier2Management.API.DTOs;

namespace Tier2Management.API.Services;

public class FileManagementService : IFileManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FileManagementService> _logger;
    private readonly string _mockServerBaseUrl;

    public FileManagementService(HttpClient httpClient, IConfiguration configuration, ILogger<FileManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _mockServerBaseUrl = configuration["MockCollaborationServer:BaseUrl"] ?? "https://localhost:7001";
    }

    public async Task<List<FileLockDto>> GetLockedFilesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/files/locked");
            if (!response.IsSuccessStatusCode) return new List<FileLockDto>();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Retrieved locked files from mock server");
            
            // Parse and return simplified data
            return new List<FileLockDto>(); // Simplified for now
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving locked files");
            return new List<FileLockDto>();
        }
    }

    public async Task<bool> UnlockFileAsync(Guid fileId, string adminUser, string justification)
    {
        try
        {
            var request = new { AdminUser = adminUser, Justification = justification };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_mockServerBaseUrl}/api/files/{fileId}/unlock", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking file {FileId}", fileId);
            return false;
        }
    }

    public async Task<List<FileVersionDto>> GetFileVersionsAsync(Guid fileId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/files/{fileId}/versions");
            return response.IsSuccessStatusCode ? new List<FileVersionDto>() : new List<FileVersionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file versions");
            return new List<FileVersionDto>();
        }
    }

    public async Task<bool> RestoreFileVersionAsync(Guid fileId, int versionNumber, string adminUser)
    {
        try
        {
            var request = new { VersionNumber = versionNumber, AdminUser = adminUser };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_mockServerBaseUrl}/api/files/{fileId}/restore-version", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring file version");
            return false;
        }
    }

    public async Task<List<DeletedItemDto>> GetDeletedItemsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/deleted-items");
            return response.IsSuccessStatusCode ? new List<DeletedItemDto>() : new List<DeletedItemDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving deleted items");
            return new List<DeletedItemDto>();
        }
    }

    public async Task<bool> RestoreDeletedItemAsync(Guid itemId, string adminUser)
    {
        try
        {
            var request = new { AdminUser = adminUser };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_mockServerBaseUrl}/api/deleted-items/{itemId}/restore", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring deleted item");
            return false;
        }
    }

    public async Task<List<UserPermissionDto>> GetItemPermissionsAsync(Guid itemId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/files/{itemId}/permissions");
            return response.IsSuccessStatusCode ? new List<UserPermissionDto>() : new List<UserPermissionDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions");
            return new List<UserPermissionDto>();
        }
    }

    public async Task<UserAccessResultDto> CheckUserAccessAsync(string username, Guid itemId, string requiredAccess)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/users/{username}/permissions/{itemId}");
            return new UserAccessResultDto { UserName = username, ItemId = itemId, HasAccess = response.IsSuccessStatusCode };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user access");
            return new UserAccessResultDto { UserName = username, ItemId = itemId, HasAccess = false };
        }
    }

    public async Task<List<ClassificationChangeLogDto>> GetClassificationHistoryAsync(Guid itemId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/files/{itemId}/classification-history");
            return response.IsSuccessStatusCode ? new List<ClassificationChangeLogDto>() : new List<ClassificationChangeLogDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving classification history");
            return new List<ClassificationChangeLogDto>();
        }
    }
} 