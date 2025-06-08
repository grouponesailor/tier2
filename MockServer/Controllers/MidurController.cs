using Microsoft.AspNetCore.Mvc;
using MockCollaborationServer.Models;
using MockCollaborationServer.Services;

namespace MockCollaborationServer.Controllers;

[ApiController]
[Route("api/midur")]
public class MidurController : ControllerBase
{
    private readonly MockDataService _dataService;
    private readonly ILogger<MidurController> _logger;

    public MidurController(MockDataService dataService, ILogger<MidurController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    /// <summary>
    /// Get item permissions
    /// </summary>
    [HttpGet("getItemPermissions/{itemId}")]
    public ActionResult<ItemPermissionsResponse> GetItemPermissions(
        int itemId, 
        [FromQuery] string reqId = "", 
        [FromQuery] int callingSystemId = 0)
    {
        try
        {
            _logger.LogInformation("Mock get item permissions request received for item ID: {ItemId}, Request ID: {ReqId}, Calling System: {CallingSystemId}", 
                itemId, reqId, callingSystemId);

            // Return mock data as per the example
            var response = new ItemPermissionsResponse
            {
                ResponseBody = new ItemPermissionsResponseBody
                {
                    ItemId = itemId,
                    DirectAdItems = new List<AdItem>
                    {
                        new AdItem
                        {
                            AdId = "088888",
                            AdName = "Shimon",
                            AccessMethods = new List<int> { 1, 2, 3 }
                        }
                    },
                    IsInherite = true,
                    InheriteAdItems = new List<AdItem>
                    {
                        new AdItem
                        {
                            AdId = "0777777",
                            AdName = "Ted",
                            AccessMethods = new List<int> { 1, 2, 3, 8 }
                        }
                    }
                },
                ResponseHeader = new Models.ResponseHeader { ReqId = reqId },
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
                ResponseHeader = new Models.ResponseHeader { ReqId = reqId },
                Ex = ex.Message
            });
        }
    }
} 