using System.ComponentModel.DataAnnotations;

namespace Tier2Management.API.Models;

public class UserSession : BaseEntity
{
    public int UserId { get; set; }
    
    public virtual ADUser User { get; set; } = null!;
    
    [Required]
    [MaxLength(255)]
    public string SessionId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string UserName { get; set; } = string.Empty;
    
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    
    public DateTime? EndTime { get; set; }
    
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    
    [MaxLength(45)]
    public string? IpAddress { get; set; }
    
    [MaxLength(1000)]
    public string? UserAgent { get; set; }
    
    [MaxLength(500)]
    public string? MachineName { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public SessionType SessionType { get; set; } = SessionType.Web;
    
    // Navigation properties
    public virtual ICollection<FileLock> FileLocks { get; set; } = new List<FileLock>();
}

public enum SessionType
{
    Web = 1,
    Desktop = 2,
    Mobile = 3,
    API = 4
} 