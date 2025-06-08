using Tier2Management.API.Models;

namespace Tier2Management.API.DTOs;

// File DTOs
public class FileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? MimeType { get; set; }
    public int FolderId { get; set; }
    public string FolderName { get; set; } = string.Empty;
    public SecurityClassification Classification { get; set; }
    public bool IsLocked { get; set; }
    public FileLockDto? Lock { get; set; }
    public FileVersionDto? CurrentVersion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

public class CreateFileDto
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long Size { get; set; }
    public string? MimeType { get; set; }
    public int FolderId { get; set; }
    public SecurityClassification Classification { get; set; } = SecurityClassification.Internal;
    public string? MD5Hash { get; set; }
    public string? SHA256Hash { get; set; }
}

public class UpdateFileDto
{
    public string? Name { get; set; }
    public SecurityClassification? Classification { get; set; }
    public string? ClassificationJustification { get; set; }
}

// Folder DTOs
public class FolderDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int? ParentFolderId { get; set; }
    public string? ParentFolderName { get; set; }
    public SecurityClassification Classification { get; set; }
    public bool IsRoot { get; set; }
    public int FileCount { get; set; }
    public int SubFolderCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

public class CreateFolderDto
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int? ParentFolderId { get; set; }
    public SecurityClassification Classification { get; set; } = SecurityClassification.Internal;
}

public class UpdateFolderDto
{
    public string? Name { get; set; }
    public SecurityClassification? Classification { get; set; }
    public string? ClassificationJustification { get; set; }
}

// File Version DTOs
public class FileVersionDto
{
    public int Id { get; set; }
    public int FileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int VersionNumber { get; set; }
    public string? Comments { get; set; }
    public long Size { get; set; }
    public string? MD5Hash { get; set; }
    public string? SHA256Hash { get; set; }
    public bool IsCurrent { get; set; }
    public string? VersionLabel { get; set; }
    public FileVersionType VersionType { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class CreateFileVersionDto
{
    public int FileId { get; set; }
    public string? Comments { get; set; }
    public long Size { get; set; }
    public string? MD5Hash { get; set; }
    public string? SHA256Hash { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string? VersionLabel { get; set; }
    public FileVersionType VersionType { get; set; } = FileVersionType.Minor;
}

public class RestoreFileVersionDto
{
    public int FileId { get; set; }
    public int VersionId { get; set; }
    public string Justification { get; set; } = string.Empty;
    public bool CreateBackup { get; set; } = true;
}

// File Lock DTOs
public class FileLockDto
{
    public int Id { get; set; }
    public int FileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string LockedBy { get; set; } = string.Empty;
    public DateTime LockedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public LockType LockType { get; set; }
    public string? Reason { get; set; }
    public bool IsActive { get; set; }
    public string? MachineName { get; set; }
    public string? IpAddress { get; set; }
    public UserSessionDto? UserSession { get; set; }
}

public class CreateFileLockDto
{
    public int FileId { get; set; }
    public LockType LockType { get; set; } = LockType.Exclusive;
    public string? Reason { get; set; }
    public int? ExpirationMinutes { get; set; }
}

public class ForceUnlockFileDto
{
    public int LockId { get; set; }
    public string Justification { get; set; } = string.Empty;
    public bool NotifyUser { get; set; } = true;
}

// New unlock request/response DTOs
public class RequestHeader
{
    public string ReqId { get; set; } = string.Empty;
    public int CallingSystemId { get; set; }
}

public class ResponseHeader
{
    public string ReqId { get; set; } = string.Empty;
}

public class UnlockRequestBody
{
    public string Id { get; set; } = string.Empty;
}

public class UnlockFileRequest
{
    public RequestHeader RequestHeader { get; set; } = new();
    public UnlockRequestBody RequestBody { get; set; } = new();
}

public class UnlockFileResponse
{
    public string Id { get; set; } = string.Empty;
    public bool Locked { get; set; }
    public string Signature { get; set; } = string.Empty;
    public string Etag { get; set; } = string.Empty;
    public ResponseHeader ResponseHeader { get; set; } = new();
    public string? Ex { get; set; }
}

// Recovery DTOs
public class DeletedItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty; // File or Folder
    public long? Size { get; set; }
    public DateTime DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? DeletionReason { get; set; }
    public bool CanRecover { get; set; }
    public DateTime? PermanentDeletionDate { get; set; }
}

public class RecoverItemDto
{
    public int ItemId { get; set; }
    public string ItemType { get; set; } = string.Empty; // File or Folder
    public int? NewParentFolderId { get; set; }
    public string? NewName { get; set; }
    public string Justification { get; set; } = string.Empty;
    public bool OverwriteExisting { get; set; } = false;
}

public class BulkRecoveryDto
{
    public List<RecoverItemDto> Items { get; set; } = new();
    public string Justification { get; set; } = string.Empty;
    public bool ContinueOnError { get; set; } = true;
} 