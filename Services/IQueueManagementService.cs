using Tier2Management.API.DTOs;

namespace Tier2Management.API.Services;

public interface IQueueManagementService
{
    Task<int> GetErrorQueueCountAsync();
    Task<QueueInfoDto?> FindItemInQueueAsync(string queueName, Guid itemId);
    Task<MessagePreviewDto> GetQueueMessagesPreviewAsync(string queueName, int count = 10);
    Task<bool> TransferMessagesAsync(TransferMessagesDto request);
    Task<List<QueueInfoDto>> GetAllQueuesAsync();
    Task<bool> PurgeQueueAsync(PurgeQueueDto request);
} 