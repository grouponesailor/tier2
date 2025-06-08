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
    /// Unlock a specific file
    /// </summary>
    [HttpPost("{id}/unlock")]
    public ActionResult<object> UnlockFile(Guid id, [FromBody] UnlockFileRequest request)
    {
        if (string.IsNullOrEmpty(request.AdminUser) || string.IsNullOrEmpty(request.Justification))
            return BadRequest(new { message = "AdminUser and Justification are required" });

        var success = _dataService.UnlockFile(id, request.AdminUser, request.Justification);
        
        if (!success)
            return BadRequest(new { message = "File not found or not locked" });

        return Ok(new
        {
            success = true,
            message = "File unlocked successfully",
            fileId = id,
            unlockedBy = request.AdminUser,
            timestamp = DateTime.UtcNow
        });
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
}

public class UnlockFileRequest
{
    public string AdminUser { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
}

public class RestoreVersionRequest
{
    public int VersionNumber { get; set; }
    public string AdminUser { get; set; } = string.Empty;
    public string? Comments { get; set; }
} 