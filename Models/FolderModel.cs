using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tier2Management.API.Models;

public class FolderModel : BaseEntity
{
    [Required]
    [MaxLength(500)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1000)]
    public string Path { get; set; } = string.Empty;
    
    public int? ParentFolderId { get; set; }
    
    [ForeignKey(nameof(ParentFolderId))]
    public virtual FolderModel? ParentFolder { get; set; }
    
    public SecurityClassification Classification { get; set; } = SecurityClassification.Internal;
    
    public bool IsRoot { get; set; } = false;
    
    // Navigation properties
    public virtual ICollection<FolderModel> SubFolders { get; set; } = new List<FolderModel>();
    public virtual ICollection<FileModel> Files { get; set; } = new List<FileModel>();
    public virtual ICollection<FolderPermission> Permissions { get; set; } = new List<FolderPermission>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
} 