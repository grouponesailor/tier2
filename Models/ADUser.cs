using System.ComponentModel.DataAnnotations;

namespace Tier2Management.API.Models;

public class ADUser : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string UserName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? DisplayName { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string DistinguishedName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Department { get; set; }
    
    [MaxLength(500)]
    public string? Title { get; set; }
    
    [MaxLength(500)]
    public string? Manager { get; set; }
    
    public bool IsEnabled { get; set; } = true;
    
    public DateTime? LastLogon { get; set; }
    
    public DateTime? PasswordLastSet { get; set; }
    
    public DateTime? AccountExpires { get; set; }
    
    [MaxLength(500)]
    public string? EmployeeId { get; set; }
    
    [MaxLength(20)]
    public string? TelephoneNumber { get; set; }
    
    public DateTime LastSyncDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    public virtual ICollection<ADUserGroup> UserGroups { get; set; } = new List<ADUserGroup>();
    public virtual ICollection<FilePermission> FilePermissions { get; set; } = new List<FilePermission>();
    public virtual ICollection<FolderPermission> FolderPermissions { get; set; } = new List<FolderPermission>();
} 