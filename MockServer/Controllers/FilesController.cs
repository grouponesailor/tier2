using Microsoft.AspNetCore.Mvc;
using MockCollaborationServer.Models;
using MockCollaborationServer.Services;
using System.IO;

namespace MockCollaborationServer.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly MockDataService _dataService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(MockDataService dataService, ILogger<FilesController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    /// <summary>
    /// Get file views data
    /// </summary>
    [HttpGet("views/id/{fileId}")]
    public ActionResult<FileViewsResponse> GetFileViews(string fileId)
    {
        try
        {
            _logger.LogInformation("Mock file views request received for file ID: {FileId}", fileId);

            var random = new Random();
            var users = new[] { "011111", "022222", "033333", "044444", "055555" };
            
            // Generate mock view data
            var views = new List<FileViewInfo>();
            var numViewers = random.Next(2, 5); // 2-4 viewers
            
            for (int i = 0; i < numViewers; i++)
            {
                var userId = users[i];
                var viewCount = random.Next(1, 10);
                var firstViewDays = random.Next(1, 90);
                var lastViewDays = random.Next(0, firstViewDays);
                
                views.Add(new FileViewInfo
                {
                    UserId = userId,
                    ViewCounter = viewCount,
                    FirstViewDate = DateTime.UtcNow.AddDays(-firstViewDays),
                    LastViewDate = DateTime.UtcNow.AddDays(-lastViewDays)
                });
            }

            var response = new FileViewsResponse
            {
                ItemId = fileId,
                Views = views,
                ResponseHeader = new ResponseHeader { ReqId = Guid.NewGuid().ToString("N")[..8] },
                Ex = null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file views for file ID: {FileId}", fileId);
            
            return Ok(new FileViewsResponse
            {
                ItemId = fileId,
                Views = new List<FileViewInfo>(),
                ResponseHeader = new ResponseHeader { ReqId = Guid.NewGuid().ToString("N")[..8] },
                Ex = ex.Message
            });
        }
    }

    /// <summary>
    /// Get lock status for a specific file
    /// </summary>
    [HttpGet("{id}/lock-status")]
    public ActionResult<object> GetLockStatus(Guid id)
    {
        var file = _dataService.GetFileById(id);
        if (file == null)
            return NotFound(new { message = "File not found" });

        return Ok(new
        {
            fileId = file.Id,
            fileName = file.Name,
            isLocked = file.IsLocked,
            lockedBy = file.LockedBy,
            lockTimestamp = file.LockTimestamp,
            canUnlock = file.IsLocked
        });
    }

    /// <summary>
    /// Unlock a specific file (Updated to use new request/response format)
    /// </summary>
    [HttpPost("unlock")]
    public ActionResult<UnlockResponse> UnlockFileById([FromBody] UnlockRequest request)
    {
        try
        {
            _logger.LogInformation("Mock unlock request received for file ID: {FileId}, Request ID: {ReqId}", 
                request.RequestBody.Id, request.RequestHeader.ReqId);

            // Try to parse file ID from request body
            if (!Guid.TryParse(request.RequestBody.Id, out var fileId))
            {
                return Ok(new UnlockResponse
                {
                    Id = request.RequestBody.Id,
                    Locked = true,
                    Signature = string.Empty,
                    Etag = string.Empty,
                    ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader.ReqId },
                    Ex = "Invalid file ID format"
                });
            }

            var success = _dataService.UnlockFile(fileId, "system", "API unlock request");
            
            var response = new UnlockResponse
            {
                Id = request.RequestBody.Id,
                Locked = !success,
                Signature = success ? Guid.NewGuid().ToString("N")[..16] : string.Empty,
                Etag = success ? Guid.NewGuid().ToString("N")[..16] : string.Empty,
                ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader.ReqId },
                Ex = success ? null : "File not found or not locked"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking file in mock server");
            return Ok(new UnlockResponse
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
    /// Search files with POST request and JSON body (Updated format)
    /// </summary>
    [HttpPost]
    public ActionResult<FileSearchResponse> SearchFilesPost([FromBody] FileSearchRequest request)
    {
        try
        {
            _logger.LogInformation("Mock files search POST request received. Query: {Query}, System: {System}, UUID: {UUID}", 
                request.Q, request.System, request.Uuid);

            var searchResults = _dataService.SearchFiles(request.Q, "hide_classified");
            var random = new Random();

            // Apply sorting if specified
            if (request.Sort?.Any() == true)
            {
                var sortField = request.Sort.First();
                searchResults = sortField.Field.ToLower() switch
                {
                    "updatedate" => sortField.Order.ToLower() == "desc" 
                        ? searchResults.OrderByDescending(f => f.CreatedAt)
                        : searchResults.OrderBy(f => f.CreatedAt),
                    "name" => sortField.Order.ToLower() == "desc"
                        ? searchResults.OrderByDescending(f => f.Name)
                        : searchResults.OrderBy(f => f.Name),
                    "size" => sortField.Order.ToLower() == "desc"
                        ? searchResults.OrderByDescending(f => f.Size)
                        : searchResults.OrderBy(f => f.Size),
                    _ => searchResults.OrderByDescending(f => f.CreatedAt)
                };
            }

            // Convert to search response format
            var response = new FileSearchResponse
            {
                Paging = new PagingInfo
                {
                    PitId = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    Sort = new List<long> { random.NextInt64(1000000000, 9999999999), random.NextInt64(100000000000, 999999999999) }
                },
                Hits = searchResults.Select(file => new FileSearchHit
                {
                    Metadata = new FileMetadata
                    {
                        AuthorizationLevel = file.Classification.Level == "Secret" || file.Classification.Level == "Confidential" 
                            ? "RESTRICTED" : "FULLY_AUTHORIZED",
                        Extension = file.Extension ?? string.Empty,
                        UpdateDate = file.CreatedAt.AddDays(random.Next(1, 30)),
                        UpdateId = Guid.NewGuid().ToString("N")[..8],
                        FullNamePath = file.Path,
                        AcExternalId = string.Empty,
                        AcInheriteType = 0,
                        OwnerId = file.CreatedBy ?? "system",
                        Type = 1,
                        FullPath = file.Path,
                        LastOperationDate = DateTime.UtcNow.AddMinutes(-random.Next(1, 1440)),
                        ParentId = Guid.NewGuid().ToString("N")[..8],
                        LastOperationByUser = file.CreatedBy ?? "system",
                        ParentName = GetParentFolderName(file.Path),
                        Size = file.Size,
                        LastOperation = file.IsLocked ? "file_locked" : "file_saved",
                        Name = Path.GetFileNameWithoutExtension(file.Name),
                        Attributes = new List<object>(),
                        Id = file.Id.ToString("N")[..8],
                        Status = file.IsDeleted ? 0 : 1,
                        CreateDate = file.CreatedAt
                    }
                }).ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files in mock server");
            return StatusCode(500, new { error = "Internal server error", message = ex.Message });
        }
    }

    /// <summary>
    /// Helper method to extract parent folder name from file path
    /// </summary>
    private static string GetParentFolderName(string filePath)
    {
        try
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(directoryPath) || directoryPath == "/")
                return "Root";
            
            return Path.GetFileName(directoryPath) ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    /// <summary>
    /// Get all versions for a specific file
    /// </summary>
    [HttpGet("{id}/versions")]
    public ActionResult<object> GetFileVersions(Guid id)
    {
        var file = _dataService.GetFileById(id);
        if (file == null)
            return NotFound(new { message = "File not found" });

        return Ok(new
        {
            fileId = file.Id,
            fileName = file.Name,
            versions = file.Versions.Select(v => new
            {
                versionId = v.VersionId,
                versionNumber = v.VersionNumber,
                createdBy = v.CreatedBy,
                createdAt = v.CreatedAt,
                size = v.Size,
                comments = v.Comments,
                isCurrent = v.IsCurrent
            }).OrderByDescending(v => v.versionNumber)
        });
    }

    /// <summary>
    /// Restore file to a specific version
    /// </summary>
    [HttpPost("{id}/restore-version")]
    public ActionResult<object> RestoreFileVersion(Guid id, [FromBody] RestoreVersionRequest request)
    {
        if (string.IsNullOrEmpty(request.AdminUser))
            return BadRequest(new { message = "AdminUser is required" });

        var success = _dataService.RestoreFileVersion(id, request.VersionNumber, request.AdminUser);
        
        if (!success)
            return BadRequest(new { message = "File not found or version not found" });

        return Ok(new
        {
            success = true,
            message = "File version restored successfully",
            fileId = id,
            restoredToVersion = request.VersionNumber,
            restoredBy = request.AdminUser,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get permissions for a specific file
    /// </summary>
    [HttpGet("{id}/permissions")]
    public ActionResult<object> GetFilePermissions(Guid id)
    {
        var file = _dataService.GetFileById(id);
        if (file == null)
            return NotFound(new { message = "File not found" });

        return Ok(new
        {
            fileId = file.Id,
            fileName = file.Name,
            filePath = file.Path,
            permissions = file.Permissions.Select(p => new
            {
                principalId = p.PrincipalId,
                principalType = p.PrincipalType,
                accessLevel = p.AccessLevel,
                isInherited = p.IsInherited,
                inheritedFrom = p.InheritedFrom,
                expiryDate = p.ExpiryDate
            })
        });
    }

    /// <summary>
    /// Get classification change history for a file
    /// </summary>
    [HttpGet("{id}/classification-history")]
    public ActionResult<object> GetClassificationHistory(Guid id)
    {
        var file = _dataService.GetFileById(id);
        if (file == null)
            return NotFound(new { message = "File not found" });

        return Ok(new
        {
            fileId = file.Id,
            fileName = file.Name,
            currentClassification = file.Classification.Level,
            history = file.Classification.History.Select(h => new
            {
                id = h.Id,
                previousLevel = h.PreviousLevel,
                newLevel = h.NewLevel,
                changedBy = h.ChangedBy,
                changedAt = h.ChangedAt,
                justification = h.Justification
            }).OrderByDescending(h => h.changedAt)
        });
    }

    /// <summary>
    /// Get all locked files
    /// </summary>
    [HttpGet("locked")]
    public ActionResult<object> GetLockedFiles()
    {
        var lockedFiles = _dataService.GetLockedFiles();

        return Ok(new
        {
            count = lockedFiles.Count(),
            files = lockedFiles.Select(f => new
            {
                fileId = f.Id,
                fileName = f.Name,
                filePath = f.Path,
                lockedBy = f.LockedBy,
                lockTimestamp = f.LockTimestamp,
                size = f.Size
            })
        });
    }

    /// <summary>
    /// Restore a file
    /// </summary>
    [HttpPut("restore")]
    public ActionResult<FileRestoreResponse> RestoreFile([FromBody] FileRestoreRequest request)
    {
        try
        {
            _logger.LogInformation("Mock file restore request received for ID: {Id}, Request ID: {ReqId}", 
                request.RequestBody.Id, request.RequestHeader.ReqId);

            // Mock file restore operation
            var response = new FileRestoreResponse
            {
                Id = request.RequestBody.Id,
                ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader.ReqId },
                Ex = null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring file in mock server");
            return Ok(new FileRestoreResponse
            {
                Id = request.RequestBody?.Id ?? string.Empty,
                ResponseHeader = new ResponseHeader { ReqId = request.RequestHeader?.ReqId ?? string.Empty },
                Ex = ex.Message
            });
        }
    }
}

// Legacy format
public class UnlockFileRequest
{
    public string AdminUser { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
}

// New unlock request/response DTOs
public class RequestHeader
{
    public string ReqId { get; set; } = string.Empty;
    public int CallingSystemId { get; set; }
}

public class UnlockRequestBody
{
    public string Id { get; set; } = string.Empty;
}

public class UnlockRequest
{
    public RequestHeader RequestHeader { get; set; } = new();
    public UnlockRequestBody RequestBody { get; set; } = new();
}

public class UnlockResponse
{
    public string Id { get; set; } = string.Empty;
    public bool Locked { get; set; }
    public string Signature { get; set; } = string.Empty;
    public string Etag { get; set; } = string.Empty;
    public Models.ResponseHeader ResponseHeader { get; set; } = new();
    public string? Ex { get; set; }
}

public class RestoreVersionRequest
{
    public int VersionNumber { get; set; }
    public string AdminUser { get; set; } = string.Empty;
    public string? Comments { get; set; }
}

// File restore DTOs
public class FileRestoreRequestBody
{
    public string Id { get; set; } = string.Empty;
    public bool Override { get; set; }
    public string? NewParentId { get; set; }
}

public class FileRestoreRequest
{
    public RequestHeader RequestHeader { get; set; } = new();
    public FileRestoreRequestBody RequestBody { get; set; } = new();
}

public class FileRestoreResponse
{
    public string Id { get; set; } = string.Empty;
    public Models.ResponseHeader ResponseHeader { get; set; } = new();
    public string? Ex { get; set; }
} 