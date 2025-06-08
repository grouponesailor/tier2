using Tier2Management.API.Models;

namespace Tier2Management.API.DTOs;

// Queue DTOs
public class QueueInfoDto
{
    public string Name { get; set; } = string.Empty;
    public int MessageCount { get; set; }
    public int ConsumerCount { get; set; }
    public int DeadLetterCount { get; set; }
    public double MessagesPerSecond { get; set; }
    public QueueStatus Status { get; set; }
    public bool IsAlertActive { get; set; }
    public string? AlertMessage { get; set; }
    public DateTime LastChecked { get; set; }
    public QueueConfigDto? Configuration { get; set; }
}

public class QueueConfigDto
{
    public bool Durable { get; set; }
    public bool AutoDelete { get; set; }
    public bool Exclusive { get; set; }
    public Dictionary<string, object> Arguments { get; set; } = new();
    public string? DeadLetterExchange { get; set; }
    public string? DeadLetterRoutingKey { get; set; }
    public int? MessageTTL { get; set; }
    public int? MaxLength { get; set; }
}

public class QueueHealthDto
{
    public int Id { get; set; }
    public string QueueName { get; set; } = string.Empty;
    public int MessageCount { get; set; }
    public int ConsumerCount { get; set; }
    public int DeadLetterCount { get; set; }
    public int ErrorCount { get; set; }
    public double MessagesPerSecond { get; set; }
    public double AverageProcessingTime { get; set; }
    public QueueStatus Status { get; set; }
    public DateTime LastChecked { get; set; }
    public string? HealthNotes { get; set; }
    public bool IsAlertActive { get; set; }
    public string? AlertMessage { get; set; }
}

// Message DTOs
public class QueueMessageDto
{
    public int Id { get; set; }
    public string QueueName { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public string MessageBody { get; set; } = string.Empty;
    public string? Headers { get; set; }
    public DateTime ReceivedAt { get; set; }
    public MessageStatus Status { get; set; }
    public int RetryCount { get; set; }
    public DateTime? LastRetryAt { get; set; }
    public string? ErrorMessage { get; set; }
    public MessagePriority Priority { get; set; }
    public string? CorrelationId { get; set; }
}

public class MessagePreviewDto
{
    public string QueueName { get; set; } = string.Empty;
    public int TotalMessages { get; set; }
    public List<QueueMessageDto> Messages { get; set; } = new();
    public DateTime RetrievedAt { get; set; }
}

// Queue Operation DTOs
public class QueueOperationDto
{
    public int Id { get; set; }
    public string QueueName { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime OperationTime { get; set; }
    public QueueOperationType OperationType { get; set; }
    public int? MessageCount { get; set; }
    public OperationStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Justification { get; set; }
}

public class CreateQueueOperationDto
{
    public string QueueName { get; set; } = string.Empty;
    public QueueOperationType OperationType { get; set; }
    public string Justification { get; set; } = string.Empty;
    public bool RequiresApproval { get; set; } = false;
}

// Queue Management Operations
public class PurgeQueueDto
{
    public string QueueName { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
    public bool ConfirmPurge { get; set; } = false;
}

public class TransferMessagesDto
{
    public string SourceQueueName { get; set; } = string.Empty;
    public string DestinationQueueName { get; set; } = string.Empty;
    public int? MessageCount { get; set; }
    public string Justification { get; set; } = string.Empty;
}

// Queue Search and Analysis
public class QueueSearchDto
{
    public string? QueueNamePattern { get; set; }
    public string? MessagePattern { get; set; }
    public MessageStatus? Status { get; set; }
    public MessagePriority? Priority { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? CorrelationId { get; set; }
    public int? MaxResults { get; set; } = 100;
    public int? Skip { get; set; } = 0;
}

public class QueueSearchResultDto
{
    public int TotalMatches { get; set; }
    public List<QueueMessageDto> Messages { get; set; } = new();
    public Dictionary<string, int> QueueDistribution { get; set; } = new();
    public Dictionary<MessageStatus, int> StatusDistribution { get; set; } = new();
    public TimeSpan SearchDuration { get; set; }
}

public class MessageLocationDto
{
    public string MessageId { get; set; } = string.Empty;
    public string? QueueName { get; set; }
    public bool Found { get; set; }
    public MessageStatus? Status { get; set; }
    public DateTime? LastSeen { get; set; }
    public string? CurrentLocation { get; set; }
    public List<string> LocationHistory { get; set; } = new();
}

// Queue Analytics and Monitoring
public class QueueAnalyticsDto
{
    public string QueueName { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalMessages { get; set; }
    public int ProcessedMessages { get; set; }
    public int FailedMessages { get; set; }
    public int DeadLetterMessages { get; set; }
    public double AverageProcessingTime { get; set; }
    public double ThroughputPerHour { get; set; }
    public double ErrorRate { get; set; }
    public List<QueueMetricDataPoint> Metrics { get; set; } = new();
    public Dictionary<string, int> ErrorTypes { get; set; } = new();
}

public class QueueMetricDataPoint
{
    public DateTime Timestamp { get; set; }
    public int MessageCount { get; set; }
    public double ProcessingTime { get; set; }
    public int ErrorCount { get; set; }
    public double ThroughputPerMinute { get; set; }
}

public class QueueDashboardDto
{
    public int TotalQueues { get; set; }
    public int HealthyQueues { get; set; }
    public int WarningQueues { get; set; }
    public int CriticalQueues { get; set; }
    public int TotalMessages { get; set; }
    public int DeadLetterMessages { get; set; }
    public int TotalConsumers { get; set; }
    public double OverallThroughput { get; set; }
    public List<QueueInfoDto> TopQueues { get; set; } = new();
    public List<QueueHealthDto> AlertQueues { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

// Queue Alert DTOs
public class QueueAlertDto
{
    public int Id { get; set; }
    public string QueueName { get; set; } = string.Empty;
    public QueueAlertType AlertType { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public string? AcknowledgedBy { get; set; }
    public RiskLevel Severity { get; set; }
}

public class CreateQueueAlertDto
{
    public string QueueName { get; set; } = string.Empty;
    public QueueAlertType AlertType { get; set; }
    public int Threshold { get; set; }
    public bool IsEnabled { get; set; } = true;
    public List<string> NotificationEmails { get; set; } = new();
}

public enum QueueAlertType
{
    HighMessageCount = 1,
    NoConsumers = 2,
    HighErrorRate = 3,
    DeadLetterThreshold = 4,
    LowThroughput = 5,
    QueueDown = 6
} 