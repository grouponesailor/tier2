using Microsoft.AspNetCore.Mvc;
using Tier2Management.API.DTOs;
using Tier2Management.API.Services;

namespace Tier2Management.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserManagementService userManagementService, ILogger<UsersController> logger)
    {
        _userManagementService = userManagementService;
        _logger = logger;
    }

    /// <summary>
    /// חיפוש משתמשים (Search users)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> SearchUsers([FromQuery] string? search = null)
    {
        try
        {
            var users = await _userManagementService.SearchUsersAsync(search);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            return StatusCode(500, new { message = "שגיאה בחיפוש משתמשים" });
        }
    }

    /// <summary>
    /// שליפת פרטי משתמש (Get user details)
    /// </summary>
    [HttpGet("{username}")]
    public async Task<ActionResult<UserDto>> GetUser(string username)
    {
        try
        {
            var user = await _userManagementService.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound(new { message = "משתמש לא נמצא" });

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {Username}", username);
            return StatusCode(500, new { message = "שגיאה בשליפת פרטי המשתמש" });
        }
    }

    /// <summary>
    /// שליפת קבוצות AD של משתמש (Get user AD groups)
    /// </summary>
    [HttpGet("{username}/ad-groups")]
    public async Task<ActionResult<List<string>>> GetUserAdGroups(string username)
    {
        try
        {
            var groups = await _userManagementService.GetUserAdGroupsAsync(username);
            return Ok(new
            {
                username = username,
                groups = groups,
                count = groups.Count,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving AD groups for user {Username}", username);
            return StatusCode(500, new { message = "שגיאה בשליפת קבוצות AD" });
        }
    }

    /// <summary>
    /// בדיקת הרשאות משתמש על פריט (Check user access to item)
    /// </summary>
    [HttpGet("{username}/check-access/{itemId}")]
    public async Task<ActionResult<UserAccessResultDto>> CheckUserAccess(
        string username, 
        Guid itemId, 
        [FromQuery] string requiredAccess = "Read")
    {
        try
        {
            var result = await _userManagementService.CheckUserAccessAsync(username, itemId, requiredAccess);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user access");
            return StatusCode(500, new { message = "שגיאה בבדיקת הרשאות המשתמש" });
        }
    }

    /// <summary>
    /// שליפת משתמשים פעילים (Get active users)
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<List<UserSummaryDto>>> GetActiveUsers()
    {
        try
        {
            var users = await _userManagementService.GetActiveUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active users");
            return StatusCode(500, new { message = "שגיאה בשליפת משתמשים פעילים" });
        }
    }

    /// <summary>
    /// שליפת סטטוס סנכרון AD (Get AD sync status)
    /// </summary>
    [HttpGet("ad-sync-status")]
    public async Task<ActionResult<ADSyncStatusDto>> GetAdSyncStatus()
    {
        try
        {
            var status = await _userManagementService.GetAdSyncStatusAsync();
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving AD sync status");
            return StatusCode(500, new { message = "שגיאה בשליפת סטטוס סנכרון AD" });
        }
    }
} 