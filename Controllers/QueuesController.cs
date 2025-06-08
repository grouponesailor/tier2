using Microsoft.AspNetCore.Mvc;
using Tier2Management.API.DTOs;
using Tier2Management.API.Services;

namespace Tier2Management.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueuesController : ControllerBase
{
    private readonly IQueueManagementService _queueManagementService;
    private readonly ILogger<QueuesController> _logger;

    public QueuesController(IQueueManagementService queueManagementService, ILogger<QueuesController> logger)
    {
        _queueManagementService = queueManagementService;
        _logger = logger;
    }

    /// <summary>
    /// שליפת כל התורים (Get all queues)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<QueueInfoDto>>> GetAllQueues()
    {
        try
        {
            var queues = await _queueManagementService.GetAllQueuesAsync();
            return Ok(queues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all queues");
            return StatusCode(500, new { message = "שגיאה בשליפת התורים" });
        }
    }

    /// <summary>
    /// שליפת כמות הודעות בתור השגיאות (Get error queue count)
    /// </summary>
    [HttpGet("error/count")]
    public async Task<ActionResult<object>> GetErrorQueueCount()
    {
        try
        {
            var count = await _queueManagementService.GetErrorQueueCountAsync();
            return Ok(new
            {
                queueName = "error-queue",
                messageCount = count,
                timestamp = DateTime.UtcNow,
                hasMessages = count > 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving error queue count");
            return StatusCode(500, new { message = "שגיאה בשליפת כמות הודעות בתור השגיאות" });
        }
    }

    /// <summary>
    /// חיפוש פריט בתור (Find item in queue)
    /// </summary>
    [HttpGet("{queueName}/search/{itemId}")]
    public async Task<ActionResult<object>> FindItemInQueue(string queueName, Guid itemId)
    {
        try
        {
            var result = await _queueManagementService.FindItemInQueueAsync(queueName, itemId);
            
            if (result == null)
            {
                return Ok(new
                {
                    found = false,
                    message = "הפריט לא נמצא בתור",
                    queueName = queueName,
                    itemId = itemId,
                    timestamp = DateTime.UtcNow
                });
            }

            return Ok(new
            {
                found = true,
                queueInfo = result,
                message = "הפריט נמצא בתור",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for item in queue");
            return StatusCode(500, new { message = "שגיאה בחיפוש פריט בתור" });
        }
    }

    /// <summary>
    /// תצוגה מקדימה של הודעות בתור (Queue messages preview)
    /// </summary>
    [HttpGet("{queueName}/messages/preview")]
    public async Task<ActionResult<MessagePreviewDto>> GetQueueMessagesPreview(
        string queueName, 
        [FromQuery] int count = 10)
    {
        try
        {
            var preview = await _queueManagementService.GetQueueMessagesPreviewAsync(queueName, count);
            return Ok(preview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving queue messages preview");
            return StatusCode(500, new { message = "שגיאה בשליפת תצוגה מקדימה של הודעות התור" });
        }
    }

    /// <summary>
    /// העברת הודעות בין תורים (Transfer messages between queues)
    /// </summary>
    [HttpPost("transfer")]
    public async Task<ActionResult<object>> TransferMessages([FromBody] TransferMessagesDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.SourceQueueName) || string.IsNullOrEmpty(request.DestinationQueueName))
            {
                return BadRequest(new { message = "שם התור המקור והיעד חובה" });
            }

            var success = await _queueManagementService.TransferMessagesAsync(request);
            
            if (success)
            {
                _logger.LogInformation("Messages transferred from {SourceQueue} to {DestinationQueue}", 
                    request.SourceQueueName, request.DestinationQueueName);
                
                return Ok(new
                {
                    success = true,
                    message = "ההודעות הועברו בהצלחה",
                    sourceQueue = request.SourceQueueName,
                    destinationQueue = request.DestinationQueueName,
                    timestamp = DateTime.UtcNow
                });
            }

            return BadRequest(new { 
                success = false, 
                message = "כשל בהעברת ההודעות" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring messages");
            return StatusCode(500, new { message = "שגיאה בהעברת הודעות" });
        }
    }

    /// <summary>
    /// ניקוי תור (Purge queue)
    /// </summary>
    [HttpPost("{queueName}/purge")]
    public async Task<ActionResult<object>> PurgeQueue(string queueName, [FromBody] PurgeQueueDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Justification))
            {
                return BadRequest(new { message = "נימוק לניקוי התור חובה" });
            }

            // Update the request with the queue name from the route
            request.QueueName = queueName;

            var success = await _queueManagementService.PurgeQueueAsync(request);
            
            if (success)
            {
                return Ok(new
                {
                    success = true,
                    message = "התור נוקה בהצלחה",
                    queueName = queueName,
                    timestamp = DateTime.UtcNow
                });
            }

            return BadRequest(new { 
                success = false, 
                message = "ניקוי התור נדחה מסיבות אבטחה" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error purging queue");
            return StatusCode(500, new { message = "שגיאה בניקוי התור" });
        }
    }
} 