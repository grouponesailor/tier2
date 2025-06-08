namespace MockCollaborationServer.Models;

/// <summary>
/// Response header for API responses
/// </summary>
public class ResponseHeader
{
    public string ReqId { get; set; } = string.Empty;
}

/// <summary>
/// AD Item with access methods for permissions response
/// </summary>
public class AdItem
{
    public string AdId { get; set; } = string.Empty;
    public string AdName { get; set; } = string.Empty;
    public List<int> AccessMethods { get; set; } = new();
}

/// <summary>
/// Response body for item permissions
/// </summary>
public class ItemPermissionsResponseBody
{
    public int ItemId { get; set; }
    public List<AdItem> DirectAdItems { get; set; } = new();
    public bool IsInherite { get; set; }
    public List<AdItem> InheriteAdItems { get; set; } = new();
}

/// <summary>
/// Complete response for item permissions
/// </summary>
public class ItemPermissionsResponse
{
    public ItemPermissionsResponseBody ResponseBody { get; set; } = new();
    public ResponseHeader ResponseHeader { get; set; } = new();
    public string? Ex { get; set; }
} 