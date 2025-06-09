using System.Text.Json.Serialization;

namespace MockCollaborationServer.Models;

public class MockFile
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public bool IsLocked { get; set; } = false;
    public string? LockedBy { get; set; }
    public DateTime? LockTimestamp { get; set; }
    public List<MockFileVersion> Versions { get; set; } = new();
    public List<MockPermission> Permissions { get; set; } = new();
    public MockClassification Classification { get; set; } = new();
    public bool IsDeleted { get; set; } = false;
    public string? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public long Size { get; set; }
    public string? Extension { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
}

public class MockFileVersion
{
    public Guid VersionId { get; set; } = Guid.NewGuid();
    public int VersionNumber { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long Size { get; set; }
    public string? Comments { get; set; }
    public bool IsCurrent { get; set; } = false;
}

public class MockPermission
{
    public string PrincipalId { get; set; } = string.Empty;
    public string PrincipalType { get; set; } = string.Empty; // User or Group
    public string AccessLevel { get; set; } = string.Empty; // Read, Write, Modify, Delete
    public bool IsInherited { get; set; } = false;
    public string? InheritedFrom { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class MockClassification
{
    public string Level { get; set; } = "Internal"; // Public, Internal, Confidential, Secret
    public string? ChangedBy { get; set; }
    public DateTime? ChangedAt { get; set; }
    public string? Justification { get; set; }
    public List<MockClassificationHistory> History { get; set; } = new();
}

public class MockClassificationHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PreviousLevel { get; set; } = string.Empty;
    public string NewLevel { get; set; } = string.Empty;
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? Justification { get; set; }
}

public class MockUser
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> AdGroups { get; set; } = new();
    public string? Department { get; set; }
    public string? Title { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime? LastLogon { get; set; }
}

public class MockAdGroup
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Members { get; set; } = new();
    public string GroupType { get; set; } = "Security"; // Security, Distribution
    public bool IsEnabled { get; set; } = true;
}

public class MockFolder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public Guid? ParentFolderId { get; set; }
    public List<MockPermission> Permissions { get; set; } = new();
    public MockClassification Classification { get; set; } = new();
    public bool IsDeleted { get; set; } = false;
    public string? DeletedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
}

public class MockQueueMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string QueueName { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public string MessageBody { get; set; } = string.Empty;
    public Dictionary<string, object> Headers { get; set; } = new();
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Failed, DeadLetter
    public int RetryCount { get; set; } = 0;
    public DateTime? LastRetryAt { get; set; }
    public string? ErrorMessage { get; set; }
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical
    public string? CorrelationId { get; set; }
    public Guid? ItemId { get; set; } // For file/folder operations
}

public class MockQueue
{
    public string Name { get; set; } = string.Empty;
    public List<MockQueueMessage> Messages { get; set; } = new();
    public int ConsumerCount { get; set; } = 0;
    public bool IsHealthy { get; set; } = true;
    public DateTime LastChecked { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Search response models for files search endpoint
/// </summary>
public class FileSearchResponse
{
    public PagingInfo Paging { get; set; } = new();
    public List<FileSearchHit> Hits { get; set; } = new();
}

public class PagingInfo
{
    [JsonPropertyName("pit_id")]
    public string PitId { get; set; } = string.Empty;
    public List<long> Sort { get; set; } = new();
}

public class FileSearchHit
{
    public FileMetadata Metadata { get; set; } = new();
}

public class FileMetadata
{
    public string AuthorizationLevel { get; set; } = "FULLY_AUTHORIZED";
    public string Extension { get; set; } = string.Empty;
    public DateTime UpdateDate { get; set; }
    public string UpdateId { get; set; } = string.Empty;
    public string FullNamePath { get; set; } = string.Empty;
    public string AcExternalId { get; set; } = string.Empty;
    public int AcInheriteType { get; set; } = 0;
    public string OwnerId { get; set; } = string.Empty;
    public int Type { get; set; } = 1;
    public string FullPath { get; set; } = string.Empty;
    public DateTime LastOperationDate { get; set; }
    public string ParentId { get; set; } = string.Empty;
    public string LastOperationByUser { get; set; } = string.Empty;
    public string ParentName { get; set; } = string.Empty;
    public long Size { get; set; }
    public string LastOperation { get; set; } = "file_saved";
    public string Name { get; set; } = string.Empty;
    public List<object> Attributes { get; set; } = new();
    public string Id { get; set; } = string.Empty;
    public int Status { get; set; } = 1;
    public DateTime CreateDate { get; set; }
}

/// <summary>
/// Request models for files search POST endpoint
/// </summary>
public class FileSearchRequest
{
    public string? Q { get; set; }
    public object? Filters { get; set; }
    public List<SortField>? Sort { get; set; }
    public string System { get; set; } = string.Empty;
    public string Uuid { get; set; } = string.Empty;
}

public class SortField
{
    public string Field { get; set; } = string.Empty;
    public string Order { get; set; } = "desc";
}

/// <summary>
/// File views models for MockServer
/// </summary>
public class FileViewsResponse
{
    public string ItemId { get; set; } = string.Empty;
    public List<FileViewInfo> Views { get; set; } = new();
    public ResponseHeader ResponseHeader { get; set; } = new();
    public string? Ex { get; set; }
}

public class FileViewInfo
{
    public string UserId { get; set; } = string.Empty;
    public int ViewCounter { get; set; }
    public DateTime FirstViewDate { get; set; }
    public DateTime LastViewDate { get; set; }
}

 