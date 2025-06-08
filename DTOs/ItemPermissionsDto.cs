namespace Tier2Management.API.DTOs;

/// <summary>
/// AD Item with access methods for permissions response
/// </summary>
public class AdItemDto
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
    public List<AdItemDto> DirectAdItems { get; set; } = new();
    public bool IsInherite { get; set; }
    public List<AdItemDto> InheriteAdItems { get; set; } = new();
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