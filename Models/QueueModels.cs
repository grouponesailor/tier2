using System.ComponentModel.DataAnnotations;

namespace Tier2Management.API.Models;

public class QueueOperation : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string QueueName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Operation { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string PerformedBy { get; set; } = string.Empty;
    
    public DateTime OperationTime { get; set; } = DateTime.UtcNow;
    
    public QueueOperationType OperationType { get; set; }
    
    public int? MessageCount { get; set; }
    
    [MaxLength(2000)]
    public string? Parameters { get; set; }
    
    public OperationStatus Status { get; set; }
    
    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }
    
    [MaxLength(2000)]
    public string? Justification { get; set; }
    
    public bool RequiresApproval { get; set; } = false;
    
    public DateTime? ApprovedAt { get; set; }
    
    [MaxLength(255)]
    public string? ApprovedBy { get; set; }
    
    [MaxLength(2000)]
    public string? ApprovalComments { get; set; }
}

public class QueueMessage : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string QueueName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string MessageId { get; set; } = string.Empty;
    
    public string MessageBody { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Headers { get; set; }
    
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    
    public MessageStatus Status { get; set; } = MessageStatus.Pending;
    
    public int RetryCount { get; set; } = 0;
    
    public DateTime? LastRetryAt { get; set; }
    
    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }
    
    [MaxLength(2000)]
    public string? ProcessingNotes { get; set; }
    
    public MessagePriority Priority { get; set; } = MessagePriority.Normal;
    
    [MaxLength(255)]
    public string? CorrelationId { get; set; }
    
    [MaxLength(255)]
    public string? ReplyTo { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
}

public class QueueHealth : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string QueueName { get; set; } = string.Empty;
    
    public int MessageCount { get; set; }
    
    public int ConsumerCount { get; set; }
    
    public int DeadLetterCount { get; set; }
    
    public int ErrorCount { get; set; }
    
    public double MessagesPerSecond { get; set; }
    
    public double AverageProcessingTime { get; set; }
    
    public QueueStatus Status { get; set; } = QueueStatus.Healthy;
    
    public DateTime LastChecked { get; set; } = DateTime.UtcNow;
    
    [MaxLength(2000)]
    public string? HealthNotes { get; set; }
    
    public bool IsAlertActive { get; set; } = false;
    
    [MaxLength(2000)]
    public string? AlertMessage { get; set; }
}

public enum QueueOperationType
{
    View = 1,
    Count = 2,
    Preview = 3,
    Transfer = 4,
    Purge = 5,
    Replay = 6,
    Create = 7,
    Delete = 8,
    Pause = 9,
    Resume = 10
}

public enum OperationStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    RequiresApproval = 6
}

public enum MessageStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    DeadLetter = 5,
    Retry = 6
}

public enum MessagePriority
{
    Low = 1,
    Normal = 2,
    High = 3,
    Critical = 4
}

public enum QueueStatus
{
    Healthy = 1,
    Warning = 2,
    Critical = 3,
    Down = 4,
    Maintenance = 5
} 