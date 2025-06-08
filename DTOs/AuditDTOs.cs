using Tier2Management.API.Models;

namespace Tier2Management.API.DTOs;

public class AuditLogDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? EntityName { get; set; }
    public AuditLogType LogType { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? MachineName { get; set; }
    public string? Comments { get; set; }
    public SecurityClassification? PreviousClassification { get; set; }
    public SecurityClassification? NewClassification { get; set; }
    public string? ClassificationJustification { get; set; }
    public AuditCategory Category { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public bool RequiresReview { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public string? ReviewComments { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAuditLogDto
{
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? EntityName { get; set; }
    public AuditLogType LogType { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? Comments { get; set; }
    public SecurityClassification? PreviousClassification { get; set; }
    public SecurityClassification? NewClassification { get; set; }
    public string? ClassificationJustification { get; set; }
    public AuditCategory Category { get; set; }
    public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;
}

public class AuditSearchDto
{
    public string? UserName { get; set; }
    public string? Action { get; set; }
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public AuditLogType? LogType { get; set; }
    public AuditCategory? Category { get; set; }
    public RiskLevel? MinRiskLevel { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? IpAddress { get; set; }
    public bool? RequiresReview { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}

public class AuditSearchResultDto
{
    public List<AuditLogDto> Logs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public class ClassificationChangeLogDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityPath { get; set; } = string.Empty;
    public SecurityClassification PreviousClassification { get; set; }
    public SecurityClassification NewClassification { get; set; }
    public string? Justification { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? IpAddress { get; set; }
    public bool RequiresReview { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewedBy { get; set; }
    public string? ReviewComments { get; set; }
}

public class ComplianceReportDto
{
    public string ReportId { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public ComplianceReportType ReportType { get; set; }
    public ComplianceReportSummaryDto Summary { get; set; } = new();
    public List<ComplianceViolationDto> Violations { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class ComplianceReportSummaryDto
{
    public int TotalAuditEntries { get; set; }
    public int HighRiskActivities { get; set; }
    public int ClassificationChanges { get; set; }
    public int UnauthorizedAccess { get; set; }
    public int FilesDeletions { get; set; }
    public int PermissionChanges { get; set; }
    public int QueueOperations { get; set; }
    public double ComplianceScore { get; set; }
    public List<string> TopUsers { get; set; } = new();
    public List<string> TopActions { get; set; } = new();
}

public class ComplianceViolationDto
{
    public int Id { get; set; }
    public string ViolationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RiskLevel Severity { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string? EntityName { get; set; }
    public string? EntityPath { get; set; }
    public string? IpAddress { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }
    public string? Resolution { get; set; }
}

public class SystemActivityDto
{
    public DateTime Timestamp { get; set; }
    public int ActiveUsers { get; set; }
    public int ActiveSessions { get; set; }
    public int FilesLocked { get; set; }
    public int QueueMessages { get; set; }
    public int AuditEntries { get; set; }
    public double SystemLoad { get; set; }
    public double MemoryUsage { get; set; }
    public double DiskUsage { get; set; }
}

public class ActivityDashboardDto
{
    public DateTime LastUpdated { get; set; }
    public int TotalActiveUsers { get; set; }
    public int TotalActiveSessions { get; set; }
    public int TotalLockedFiles { get; set; }
    public int TotalQueueMessages { get; set; }
    public int TodayAuditEntries { get; set; }
    public int TodayHighRiskActivities { get; set; }
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<SystemActivityDto> ActivityChart { get; set; } = new();
    public List<TopUserActivityDto> TopUsers { get; set; } = new();
}

public class RecentActivityDto
{
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public string? IpAddress { get; set; }
}

public class TopUserActivityDto
{
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
    public int HighRiskCount { get; set; }
    public DateTime LastActivity { get; set; }
    public List<string> TopActions { get; set; } = new();
}

public enum ComplianceReportType
{
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
    Quarterly = 4,
    Annual = 5,
    Custom = 6
} 