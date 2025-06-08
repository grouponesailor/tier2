using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tier2Management.API.Models;

public class FilePermission : BaseEntity
{
    public int FileId { get; set; }
    
    [ForeignKey(nameof(FileId))]
    public virtual FileModel File { get; set; } = null!;
    
    public int? UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public virtual ADUser? User { get; set; }
    
    public int? GroupId { get; set; }
    
    [ForeignKey(nameof(GroupId))]
    public virtual ADGroup? Group { get; set; }
    
    public PermissionType PermissionType { get; set; }
    
    public bool IsInherited { get; set; } = false;
    
    public int? InheritedFromFolderId { get; set; }
    
    public bool IsDenied { get; set; } = false;
    
    [MaxLength(1000)]
    public string? Justification { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
}

public class FolderPermission : BaseEntity
{
    public int FolderId { get; set; }
    
    [ForeignKey(nameof(FolderId))]
    public virtual FolderModel Folder { get; set; } = null!;
    
    public int? UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public virtual ADUser? User { get; set; }
    
    public int? GroupId { get; set; }
    
    [ForeignKey(nameof(GroupId))]
    public virtual ADGroup? Group { get; set; }
    
    public PermissionType PermissionType { get; set; }
    
    public bool ApplyToChildren { get; set; } = true;
    
    public bool ApplyToFiles { get; set; } = true;
    
    public bool IsDenied { get; set; } = false;
    
    [MaxLength(1000)]
    public string? Justification { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
}

[Flags]
public enum PermissionType
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4,
    Delete = 8,
    ChangePermissions = 16,
    TakeOwnership = 32,
    FullControl = Read | Write | Execute | Delete | ChangePermissions | TakeOwnership
} 