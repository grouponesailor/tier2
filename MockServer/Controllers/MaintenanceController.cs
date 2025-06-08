using Microsoft.AspNetCore.Mvc;
using MockCollaborationServer.Services;

namespace MockCollaborationServer.Controllers;

[ApiController]
[Route("api/maintenance")]
public class MaintenanceController : ControllerBase
{
    private readonly MockDataService _dataService;
    private readonly ILogger<MaintenanceController> _logger;

    public MaintenanceController(MockDataService dataService, ILogger<MaintenanceController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    /// <summary>
    /// Get storage ID for a specific item
    /// </summary>
    [HttpGet("getStorageId/id/{id}")]
    public ActionResult<GetStorageIdResponse> GetStorageId(string id)
    {
        try
        {
            _logger.LogInformation("Mock getStorageId request received for ID: {Id}", id);

            // Mock storage path generation based on ID
            var storagePath = $"\\\\server\\share\\{id}.txt";
            var storageType = 9; // File storage type

            var response = new GetStorageIdResponse
            {
                ResponseBody = new GetStorageIdResponseBody
                {
                    ItemId = id,
                    StorageId = storagePath,
                    StorageType = storageType
                },
                ResponseHeader = new Models.ResponseHeader { ReqId = id },
                Ex = null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting storage ID for item: {Id}", id);
            return Ok(new GetStorageIdResponse
            {
                ResponseBody = new GetStorageIdResponseBody { ItemId = id },
                ResponseHeader = new Models.ResponseHeader { ReqId = id },
                Ex = ex.Message
            });
        }
    }
}

/// <summary>
/// Response body for get storage ID
/// </summary>
public class GetStorageIdResponseBody
{
    public string ItemId { get; set; } = string.Empty;
    public string StorageId { get; set; } = string.Empty;
    public int StorageType { get; set; }
}

/// <summary>
/// Complete response for get storage ID
/// </summary>
public class GetStorageIdResponse
{
    public GetStorageIdResponseBody ResponseBody { get; set; } = new();
    public Models.ResponseHeader ResponseHeader { get; set; } = new();
    public string? Ex { get; set; }
} 