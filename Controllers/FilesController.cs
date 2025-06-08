using Microsoft.AspNetCore.Mvc;
using Tier2Management.API.DTOs;
using Tier2Management.API.Services;

namespace Tier2Management.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileManagementService fileManagementService, ILogger<FilesController> logger)
    {
        _fileManagementService = fileManagementService;
        _logger = logger;
    }

    /// <summary>
    /// שליפת רשימת קבצים נעולים (Get locked files)
    /// </summary>
    [HttpGet("locked")]
    public async Task<ActionResult<List<FileLockDto>>> GetLockedFiles()
    {
        try
        {
            var lockedFiles = await _fileManagementService.GetLockedFilesAsync();
            return Ok(lockedFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving locked files");
            return StatusCode(500, new { message = "Error retrieving locked files" });
        }
    }

    /// <summary>
    /// שחרור נעילת קובץ (Unlock file)
    /// </summary>
    [HttpPost("{fileId}/unlock")]
    public async Task<ActionResult> UnlockFile(Guid fileId, [FromBody] ForceUnlockFileDto request)
    {
        try
        {
            var success = await _fileManagementService.UnlockFileAsync(fileId, "admin", request.Justification);
            
            if (!success)
                return NotFound(new { message = "File not found or not locked" });

            return Ok(new { success = true, message = "File unlocked successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking file");
            return StatusCode(500, new { message = "Error unlocking file" });
        }
    }

    /// <summary>
    /// שליפת גרסאות קובץ (Get file versions)
    /// </summary>
    [HttpGet("{fileId}/versions")]
    public async Task<ActionResult<List<FileVersionDto>>> GetFileVersions(Guid fileId)
    {
        try
        {
            var versions = await _fileManagementService.GetFileVersionsAsync(fileId);
            return Ok(versions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file versions");
            return StatusCode(500, new { message = "Error retrieving file versions" });
        }
    }

    /// <summary>
    /// שחזור קובץ לגרסה קודמת (Restore file version)
    /// </summary>
    [HttpPost("{fileId}/restore-version")]
    public async Task<ActionResult> RestoreFileVersion(Guid fileId, [FromBody] RestoreFileVersionDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Justification))
                return BadRequest(new { message = "נדרש הסבר לשחזור הגרסה" });

            var success = await _fileManagementService.RestoreFileVersionAsync(fileId, request.VersionId, "admin");
            
            if (!success)
                return NotFound(new { message = "קובץ או גרסה לא נמצאו" });

            return Ok(new { 
                success = true, 
                message = "הקובץ שוחזר בהצלחה לגרסה קודמת",
                fileId = fileId,
                restoredToVersion = request.VersionId,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring file version");
            return StatusCode(500, new { message = "שגיאה בשחזור הקובץ" });
        }
    }

    /// <summary>
    /// שליפת הרשאות קובץ (Get file permissions)
    /// </summary>
    [HttpGet("{fileId}/permissions")]
    public async Task<ActionResult<List<UserPermissionDto>>> GetFilePermissions(Guid fileId)
    {
        try
        {
            var permissions = await _fileManagementService.GetItemPermissionsAsync(fileId);
            return Ok(permissions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file permissions");
            return StatusCode(500, new { message = "שגיאה בשליפת הרשאות הקובץ" });
        }
    }

    /// <summary>
    /// בדיקת הרשאות משתמש על קובץ (Check user access to file)
    /// </summary>
    [HttpGet("{fileId}/check-access/{username}")]
    public async Task<ActionResult<UserAccessResultDto>> CheckUserAccess(Guid fileId, string username, [FromQuery] string requiredAccess = "Read")
    {
        try
        {
            var result = await _fileManagementService.CheckUserAccessAsync(username, fileId, requiredAccess);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user access");
            return StatusCode(500, new { message = "שגיאה בבדיקת הרשאות המשתמש" });
        }
    }

    /// <summary>
    /// שליפת היסטוריית שינויי מידור (Get classification change history)
    /// </summary>
    [HttpGet("{fileId}/classification-history")]
    public async Task<ActionResult<List<ClassificationChangeLogDto>>> GetClassificationHistory(Guid fileId)
    {
        try
        {
            var history = await _fileManagementService.GetClassificationHistoryAsync(fileId);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving classification history");
            return StatusCode(500, new { message = "שגיאה בשליפת היסטוריית המידור" });
        }
    }
}

/// <summary>
/// פריטים מחוקים (Deleted items controller)
/// </summary>
[ApiController]
[Route("api/deleted-items")]
public class DeletedItemsController : ControllerBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly ILogger<DeletedItemsController> _logger;

    public DeletedItemsController(IFileManagementService fileManagementService, ILogger<DeletedItemsController> logger)
    {
        _fileManagementService = fileManagementService;
        _logger = logger;
    }

    /// <summary>
    /// שליפת פריטים מחוקים (Get deleted items)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<DeletedItemDto>>> GetDeletedItems()
    {
        try
        {
            var deletedItems = await _fileManagementService.GetDeletedItemsAsync();
            return Ok(deletedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving deleted items");
            return StatusCode(500, new { message = "שגיאה בשליפת פריטים מחוקים" });
        }
    }

    /// <summary>
    /// שחזור פריט מחוק (Restore deleted item)
    /// </summary>
    [HttpPost("{itemId}/restore")]
    public async Task<ActionResult> RestoreDeletedItem(Guid itemId, [FromBody] RecoverItemDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Justification))
                return BadRequest(new { message = "נדרש הסבר לשחזור הפריט" });

            var success = await _fileManagementService.RestoreDeletedItemAsync(itemId, "admin");
            
            if (!success)
                return NotFound(new { message = "פריט לא נמצא או לא יכול להישחזר" });

            return Ok(new { 
                success = true, 
                message = "הפריט שוחזר בהצלחה",
                itemId = itemId,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring deleted item");
            return StatusCode(500, new { message = "שגיאה בשחזור הפריט" });
        }
    }
} 