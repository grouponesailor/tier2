using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tier2Management.API.Models;

public class FileModel : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Path { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Extension { get; set; } = string.Empty;
    
    public long Size { get; set; }
    
    [MaxLength(32)]
    public string? MD5Hash { get; set; }
    
    [MaxLength(64)]
    public string? SHA256Hash { get; set; }
    
    [MaxLength(200)]
    public string? MimeType { get; set; }
    
    public int FolderId { get; set; }
    
    [ForeignKey(nameof(FolderId))]
    public virtual FolderModel Folder { get; set; } = null!;
    
    public int? CurrentVersionId { get; set; }
    
    [ForeignKey(nameof(CurrentVersionId))]
    public virtual FileVersion? CurrentVersion { get; set; }
    
    public SecurityClassification Classification { get; set; } = SecurityClassification.Internal;
    
    public bool IsLocked { get; set; } = false;
    
    public int? LockId { get; set; }
    
    [ForeignKey(nameof(LockId))]
    public virtual FileLock? Lock { get; set; }
    
    // Navigation properties
    public virtual ICollection<FileVersion> Versions { get; set; } = new List<FileVersion>();
    public virtual ICollection<FilePermission> Permissions { get; set; } = new List<FilePermission>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}

public enum SecurityClassification
{
    Public = 1,
    Internal = 2,
    Confidential = 3,
    Restricted = 4,
    TopSecret = 5
} 