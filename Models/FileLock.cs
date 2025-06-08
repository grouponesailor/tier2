using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tier2Management.API.Models;

public class FileLock : BaseEntity
{
    public int FileId { get; set; }
    
    [ForeignKey(nameof(FileId))]
    public virtual FileModel File { get; set; } = null!;
    
    public int UserSessionId { get; set; }
    
    [ForeignKey(nameof(UserSessionId))]
    public virtual UserSession UserSession { get; set; } = null!;
    
    [Required]
    [MaxLength(255)]
    public string LockedBy { get; set; } = string.Empty;
    
    public DateTime LockedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiresAt { get; set; }
    
    public LockType LockType { get; set; } = LockType.Exclusive;
    
    [MaxLength(1000)]
    public string? Reason { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(500)]
    public string? MachineName { get; set; }
    
    [MaxLength(45)]
    public string? IpAddress { get; set; }
}

public enum LockType
{
    Shared = 1,
    Exclusive = 2,
    ReadOnly = 3
} 