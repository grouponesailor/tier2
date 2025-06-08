using Microsoft.AspNetCore.Mvc;
using MockCollaborationServer.Services;

namespace MockCollaborationServer.Controllers;

[ApiController]
[Route("api/queues")]
public class QueuesController : ControllerBase
{
    private readonly MockDataService _dataService;
    private readonly ILogger<QueuesController> _logger;

    public QueuesController(MockDataService dataService, ILogger<QueuesController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    /// <summary>
    /// Get error queue message count
    /// </summary>
    [HttpGet("error/count")]
    public ActionResult<object> GetErrorQueueCount()
    {
        var count = _dataService.GetErrorQueueCount();

        return Ok(new
        {
            queueName = "error",
            messageCount = count,
            timestamp = DateTime.UtcNow,
            status = count > 100 ? "Critical" : count > 50 ? "Warning" : "Healthy"
        });
    }

    /// <summary>
    /// Search for an item in a specific queue
    /// </summary>
    [HttpGet("{queueName}/search/{itemId}")]
    public ActionResult<object> SearchItemInQueue(string queueName, Guid itemId)
    {
        var message = _dataService.FindItemInQueue(queueName, itemId);

        if (message == null)
        {
            return Ok(new
            {
                found = false,
                queueName = queueName,
                itemId = itemId,
                message = "Item not found in queue"
            });
        }

        return Ok(new
        {
            found = true,
            queueName = queueName,
            itemId = itemId,
            message = new
            {
                id = message.Id,
                messageId = message.MessageId,
                messageBody = message.MessageBody,
                status = message.Status,
                retryCount = message.RetryCount,
                receivedAt = message.ReceivedAt,
                lastRetryAt = message.LastRetryAt,
                errorMessage = message.ErrorMessage,
                priority = message.Priority,
                correlationId = message.CorrelationId,
                headers = message.Headers
            }
        });
    }

    /// <summary>
    /// Get message preview for a queue (first 10 messages)
    /// </summary>
    [HttpGet("{queueName}/messages/preview")]
    public ActionResult<object> GetQueueMessagesPreview(string queueName, [FromQuery] int count = 10)
    {
        var messages = _dataService.GetQueueMessagesPreview(queueName, count);

        return Ok(new
        {
            queueName = queueName,
            totalMessages = messages.Count(),
            requestedCount = count,
            timestamp = DateTime.UtcNow,
            messages = messages.Select(m => new
            {
                id = m.Id,
                messageId = m.MessageId,
                messageBody = m.MessageBody.Length > 200 ? m.MessageBody.Substring(0, 200) + "..." : m.MessageBody,
                status = m.Status,
                retryCount = m.RetryCount,
                receivedAt = m.ReceivedAt,
                priority = m.Priority,
                correlationId = m.CorrelationId,
                itemId = m.ItemId,
                hasError = !string.IsNullOrEmpty(m.ErrorMessage)
            })
        });
    }

    /// <summary>
    /// Transfer messages between queues
    /// </summary>
    [HttpPost("transfer")]
    public ActionResult<object> TransferMessages([FromBody] TransferMessagesRequest request)
    {
        if (string.IsNullOrEmpty(request.SourceQueue) || string.IsNullOrEmpty(request.TargetQueue))
            return BadRequest(new { message = "SourceQueue and TargetQueue are required" });

        if (string.IsNullOrEmpty(request.AdminUser))
            return BadRequest(new { message = "AdminUser is required" });

        var success = _dataService.TransferMessages(request.SourceQueue, request.TargetQueue, request.MessageIds, request.AdminUser);

        if (!success)
            return BadRequest(new { message = "Transfer failed. Check queue names and message IDs." });

        return Ok(new
        {
            success = true,
            message = "Messages transferred successfully",
            sourceQueue = request.SourceQueue,
            targetQueue = request.TargetQueue,
            messageCount = request.MessageIds.Count,
            transferredBy = request.AdminUser,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Get all queues status
    /// </summary>
    [HttpGet]
    public ActionResult<object> GetAllQueues()
    {
        var queues = _dataService.GetAllQueues();

        return Ok(new
        {
            count = queues.Count(),
            timestamp = DateTime.UtcNow,
            queues = queues.Select(q => new
            {
                name = q.Name,
                messageCount = q.Messages.Count,
                consumerCount = q.ConsumerCount,
                isHealthy = q.IsHealthy,
                lastChecked = q.LastChecked,
                status = q.Messages.Count > 100 ? "Critical" : q.Messages.Count > 50 ? "Warning" : "Healthy"
            })
        });
    }
}

public class TransferMessagesRequest
{
    public string SourceQueue { get; set; } = string.Empty;
    public string TargetQueue { get; set; } = string.Empty;
    public List<Guid> MessageIds { get; set; } = new();
    public string AdminUser { get; set; } = string.Empty;
    public string? Justification { get; set; }
} 