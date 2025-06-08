using System.Text;
using System.Text.Json;
using Tier2Management.API.DTOs;

namespace Tier2Management.API.Services;

public class QueueManagementService : IQueueManagementService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<QueueManagementService> _logger;
    private readonly string _mockServerBaseUrl;

    public QueueManagementService(HttpClient httpClient, IConfiguration configuration, ILogger<QueueManagementService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _mockServerBaseUrl = configuration["MockCollaborationServer:BaseUrl"] ?? "https://localhost:7001";
    }

    public async Task<int> GetErrorQueueCountAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/queues/error/count");
            if (!response.IsSuccessStatusCode) return 0;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            return data.GetProperty("messageCount").GetInt32();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving error queue count");
            return 0;
        }
    }

    public async Task<QueueInfoDto?> FindItemInQueueAsync(string queueName, Guid itemId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/queues/{queueName}/search/{itemId}");
            if (!response.IsSuccessStatusCode) return null;

            return new QueueInfoDto
            {
                Name = queueName,
                MessageCount = 1,
                Status = Models.QueueStatus.Healthy,
                LastChecked = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for item in queue");
            return null;
        }
    }

    public async Task<MessagePreviewDto> GetQueueMessagesPreviewAsync(string queueName, int count = 10)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/queues/{queueName}/messages/preview?count={count}");
            
            return new MessagePreviewDto
            {
                QueueName = queueName,
                TotalMessages = response.IsSuccessStatusCode ? count : 0,
                RetrievedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving queue messages preview");
            return new MessagePreviewDto { QueueName = queueName, TotalMessages = 0, RetrievedAt = DateTime.UtcNow };
        }
    }

    public async Task<bool> TransferMessagesAsync(TransferMessagesDto request)
    {
        try
        {
            var transferRequest = new
            {
                SourceQueue = request.SourceQueueName,
                TargetQueue = request.DestinationQueueName,
                MessageIds = new List<Guid>(),
                AdminUser = "admin",
                Justification = request.Justification
            };

            var json = JsonSerializer.Serialize(transferRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_mockServerBaseUrl}/api/queues/transfer", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error transferring messages");
            return false;
        }
    }

    public async Task<List<QueueInfoDto>> GetAllQueuesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_mockServerBaseUrl}/api/queues");
            return response.IsSuccessStatusCode ? new List<QueueInfoDto>() : new List<QueueInfoDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all queues");
            return new List<QueueInfoDto>();
        }
    }

    public async Task<bool> PurgeQueueAsync(PurgeQueueDto request)
    {
        try
        {
            _logger.LogWarning("Queue purge requested for {QueueName}", request.QueueName);
            return false; // For safety in mock
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error purging queue");
            return false;
        }
    }
} 