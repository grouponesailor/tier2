using Microsoft.AspNetCore.Mvc;
using MockCollaborationServer.Models;
using MockCollaborationServer.Services;

namespace MockCollaborationServer.Controllers;

[ApiController]
[Route("files")]
public class FilesSearchController : ControllerBase
{
    private readonly MockDataService _dataService;
    private readonly ILogger<FilesSearchController> _logger;

    public FilesSearchController(MockDataService dataService, ILogger<FilesSearchController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    /// <summary>
    /// Search files with optional classification filter
    /// </summary>
    [HttpPost]
    public ActionResult<FilesSearchResponse> SearchFiles([FromBody] FilesSearchRequest request, [FromQuery] string? classification = null)
    {
        try
        {
            _logger.LogInformation("Mock files search request received. Query: {Query}, System: {System}, UUID: {Uuid}, Classification: {Classification}", 
                request.Q, request.System, request.Uuid, classification);

            // Mock implementation - return sample data matching the expected format
            var response = new FilesSearchResponse
            {
                Paging = new PagingInfo
                {
                    PitId = "aaa=efgrwhfrwf",
                    Sort = new List<long> { 2658136474, 478034234238 }
                },
                Hits = new List<FileHit>
                {
                    new FileHit
                    {
                        Metadata = new FileMetadata
                        {
                            AuthorizationLevel = "FULLY_ATHORIZED",
                            Extension = "pptx",
                            UpdateDate = DateTime.Parse("2025-06-03T12:12:13"),
                            UpdateId = "",
                            FullNamePath = "/app/im/aaa/bbb/ccc.pptx",
                            AcExternalId = "",
                            AcInheriteType = 0,
                            OwnerId = "0111111",
                            Type = 1,
                            FullPath = "/app/im/aaa/bbb/ccc.pptx",
                            LastOperationDate = DateTime.Parse("2025-06-03T12:12:13.770816"),
                            ParentId = "222",
                            LastOperationByUser = "0222222",
                            ParentName = "Shoki Hahamod",
                            Size = 277882,
                            LastOperation = "file_saved",
                            Name = "fwqded",
                            Attributes = new List<object>(),
                            Id = "111",
                            Status = 1,
                            CreateDate = DateTime.Parse("2025-06-03T12:12:13")
                        }
                    }
                }
            };

            // If searching for "shoki", return relevant results
            if (!string.IsNullOrEmpty(request.Q) && request.Q.ToLower().Contains("shoki"))
            {
                response.Hits[0].Metadata.Name = "shoki_document";
                response.Hits[0].Metadata.ParentName = "Shoki Hahamod";
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching files in mock server");
            return StatusCode(500, new { message = "Error searching files" });
        }
    }
} 