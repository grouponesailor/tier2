using Microsoft.AspNetCore.Mvc;
using MockCollaborationServer.Services;

namespace MockCollaborationServer.Controllers;

[ApiController]
[Route("api/directory")]
public class DirectoryController : ControllerBase
{
    private readonly MockDataService _dataService;
    private readonly ILogger<DirectoryController> _logger;

    public DirectoryController(MockDataService dataService, ILogger<DirectoryController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    /// <summary>
    /// Restore a directory
    /// </summary>
    [HttpPut("restore")]
    public ActionResult<DirectoryRestoreResponse> RestoreDirectory([FromBody] DirectoryRestoreRequest request)
    {
        try
        {
            _logger.LogInformation("Mock directory restore request received for ID: {Id}, Request ID: {ReqId}", 
                request.RequestBody.Id, request.RequestHeader.ReqId);

            // Mock directory restore operation
            var response = new DirectoryRestoreResponse
            {
                Id = request.RequestBody.Id,
                ResponseHeader = new DirectoryResponseHeader { ReqId = request.RequestHeader.ReqId },
                Ex = null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring directory in mock server");
            return Ok(new DirectoryRestoreResponse
            {
                Id = request.RequestBody?.Id ?? string.Empty,
                ResponseHeader = new DirectoryResponseHeader { ReqId = request.RequestHeader?.ReqId ?? string.Empty },
                Ex = ex.Message
            });
        }
    }
}

// Directory restore DTOs
public class DirectoryRequestHeader
{
    public string ReqId { get; set; } = string.Empty;
    public string CallingSystemId { get; set; } = string.Empty;
}

public class DirectoryResponseHeader
{
    public string ReqId { get; set; } = string.Empty;
}

public class DirectoryRestoreRequestBody
{
    public string Id { get; set; } = string.Empty;
    public bool Override { get; set; }
    public string? NewParentId { get; set; }
}

public class DirectoryRestoreRequest
{
    public DirectoryRequestHeader RequestHeader { get; set; } = new();
    public DirectoryRestoreRequestBody RequestBody { get; set; } = new();
}

public class DirectoryRestoreResponse
{
    public string Id { get; set; } = string.Empty;
    public DirectoryResponseHeader ResponseHeader { get; set; } = new();
    public string? Ex { get; set; }
} 