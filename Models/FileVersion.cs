using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tier2Management.API.Models;

public class FileVersion : BaseEntity
{
    public int FileId { get; set; }
    
    [ForeignKey(nameof(FileId))]
    public virtual FileModel File { get; set; } = null!;
    
    [Required]
    public int VersionNumber { get; set; }
    
    [MaxLength(1000)]
    public string? Comments { get; set; }
    
    public long Size { get; set; }
    
    [MaxLength(32)]
    public string? MD5Hash { get; set; }
    
    [MaxLength(64)]
    public string? SHA256Hash { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string StoragePath { get; set; } = string.Empty;
    
    public bool IsCurrent { get; set; } = false;
    
    [MaxLength(500)]
    public string? VersionLabel { get; set; }
    
    public FileVersionType VersionType { get; set; } = FileVersionType.Minor;
}

public enum FileVersionType
{
    Major = 1,
    Minor = 2,
    Patch = 3,
    Snapshot = 4
} 