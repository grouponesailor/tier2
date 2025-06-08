using Microsoft.AspNetCore.Mvc;
using MockCollaborationServer.Models;
using MockCollaborationServer.Services;

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

public class ResponseHeader
{
    public string ReqId { get; set; } = string.Empty;
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
    public ResponseHeader ResponseHeader { get; set; } = new();
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
    public ResponseHeader ResponseHeader { get; set; } = new();
    public string? Ex { get; set; }
} 