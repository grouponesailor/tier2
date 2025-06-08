using Microsoft.AspNetCore.Mvc;
using Tier2Management.API.DTOs;
using Tier2Management.API.Services;

namespace Tier2Management.API.Controllers;

[ApiController]
[Route("api/maintenance")]
public class MaintenanceController : ControllerBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly ILogger<MaintenanceController> _logger;

    public MaintenanceController(IFileManagementService fileManagementService, ILogger<MaintenanceController> logger)
    {
        _fileManagementService = fileManagementService;
        _logger = logger;
    }

    /// <summary>
    /// שליפת מזהה אחסון (Get storage ID)
    /// </summary>
    [HttpGet("getStorageId/id/{id}")]
    public async Task<ActionResult<GetStorageIdResponse>> GetStorageId(string id)
    {
        try
        {
            _logger.LogInformation("GetStorageId request received for ID: {Id}", id);

            // Mock implementation - in real scenario, this would query the storage system
            var response = new GetStorageIdResponse
            {
                ResponseBody = new GetStorageIdResponseBody
                {
                    ItemId = id,
                    StorageId = $"\\\\aa\\bb\\{id}.txt", // Mock storage path
                    StorageType = 9 // Mock storage type
                },
                ResponseHeader = new ResponseHeader { ReqId = id },
                Ex = null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving storage ID for {Id}", id);
            return Ok(new GetStorageIdResponse
            {
                ResponseBody = new GetStorageIdResponseBody
                {
                    ItemId = id,
                    StorageId = string.Empty,
                    StorageType = 0
                },
                ResponseHeader = new ResponseHeader { ReqId = id },
                Ex = ex.Message
            });
        }
    }
} 