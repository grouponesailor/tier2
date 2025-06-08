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
    [HttpPost("unlock")]
    public async Task<ActionResult<UnlockFileResponse>> UnlockFile([FromBody] UnlockFileRequest request)
    {
        try
        {
            _logger.LogInformation("Unlock request received for file ID: {FileId}, Request ID: {ReqId}", 
                request.RequestBody.Id, request.RequestHeader.ReqId);

            // Try to parse file ID from request body
            if (!Guid.TryParse(request.RequestBody.Id, out var fileId))
            {
                return Ok(new UnlockFileResponse
                {
                    Id = request.RequestBody.Id,
                    Locked = true,
                    Signature = string.Empty,
                    Etag = string.Empty,
                    ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader.ReqId },
                    Ex = "Invalid file ID format"
                });
            }

            var success = await _fileManagementService.UnlockFileAsync(fileId, "admin", "API unlock request");
            
            var response = new UnlockFileResponse
            {
                Id = request.RequestBody.Id,
                Locked = !success,
                Signature = success ? Guid.NewGuid().ToString("N") : string.Empty,
                Etag = success ? Guid.NewGuid().ToString("N") : string.Empty,
                ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader.ReqId },
                Ex = success ? null : "File not found or not locked"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking file");
            return Ok(new UnlockFileResponse
            {
                Id = request.RequestBody?.Id ?? string.Empty,
                Locked = true,
                Signature = string.Empty,
                Etag = string.Empty,
                ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader?.ReqId ?? string.Empty },
                Ex = ex.Message
            });
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
/// בקר חיפוש קבצים (Files search controller)
/// </summary>
[ApiController]
[Route("files")]
public class FilesSearchController : ControllerBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly ILogger<FilesSearchController> _logger;

    public FilesSearchController(IFileManagementService fileManagementService, ILogger<FilesSearchController> logger)
    {
        _fileManagementService = fileManagementService;
        _logger = logger;
    }

    /// <summary>
    /// חיפוש קבצים (Search files)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<FilesSearchResponse>> SearchFiles([FromBody] FilesSearchRequest request, [FromQuery] string? classification = null)
    {
        try
        {
            _logger.LogInformation("Files search request received. Query: {Query}, System: {System}, UUID: {Uuid}, Classification: {Classification}", 
                request.Q, request.System, request.Uuid, classification);

            // Mock implementation - in real scenario, this would search the file system
            var response = new FilesSearchResponse
            {
                Paging = new PagingInfo
                {
                    PitId = "aaa=efgrwhfrwf",
                    Sort = new List<long> { 2658136474, 478034234238 }
                },
                Hits = new List<FileHit>
                {
                    new FileHit
                    {
                        Metadata = new FileMetadata
                        {
                            AuthorizationLevel = "FULLY_ATHORIZED",
                            Extension = "pptx",
                            UpdateDate = DateTime.Parse("2025-06-03T12:12:13"),
                            UpdateId = "",
                            FullNamePath = "/app/im/aaa/bbb/ccc.pptx",
                            AcExternalId = "",
                            AcInheriteType = 0,
                            OwnerId = "0111111",
                            Type = 1,
                            FullPath = "/app/im/aaa/bbb/ccc.pptx",
                            LastOperationDate = DateTime.Parse("2025-06-03T12:12:13.770816"),
                            ParentId = "222",
                            LastOperationByUser = "0222222",
                            ParentName = "Shoki Hahamod",
                            Size = 277882,
                            LastOperation = "file_saved",
                            Name = "fwqded",
                            Attributes = new List<object>(),
                            Id = "111",
                            Status = 1,
                            CreateDate = DateTime.Parse("2025-06-03T12:12:13")
                        }
                    }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files");
            return StatusCode(500, new { message = "שגיאה בחיפוש קבצים" });
        }
    }
}

/// <summary>
/// בקר שחזור קבצים (File restore controller)
/// </summary>
[ApiController]
[Route("api/file")]
public class FileRestoreController : ControllerBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly ILogger<FileRestoreController> _logger;

    public FileRestoreController(IFileManagementService fileManagementService, ILogger<FileRestoreController> logger)
    {
        _fileManagementService = fileManagementService;
        _logger = logger;
    }

    /// <summary>
    /// שחזור קובץ (Restore file)
    /// </summary>
    [HttpPut("restore")]
    public async Task<ActionResult<RestoreResponse>> RestoreFile([FromBody] RestoreRequest request)
    {
        try
        {
            _logger.LogInformation("File restore request received for ID: {Id}, Request ID: {ReqId}", 
                request.RequestBody.Id, request.RequestHeader.ReqId);

            // Mock implementation - in real scenario, this would restore the file
            var response = new RestoreResponse
            {
                Id = request.RequestBody.Id,
                ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader.ReqId },
                Ex = null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring file");
            return Ok(new RestoreResponse
            {
                Id = request.RequestBody?.Id ?? string.Empty,
                ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader?.ReqId ?? string.Empty },
                Ex = ex.Message
            });
        }
    }
}

/// <summary>
/// בקר שחזור תיקיות (Directory restore controller)
/// </summary>
[ApiController]
[Route("api/directory")]
public class DirectoryRestoreController : ControllerBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly ILogger<DirectoryRestoreController> _logger;

    public DirectoryRestoreController(IFileManagementService fileManagementService, ILogger<DirectoryRestoreController> logger)
    {
        _fileManagementService = fileManagementService;
        _logger = logger;
    }

    /// <summary>
    /// שחזור תיקייה (Restore directory)
    /// </summary>
    [HttpPut("restore")]
    public async Task<ActionResult<RestoreResponse>> RestoreDirectory([FromBody] RestoreRequest request)
    {
        try
        {
            _logger.LogInformation("Directory restore request received for ID: {Id}, Request ID: {ReqId}", 
                request.RequestBody.Id, request.RequestHeader.ReqId);

            // Mock implementation - in real scenario, this would restore the directory
            var response = new RestoreResponse
            {
                Id = request.RequestBody.Id,
                ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader.ReqId },
                Ex = null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring directory");
            return Ok(new RestoreResponse
            {
                Id = request.RequestBody?.Id ?? string.Empty,
                ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader?.ReqId ?? string.Empty },
                Ex = ex.Message
            });
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


} 