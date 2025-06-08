using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tier2Management.API.Models;

public class AuditLog : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string UserName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string Action { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string EntityType { get; set; } = string.Empty;
    
    public int? EntityId { get; set; }
    
    [MaxLength(1000)]
    public string? EntityName { get; set; }
    
    public AuditLogType LogType { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string? OldValues { get; set; }
    
    [Column(TypeName = "nvarchar(max)")]
    public string? NewValues { get; set; }
    
    [MaxLength(45)]
    public string? IpAddress { get; set; }
    
    [MaxLength(1000)]
    public string? UserAgent { get; set; }
    
    [MaxLength(500)]
    public string? MachineName { get; set; }
    
    [MaxLength(2000)]
    public string? Comments { get; set; }
    
    public SecurityClassification? PreviousClassification { get; set; }
    
    public SecurityClassification? NewClassification { get; set; }
    
    [MaxLength(1000)]
    public string? ClassificationJustification { get; set; }
    
    public AuditCategory Category { get; set; }
    
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;
    
    public bool RequiresReview { get; set; } = false;
    
    public DateTime? ReviewedAt { get; set; }
    
    [MaxLength(255)]
    public string? ReviewedBy { get; set; }
    
    [MaxLength(2000)]
    public string? ReviewComments { get; set; }
}

public enum AuditLogType
{
    Create = 1,
    Update = 2,
    Delete = 3,
    Access = 4,
    Login = 5,
    Logout = 6,
    PermissionChange = 7,
    ClassificationChange = 8,
    Lock = 9,
    Unlock = 10,
    VersionRestore = 11,
    Recovery = 12,
    QueueOperation = 13
}

public enum AuditCategory
{
    FileOperation = 1,
    FolderOperation = 2,
    UserManagement = 3,
    PermissionManagement = 4,
    SystemOperation = 5,
    SecurityOperation = 6,
    QueueManagement = 7,
    Compliance = 8
}

public enum RiskLevel
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
} 