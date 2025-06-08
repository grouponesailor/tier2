using Microsoft.EntityFrameworkCore;
using Tier2Management.API.Models;

namespace Tier2Management.API.Data;

public class Tier2ManagementDbContext : DbContext
{
    public Tier2ManagementDbContext(DbContextOptions<Tier2ManagementDbContext> options)
        : base(options)
    {
    }

    // File Management
    public DbSet<FileModel> Files { get; set; }
    public DbSet<FolderModel> Folders { get; set; }
    public DbSet<FileVersion> FileVersions { get; set; }
    public DbSet<FileLock> FileLocks { get; set; }

    // User Management
    public DbSet<ADUser> ADUsers { get; set; }
    public DbSet<ADGroup> ADGroups { get; set; }
    public DbSet<ADUserGroup> ADUserGroups { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }

    // Permissions
    public DbSet<FilePermission> FilePermissions { get; set; }
    public DbSet<FolderPermission> FolderPermissions { get; set; }

    // Audit & Logging
    public DbSet<AuditLog> AuditLogs { get; set; }

    // Queue Management
    public DbSet<QueueOperation> QueueOperations { get; set; }
    public DbSet<QueueMessage> QueueMessages { get; set; }
    public DbSet<QueueHealth> QueueHealth { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure indexes for better performance
        ConfigureIndexes(modelBuilder);
        
        // Configure relationships
        ConfigureRelationships(modelBuilder);
        
        // Configure constraints
        ConfigureConstraints(modelBuilder);
        
        // Configure value conversions
        ConfigureValueConversions(modelBuilder);
    }

    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // File indexes
        modelBuilder.Entity<FileModel>()
            .HasIndex(f => f.Path)
            .IsUnique();

        modelBuilder.Entity<FileModel>()
            .HasIndex(f => new { f.Name, f.FolderId });

        modelBuilder.Entity<FileModel>()
            .HasIndex(f => f.IsLocked);

        // Folder indexes
        modelBuilder.Entity<FolderModel>()
            .HasIndex(f => f.Path)
            .IsUnique();

        modelBuilder.Entity<FolderModel>()
            .HasIndex(f => f.ParentFolderId);

        // User indexes
        modelBuilder.Entity<ADUser>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<ADUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<ADUser>()
            .HasIndex(u => u.DistinguishedName)
            .IsUnique();

        // Group indexes
        modelBuilder.Entity<ADGroup>()
            .HasIndex(g => g.DistinguishedName)
            .IsUnique();

        modelBuilder.Entity<ADGroup>()
            .HasIndex(g => g.SamAccountName);

        // Session indexes
        modelBuilder.Entity<UserSession>()
            .HasIndex(s => s.SessionId)
            .IsUnique();

        modelBuilder.Entity<UserSession>()
            .HasIndex(s => s.IsActive);

        // Audit indexes
        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.CreatedAt);

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => a.UserName);

        modelBuilder.Entity<AuditLog>()
            .HasIndex(a => new { a.EntityType, a.EntityId });

        // Queue indexes
        modelBuilder.Entity<QueueOperation>()
            .HasIndex(q => q.QueueName);

        modelBuilder.Entity<QueueMessage>()
            .HasIndex(q => q.QueueName);

        modelBuilder.Entity<QueueMessage>()
            .HasIndex(q => q.MessageId)
            .IsUnique();
    }

    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // File relationships
        modelBuilder.Entity<FileModel>()
            .HasOne(f => f.Folder)
            .WithMany(fo => fo.Files)
            .HasForeignKey(f => f.FolderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FileModel>()
            .HasOne(f => f.CurrentVersion)
            .WithOne()
            .HasForeignKey<FileModel>(f => f.CurrentVersionId)
            .OnDelete(DeleteBehavior.SetNull);

        // Folder relationships
        modelBuilder.Entity<FolderModel>()
            .HasOne(f => f.ParentFolder)
            .WithMany(f => f.SubFolders)
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Version relationships
        modelBuilder.Entity<FileVersion>()
            .HasOne(v => v.File)
            .WithMany(f => f.Versions)
            .HasForeignKey(v => v.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Lock relationships
        modelBuilder.Entity<FileLock>()
            .HasOne(l => l.File)
            .WithOne(f => f.Lock)
            .HasForeignKey<FileLock>(l => l.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        // User-Group relationships
        modelBuilder.Entity<ADUserGroup>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ADUserGroup>()
            .HasOne(ug => ug.Group)
            .WithMany(g => g.UserGroups)
            .HasForeignKey(ug => ug.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Permission relationships
        modelBuilder.Entity<FilePermission>()
            .HasOne(p => p.File)
            .WithMany(f => f.Permissions)
            .HasForeignKey(p => p.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FolderPermission>()
            .HasOne(p => p.Folder)
            .WithMany(f => f.Permissions)
            .HasForeignKey(p => p.FolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureConstraints(ModelBuilder modelBuilder)
    {
        // Ensure either User or Group is specified for permissions, but not both
        modelBuilder.Entity<FilePermission>()
            .HasCheckConstraint("CK_FilePermission_UserOrGroup", 
                "(UserId IS NOT NULL AND GroupId IS NULL) OR (UserId IS NULL AND GroupId IS NOT NULL)");

        modelBuilder.Entity<FolderPermission>()
            .HasCheckConstraint("CK_FolderPermission_UserOrGroup",
                "(UserId IS NOT NULL AND GroupId IS NULL) OR (UserId IS NULL AND GroupId IS NOT NULL)");

        // Ensure file path is valid
        modelBuilder.Entity<FileModel>()
            .HasCheckConstraint("CK_File_PathNotEmpty", "LEN(Path) > 0");

        // Ensure folder path is valid
        modelBuilder.Entity<FolderModel>()
            .HasCheckConstraint("CK_Folder_PathNotEmpty", "LEN(Path) > 0");
    }

    private void ConfigureValueConversions(ModelBuilder modelBuilder)
    {
        // Configure enum conversions if needed
        // This ensures enums are stored as integers in the database
        modelBuilder.Entity<FileModel>()
            .Property(f => f.Classification)
            .HasConversion<int>();

        modelBuilder.Entity<FolderModel>()
            .Property(f => f.Classification)
            .HasConversion<int>();
    }

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        var currentUser = "System"; // This should be replaced with actual user context

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = currentUser;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = currentUser;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.DeletedBy = currentUser;
                    break;
            }
        }
    }
} 