using Microsoft.AspNetCore.Mvc;
using MockCollaborationServer.Services;

namespace MockCollaborationServer.Controllers;

[ApiController]
[Route("api/deleted-items")]
public class DeletedItemsController : ControllerBase
{
    private readonly MockDataService _dataService;
    private readonly ILogger<DeletedItemsController> _logger;

    public DeletedItemsController(MockDataService dataService, ILogger<DeletedItemsController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    /// <summary>
    /// Get all deleted items (files and folders)
    /// </summary>
    [HttpGet]
    public ActionResult<object> GetDeletedItems()
    {
        var deletedFiles = _dataService.GetDeletedFiles();
        var deletedFolders = _dataService.GetDeletedFolders();

        var items = deletedFiles.Select(f => new
        {
            id = f.Id,
            name = f.Name,
            path = f.Path,
            type = "File",
            size = (long?)f.Size,
            deletedBy = f.DeletedBy,
            deletedAt = f.DeletedAt,
            canRestore = true
        }).Concat(deletedFolders.Select(f => new
        {
            id = f.Id,
            name = f.Name,
            path = f.Path,
            type = "Folder",
            size = (long?)null,
            deletedBy = f.DeletedBy,
            deletedAt = f.DeletedAt,
            canRestore = true
        })).OrderByDescending(i => i.deletedAt);

        return Ok(new
        {
            count = items.Count(),
            items = items
        });
    }

    /// <summary>
    /// Restore a deleted item
    /// </summary>
    [HttpPost("{id}/restore")]
    public ActionResult<object> RestoreDeletedItem(Guid id, [FromBody] RestoreItemRequest request)
    {
        if (string.IsNullOrEmpty(request.AdminUser))
            return BadRequest(new { message = "AdminUser is required" });

        // Try to restore as file first
        var fileSuccess = _dataService.RestoreDeletedFile(id, request.AdminUser);
        if (fileSuccess)
        {
            return Ok(new
            {
                success = true,
                message = "File restored successfully",
                itemId = id,
                itemType = "File",
                restoredBy = request.AdminUser,
                timestamp = DateTime.UtcNow
            });
        }

        // Try to restore as folder
        var folderSuccess = _dataService.RestoreDeletedFolder(id, request.AdminUser);
        if (folderSuccess)
        {
            return Ok(new
            {
                success = true,
                message = "Folder restored successfully",
                itemId = id,
                itemType = "Folder",
                restoredBy = request.AdminUser,
                timestamp = DateTime.UtcNow
            });
        }

        return NotFound(new { message = "Deleted item not found" });
    }
}

public class RestoreItemRequest
{
    public string AdminUser { get; set; } = string.Empty;
    public string? Justification { get; set; }
} 