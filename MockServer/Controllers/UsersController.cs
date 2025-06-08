using Microsoft.AspNetCore.Mvc;
using MockCollaborationServer.Services;

namespace MockCollaborationServer.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly MockDataService _dataService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(MockDataService dataService, ILogger<UsersController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    /// <summary>
    /// Get AD groups for a specific user
    /// </summary>
    [HttpGet("{username}/ad-groups")]
    public ActionResult<object> GetUserAdGroups(string username)
    {
        var user = _dataService.GetUserByUsername(username);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var groups = _dataService.GetUserAdGroups(username);

        return Ok(new
        {
            username = user.Username,
            displayName = user.DisplayName,
            email = user.Email,
            department = user.Department,
            groups = groups.Select(groupName => new
            {
                name = groupName,
                type = "Security",
                description = $"AD Group: {groupName}"
            })
        });
    }

    /// <summary>
    /// Check user permissions for a specific item
    /// </summary>
    [HttpGet("{username}/permissions/{itemId}")]
    public ActionResult<object> GetUserPermissions(string username, Guid itemId, [FromQuery] string? requiredAccess = "Read")
    {
        var user = _dataService.GetUserByUsername(username);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var denialReason = _dataService.CheckUserAccess(username, itemId, requiredAccess ?? "Read");
        var hasAccess = denialReason == null;

        var itemPermissions = _dataService.GetItemPermissions(itemId);

        return Ok(new
        {
            username = user.Username,
            displayName = user.DisplayName,
            itemId = itemId,
            requiredAccess = requiredAccess,
            hasAccess = hasAccess,
            denialReason = denialReason,
            permissions = itemPermissions.Where(p => 
                (p.PrincipalType == "User" && p.PrincipalId == username) ||
                (p.PrincipalType == "Group" && user.AdGroups.Contains(p.PrincipalId))
            ).Select(p => new
            {
                principalId = p.PrincipalId,
                principalType = p.PrincipalType,
                accessLevel = p.AccessLevel,
                isInherited = p.IsInherited,
                source = p.PrincipalType == "User" ? "Direct" : "Group"
            })
        });
    }

    /// <summary>
    /// Get user details
    /// </summary>
    [HttpGet("{username}")]
    public ActionResult<object> GetUser(string username)
    {
        var user = _dataService.GetUserByUsername(username);
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(new
        {
            id = user.Id,
            username = user.Username,
            displayName = user.DisplayName,
            email = user.Email,
            department = user.Department,
            title = user.Title,
            isEnabled = user.IsEnabled,
            lastLogon = user.LastLogon,
            adGroups = user.AdGroups
        });
    }

    /// <summary>
    /// Search users
    /// </summary>
    [HttpGet]
    public ActionResult<object> SearchUsers([FromQuery] string? search = null)
    {
        var users = _dataService.GetAllUsers();

        if (!string.IsNullOrEmpty(search))
        {
            users = users.Where(u => 
                u.Username.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                u.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(new
        {
            count = users.Count(),
            users = users.Select(u => new
            {
                id = u.Id,
                username = u.Username,
                displayName = u.DisplayName,
                email = u.Email,
                department = u.Department,
                title = u.Title,
                isEnabled = u.IsEnabled,
                lastLogon = u.LastLogon
            })
        });
    }
} 