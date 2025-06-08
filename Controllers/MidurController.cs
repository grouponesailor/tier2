using Microsoft.AspNetCore.Mvc;
using Tier2Management.API.DTOs;
using Tier2Management.API.Services;

namespace Tier2Management.API.Controllers;

[ApiController]
[Route("api/midur")]
public class MidurController : ControllerBase
{
    private readonly IFileManagementService _fileManagementService;
    private readonly ILogger<MidurController> _logger;

    public MidurController(IFileManagementService fileManagementService, ILogger<MidurController> logger)
    {
        _fileManagementService = fileManagementService;
        _logger = logger;
    }

    /// <summary>
    /// שליפת הרשאות פריט (Get item permissions)
    /// </summary>
    [HttpGet("getItemPermissions/{itemId}")]
    public async Task<ActionResult<ItemPermissionsResponse>> GetItemPermissions(
        int itemId, 
        [FromQuery] string reqId = "", 
        [FromQuery] int callingSystemId = 0)
    {
        try
        {
            _logger.LogInformation("Get item permissions request received for item ID: {ItemId}, Request ID: {ReqId}, Calling System: {CallingSystemId}", 
                itemId, reqId, callingSystemId);

            // Call service to get permissions (no validation as requested)
            var permissions = await _fileManagementService.GetItemPermissionsByIdAsync(itemId);
            
            var response = new ItemPermissionsResponse
            {
                ResponseBody = new ItemPermissionsResponseBody
                {
                    ItemId = itemId,
                    DirectAdItems = new List<AdItemDto>
                    {
                        new AdItemDto
                        {
                            AdId = "088888",
                            AdName = "Shimon",
                            AccessMethods = new List<int> { 1, 2, 3 }
                        }
                    },
                    IsInherite = true,
                    InheriteAdItems = new List<AdItemDto>
                    {
                        new AdItemDto
                        {
                            AdId = "0777777",
                            AdName = "Ted",
                            AccessMethods = new List<int> { 1, 2, 3, 8 }
                        }
                    }
                },
                ResponseHeader = new ResponseHeader { ReqId = reqId },
                Ex = null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting item permissions for item ID: {ItemId}", itemId);
            return Ok(new ItemPermissionsResponse
            {
                ResponseBody = new ItemPermissionsResponseBody { ItemId = itemId },
                ResponseHeader = new ResponseHeader { ReqId = reqId },
                Ex = ex.Message
            });
        }
    }
} 