using Tier2Management.API.DTOs;

namespace Tier2Management.API.Services;

public interface IFileManagementService
{
    Task<List<FileLockDto>> GetLockedFilesAsync();
    Task<bool> UnlockFileAsync(Guid fileId, string adminUser, string justification);
    Task<List<FileVersionDto>> GetFileVersionsAsync(Guid fileId);
    Task<bool> RestoreFileVersionAsync(Guid fileId, int versionNumber, string adminUser);
    Task<List<DeletedItemDto>> GetDeletedItemsAsync();
    Task<bool> RestoreDeletedItemAsync(Guid itemId, string adminUser);
    Task<List<UserPermissionDto>> GetItemPermissionsAsync(Guid itemId);
    Task<UserAccessResultDto> CheckUserAccessAsync(string username, Guid itemId, string requiredAccess);
    Task<List<ClassificationChangeLogDto>> GetClassificationHistoryAsync(Guid itemId);
} 