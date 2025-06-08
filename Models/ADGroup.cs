using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tier2Management.API.Models;

public class ADGroup : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string DistinguishedName { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [MaxLength(500)]
    public string? SamAccountName { get; set; }
    
    public GroupType GroupType { get; set; } = GroupType.Security;
    
    public GroupScope GroupScope { get; set; } = GroupScope.Global;
    
    public bool IsEnabled { get; set; } = true;
    
    public DateTime LastSyncDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual ICollection<ADUserGroup> UserGroups { get; set; } = new List<ADUserGroup>();
    public virtual ICollection<FilePermission> FilePermissions { get; set; } = new List<FilePermission>();
    public virtual ICollection<FolderPermission> FolderPermissions { get; set; } = new List<FolderPermission>();
}

public class ADUserGroup : BaseEntity
{
    public int UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public virtual ADUser User { get; set; } = null!;
    
    public int GroupId { get; set; }
    
    [ForeignKey(nameof(GroupId))]
    public virtual ADGroup Group { get; set; } = null!;
    
    public bool IsDirectMember { get; set; } = true;
    
    public DateTime MemberSince { get; set; } = DateTime.UtcNow;
}

public enum GroupType
{
    Security = 1,
    Distribution = 2
}

public enum GroupScope
{
    DomainLocal = 1,
    Global = 2,
    Universal = 3
} 