using Tier2Management.API.Models;

namespace Tier2Management.API.DTOs;

// User DTOs
public class UserDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string DistinguishedName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Title { get; set; }
    public string? Manager { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? LastLogon { get; set; }
    public DateTime? PasswordLastSet { get; set; }
    public DateTime? AccountExpires { get; set; }
    public string? EmployeeId { get; set; }
    public string? TelephoneNumber { get; set; }
    public DateTime LastSyncDate { get; set; }
    public List<UserGroupDto> Groups { get; set; } = new();
    public List<UserSessionDto> ActiveSessions { get; set; } = new();
}

public class UserSummaryDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Title { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime? LastLogon { get; set; }
    public int ActiveSessionCount { get; set; }
    public int GroupCount { get; set; }
}

// Group DTOs
public class GroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DistinguishedName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SamAccountName { get; set; }
    public GroupType GroupType { get; set; }
    public GroupScope GroupScope { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime LastSyncDate { get; set; }
    public int MemberCount { get; set; }
    public List<UserSummaryDto> Members { get; set; } = new();
}

public class GroupSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DistinguishedName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GroupType GroupType { get; set; }
    public GroupScope GroupScope { get; set; }
    public bool IsEnabled { get; set; }
    public int MemberCount { get; set; }
}

public class UserGroupDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string GroupDistinguishedName { get; set; } = string.Empty;
    public bool IsDirectMember { get; set; }
    public DateTime MemberSince { get; set; }
}

// Session DTOs
public class UserSessionDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime LastActivity { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? MachineName { get; set; }
    public bool IsActive { get; set; }
    public SessionType SessionType { get; set; }
    public int ActiveLockCount { get; set; }
    public TimeSpan Duration { get; set; }
}

public class ActiveSessionSummaryDto
{
    public int TotalActiveSessions { get; set; }
    public int WebSessions { get; set; }
    public int DesktopSessions { get; set; }
    public int MobileSessions { get; set; }
    public int ApiSessions { get; set; }
    public int TotalActiveUsers { get; set; }
    public int TotalFileLocks { get; set; }
    public List<UserSessionDto> RecentSessions { get; set; } = new();
}

// Permission DTOs
public class UserPermissionDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemPath { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty; // File or Folder
    public PermissionType PermissionType { get; set; }
    public bool IsInherited { get; set; }
    public bool IsDenied { get; set; }
    public string? InheritedFrom { get; set; }
    public string? Justification { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

public class CheckUserAccessDto
{
    public string UserName { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public string ItemType { get; set; } = string.Empty; // File or Folder
    public PermissionType RequiredPermission { get; set; }
}

public class UserAccessResultDto
{
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemPath { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public PermissionType RequestedPermission { get; set; }
    public bool HasAccess { get; set; }
    public PermissionType EffectivePermissions { get; set; }
    public List<PermissionSourceDto> PermissionSources { get; set; } = new();
    public string? DenialReason { get; set; }
    public List<string> PermissionPath { get; set; } = new();
}

public class PermissionSourceDto
{
    public string SourceType { get; set; } = string.Empty; // Direct, Group, Inherited
    public string SourceName { get; set; } = string.Empty;
    public PermissionType PermissionType { get; set; }
    public bool IsDenied { get; set; }
    public bool IsInherited { get; set; }
    public string? InheritedFrom { get; set; }
}

// Bulk Operations DTOs
public class BulkUserAccessCheckDto
{
    public List<string> UserNames { get; set; } = new();
    public Guid ItemId { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public PermissionType RequiredPermission { get; set; }
}

public class BulkUserAccessResultDto
{
    public List<UserAccessResultDto> Results { get; set; } = new();
    public int TotalUsers { get; set; }
    public int UsersWithAccess { get; set; }
    public int UsersWithoutAccess { get; set; }
    public TimeSpan ProcessingTime { get; set; }
}

// AD Sync DTOs
public class ADSyncStatusDto
{
    public DateTime LastSyncTime { get; set; }
    public bool IsInProgress { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalUsers { get; set; }
    public int TotalGroups { get; set; }
    public int UsersAdded { get; set; }
    public int UsersUpdated { get; set; }
    public int UsersDisabled { get; set; }
    public int GroupsAdded { get; set; }
    public int GroupsUpdated { get; set; }
    public int ErrorCount { get; set; }
    public List<string> Errors { get; set; } = new();
    public TimeSpan SyncDuration { get; set; }
}

public class ADSyncConfigDto
{
    public string LdapServer { get; set; } = string.Empty;
    public int LdapPort { get; set; } = 389;
    public bool UseSSL { get; set; } = false;
    public string BaseDN { get; set; } = string.Empty;
    public string ServiceAccount { get; set; } = string.Empty;
    public int SyncIntervalMinutes { get; set; } = 60;
    public bool SyncUsers { get; set; } = true;
    public bool SyncGroups { get; set; } = true;
    public List<string> IncludedOUs { get; set; } = new();
    public List<string> ExcludedOUs { get; set; } = new();
} 