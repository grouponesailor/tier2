using MockCollaborationServer.Models;
using Newtonsoft.Json;
using System.Text;

namespace MockCollaborationServer.Services;

public class MockDataService
{
    private readonly List<MockFile> _files = new();
    private readonly List<MockFolder> _folders = new();
    private readonly List<MockUser> _users = new();
    private readonly List<MockAdGroup> _adGroups = new();
    private readonly List<MockQueue> _queues = new();
    private readonly Dictionary<Guid, string> _fileContents = new(); // Store file contents for search
    private readonly ILogger<MockDataService> _logger;
    
    // Physical file storage path
    private readonly string _mockFilesPath;

    public MockDataService(ILogger<MockDataService> logger)
    {
        _logger = logger;
        
        // Set up the mock files directory
        _mockFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "MockFiles");
        
        InitializeSampleData();
    }

    #region File Operations

    public IEnumerable<MockFile> GetAllFiles() => _files.Where(f => !f.IsDeleted);

    public IEnumerable<MockFile> GetLockedFiles() => _files.Where(f => f.IsLocked && !f.IsDeleted);

    public IEnumerable<MockFile> GetDeletedFiles() => _files.Where(f => f.IsDeleted);

    public MockFile? GetFileById(Guid id) => _files.FirstOrDefault(f => f.Id == id);

    public bool UnlockFile(Guid fileId, string adminUser, string justification)
    {
        var file = GetFileById(fileId);
        if (file == null || !file.IsLocked) return false;

        file.IsLocked = false;
        file.LockedBy = null;
        file.LockTimestamp = null;

        _logger.LogInformation("File {FileId} unlocked by {AdminUser}", fileId, adminUser);
        return true;
    }

    public bool RestoreFileVersion(Guid fileId, int versionNumber, string adminUser)
    {
        var file = GetFileById(fileId);
        if (file == null) return false;

        var version = file.Versions.FirstOrDefault(v => v.VersionNumber == versionNumber);
        if (version == null) return false;

        // Mark all versions as not current
        file.Versions.ForEach(v => v.IsCurrent = false);
        
        // Create new current version based on the restored one
        var newVersion = new MockFileVersion
        {
            VersionNumber = file.Versions.Max(v => v.VersionNumber) + 1,
            CreatedBy = adminUser,
            CreatedAt = DateTime.UtcNow,
            Size = version.Size,
            Comments = $"Restored from version {versionNumber}",
            IsCurrent = true
        };

        file.Versions.Add(newVersion);

        _logger.LogInformation("File {FileId} restored to version {VersionNumber} by {AdminUser}", 
            fileId, versionNumber, adminUser);

        return true;
    }

    public bool RestoreDeletedFile(Guid fileId, string adminUser)
    {
        var file = GetFileById(fileId);
        if (file == null || !file.IsDeleted) return false;

        file.IsDeleted = false;
        file.DeletedBy = null;
        file.DeletedAt = null;

        _logger.LogInformation("File {FileId} restored from deletion by {AdminUser}", fileId, adminUser);

        return true;
    }

    #endregion

    #region Folder Operations

    public IEnumerable<MockFolder> GetAllFolders() => _folders.Where(f => !f.IsDeleted);

    public IEnumerable<MockFolder> GetDeletedFolders() => _folders.Where(f => f.IsDeleted);

    public MockFolder? GetFolderById(Guid id) => _folders.FirstOrDefault(f => f.Id == id);

    public bool RestoreDeletedFolder(Guid folderId, string adminUser)
    {
        var folder = GetFolderById(folderId);
        if (folder == null || !folder.IsDeleted) return false;

        folder.IsDeleted = false;
        folder.DeletedBy = null;
        folder.DeletedAt = null;

        _logger.LogInformation("Folder {FolderId} restored from deletion by {AdminUser}", folderId, adminUser);

        return true;
    }

    #endregion

    #region User Operations

    public IEnumerable<MockUser> GetAllUsers() => _users;

    public MockUser? GetUserByUsername(string username) => 
        _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    public MockUser? GetUserById(string id) => _users.FirstOrDefault(u => u.Id == id);

    public IEnumerable<string> GetUserAdGroups(string username)
    {
        var user = GetUserByUsername(username);
        return user?.AdGroups ?? new List<string>();
    }

    #endregion

    #region Group Operations

    public IEnumerable<MockAdGroup> GetAllGroups() => _adGroups;

    public MockAdGroup? GetGroupById(string id) => _adGroups.FirstOrDefault(g => g.Id == id);

    public MockAdGroup? GetGroupByName(string name) => 
        _adGroups.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    #endregion

    #region Permission Operations

    public IEnumerable<MockPermission> GetItemPermissions(Guid itemId)
    {
        var file = GetFileById(itemId);
        if (file != null) return file.Permissions;

        var folder = GetFolderById(itemId);
        return folder?.Permissions ?? new List<MockPermission>();
    }

    public string? CheckUserAccess(string username, Guid itemId, string requiredAccess)
    {
        var permissions = GetItemPermissions(itemId);
        var user = GetUserByUsername(username);
        
        if (user == null) return "User not found";

        // Check direct user permissions
        var userPermission = permissions.FirstOrDefault(p => 
            p.PrincipalType == "User" && p.PrincipalId == username);

        if (userPermission != null)
        {
            return HasRequiredAccess(userPermission.AccessLevel, requiredAccess) ? null : "Insufficient permissions";
        }

        // Check group permissions
        foreach (var groupName in user.AdGroups)
        {
            var groupPermission = permissions.FirstOrDefault(p => 
                p.PrincipalType == "Group" && p.PrincipalId == groupName);

            if (groupPermission != null && HasRequiredAccess(groupPermission.AccessLevel, requiredAccess))
            {
                return null;
            }
        }

        return "No access permissions found";
    }

    private static bool HasRequiredAccess(string grantedAccess, string requiredAccess)
    {
        var accessLevels = new Dictionary<string, int>
        {
            ["Read"] = 1,
            ["Write"] = 2,
            ["Modify"] = 3,
            ["Delete"] = 4
        };

        return accessLevels.GetValueOrDefault(grantedAccess, 0) >= accessLevels.GetValueOrDefault(requiredAccess, 0);
    }

    #endregion

    #region Queue Operations

    public IEnumerable<MockQueue> GetAllQueues() => _queues;

    public MockQueue? GetQueueByName(string queueName) => 
        _queues.FirstOrDefault(q => q.Name.Equals(queueName, StringComparison.OrdinalIgnoreCase));

    public int GetErrorQueueCount()
    {
        var errorQueue = _queues.FirstOrDefault(q => q.Name == "error");
        return errorQueue?.Messages.Count ?? 0;
    }

    public MockQueueMessage? FindItemInQueue(string queueName, Guid itemId)
    {
        var queue = GetQueueByName(queueName);
        return queue?.Messages.FirstOrDefault(m => m.ItemId == itemId);
    }

    public IEnumerable<MockQueueMessage> GetQueueMessagesPreview(string queueName, int count = 10)
    {
        var queue = GetQueueByName(queueName);
        return queue?.Messages.Take(count) ?? new List<MockQueueMessage>();
    }

    public bool TransferMessages(string sourceQueue, string targetQueue, List<Guid> messageIds, string adminUser)
    {
        var source = GetQueueByName(sourceQueue);
        var target = GetQueueByName(targetQueue);

        if (source == null || target == null) return false;

        var messagesToTransfer = source.Messages.Where(m => messageIds.Contains(m.Id)).ToList();

        foreach (var message in messagesToTransfer)
        {
            source.Messages.Remove(message);
            message.QueueName = targetQueue;
            target.Messages.Add(message);
        }

        _logger.LogInformation("Transferred {Count} messages from {SourceQueue} to {TargetQueue} by {AdminUser}", 
            messagesToTransfer.Count, sourceQueue, targetQueue, adminUser);

        return true;
    }

    #endregion

    #region Classification Operations

    public IEnumerable<MockClassificationHistory> GetClassificationHistory(Guid itemId)
    {
        var file = GetFileById(itemId);
        if (file != null) return file.Classification.History;

        var folder = GetFolderById(itemId);
        return folder?.Classification.History ?? new List<MockClassificationHistory>();
    }

    #endregion

    #region Search Operations

    /// <summary>
    /// Search files by name and content
    /// </summary>
    public IEnumerable<MockFile> SearchFiles(string? query, string? classification = null)
    {
        var files = GetAllFiles();
        
        // Filter by classification if provided
        if (!string.IsNullOrEmpty(classification) && classification == "hide_classified")
        {
            files = files.Where(f => f.Classification.Level != "Confidential" && f.Classification.Level != "Secret");
        }

        // If no query, return all files
        if (string.IsNullOrEmpty(query))
        {
            return files;
        }

        var queryLower = query.ToLower();
        
        // Search by filename and content
        return files.Where(file => 
        {
            // Search in filename
            if (file.Name.ToLower().Contains(queryLower))
                return true;

            // Search in file content if exists
            if (_fileContents.TryGetValue(file.Id, out var filePath))
            {
                try
                {
                    var fileContent = File.ReadAllText(filePath, Encoding.UTF8);
                    return fileContent.ToLower().Contains(queryLower);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to read file content for search: {FilePath}", filePath);
                    return false;
                }
            }

            return false;
        });
    }

    #endregion

    #region Data Initialization

    /// <summary>
    /// Saves file content to physical .txt file on disk
    /// </summary>
    private void SaveFileToDisc(Guid fileId, string fileName, string content)
    {
        try
        {
            // Ensure the MockFiles directory exists
            if (!Directory.Exists(_mockFilesPath))
            {
                Directory.CreateDirectory(_mockFilesPath);
                _logger.LogInformation("Created MockFiles directory at {Path}", _mockFilesPath);
            }

            // Create filename with ID to ensure uniqueness (remove original extension, add .txt)
            var txtFileName = $"{fileId}_{Path.GetFileNameWithoutExtension(fileName)}.txt";
            var filePath = Path.Combine(_mockFilesPath, txtFileName);

            // Write content to file
            File.WriteAllText(filePath, content, Encoding.UTF8);
            
            _logger.LogDebug("Saved mock file {FileName} to {FilePath}", fileName, filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save mock file {FileName} to disk", fileName);
        }
    }

    /// <summary>
    /// Loads file content from physical file on disk
    /// </summary>
    private string? LoadFileFromDisc(Guid fileId, string fileName)
    {
        try
        {
            var txtFileName = $"{fileId}_{Path.GetFileNameWithoutExtension(fileName)}.txt";
            var filePath = Path.Combine(_mockFilesPath, txtFileName);

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath, Encoding.UTF8);
            }

            _logger.LogWarning("Mock file not found on disk: {FilePath}", filePath);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load mock file {FileName} from disk", fileName);
            return null;
        }
    }

    private void InitializeSampleData()
    {
        InitializeUsers();
        InitializeGroups();
        InitializeFolders();
        InitializeFiles();
        InitializeQueues();
        
        _logger.LogInformation("Mock data initialized");
    }

    private void InitializeUsers()
    {
        _users.AddRange(new[]
        {
            new MockUser
            {
                Id = "user1",
                Username = "john.doe",
                DisplayName = "John Doe",
                Email = "john.doe@company.com",
                AdGroups = ["Domain Users", "File Managers"],
                Department = "IT"
            },
            new MockUser
            {
                Id = "user2",
                Username = "jane.smith",
                DisplayName = "Jane Smith",
                Email = "jane.smith@company.com",
                AdGroups = ["Domain Users", "Knowledge Managers"],
                Department = "Operations",
                Title = "Knowledge Manager",
                LastLogon = DateTime.UtcNow.AddHours(-1)
            },
            new MockUser
            {
                Id = "user3",
                Username = "bob.wilson",
                DisplayName = "Bob Wilson",
                Email = "bob.wilson@company.com",
                AdGroups = ["Domain Users", "Developers"],
                Department = "Development",
                Title = "Senior Developer",
                LastLogon = DateTime.UtcNow.AddMinutes(-30)
            }
        });
    }

    private void InitializeGroups()
    {
        _adGroups.AddRange(new[]
        {
            new MockAdGroup
            {
                Id = "group1",
                Name = "Domain Users",
                Description = "All domain users",
                Members = ["john.doe", "jane.smith", "bob.wilson"]
            },
            new MockAdGroup
            {
                Id = "group2",
                Name = "File Managers",
                Description = "Users who can manage files",
                Members = ["john.doe", "jane.smith"]
            },
            new MockAdGroup
            {
                Id = "group3",
                Name = "Knowledge Managers",
                Description = "Content and permission administrators",
                Members = ["jane.smith"]
            },
            new MockAdGroup
            {
                Id = "group4",
                Name = "Help Desk",
                Description = "Help desk personnel",
                Members = ["john.doe"]
            },
            new MockAdGroup
            {
                Id = "group5",
                Name = "Developers",
                Description = "Development team members",
                Members = ["bob.wilson"]
            }
        });
    }

    private void InitializeFolders()
    {
        var rootFolder = new MockFolder
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Name = "Root",
            Path = "/",
            ParentFolderId = null,
            CreatedBy = "system",
            Classification = new MockClassification { Level = "Internal" }
        };

        var documentsFolder = new MockFolder
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Name = "Documents",
            Path = "/Documents",
            ParentFolderId = rootFolder.Id,
            CreatedBy = "system",
            Classification = new MockClassification { Level = "Internal" }
        };

        var projectsFolder = new MockFolder
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Name = "Projects",
            Path = "/Projects",
            ParentFolderId = rootFolder.Id,
            CreatedBy = "system",
            Classification = new MockClassification { Level = "Confidential" }
        };

        // Add permissions
        rootFolder.Permissions.Add(new MockPermission
        {
            PrincipalId = "Domain Users",
            PrincipalType = "Group",
            AccessLevel = "Read"
        });

        documentsFolder.Permissions.Add(new MockPermission
        {
            PrincipalId = "File Managers",
            PrincipalType = "Group",
            AccessLevel = "Modify"
        });

        projectsFolder.Permissions.Add(new MockPermission
        {
            PrincipalId = "Developers",
            PrincipalType = "Group",
            AccessLevel = "Delete"
        });

        _folders.AddRange([rootFolder, documentsFolder, projectsFolder]);
    }

    private void InitializeFiles()
    {
        var documentsFolder = _folders.First(f => f.Name == "Documents");
        var projectsFolder = _folders.First(f => f.Name == "Projects");
        var random = new Random(42); // Fixed seed for consistent data

        // Path to the mockupdata directory
        var mockupDataPath = Path.Combine(Directory.GetCurrentDirectory(), "mockupdata");
        
        if (!Directory.Exists(mockupDataPath))
        {
            _logger.LogWarning("Mockupdata directory not found at {Path}", mockupDataPath);
            return;
        }

        var files = new List<MockFile>();
        var txtFiles = Directory.GetFiles(mockupDataPath, "*.txt");
        
        var classifications = new[] { "Public", "Internal", "Confidential", "Secret" };
        var users = new[] { "john.doe", "jane.smith", "bob.wilson", "alice.brown", "charlie.davis", "diana.lee", "frank.miller", "grace.wong" };
        var paths = new[] { "/Documents", "/Projects", "/Documents/Archive", "/Projects/Current", "/Projects/Completed" };

        foreach (var filePath in txtFiles)
        {
            var fileName = Path.GetFileName(filePath);
            var fileId = Guid.NewGuid();
            var creator = users[random.Next(users.Length)];
            var classification = classifications[random.Next(classifications.Length)];
            var folderPath = paths[random.Next(paths.Length)];
            
            var file = new MockFile
            {
                Id = fileId,
                Name = fileName,
                Path = $"{folderPath}/{fileName}",
                Size = new FileInfo(filePath).Length,
                Extension = "txt",
                IsLocked = random.Next(100) < 15, // 15% chance of being locked
                LockedBy = random.Next(100) < 15 ? users[random.Next(users.Length)] : null,
                LockTimestamp = random.Next(100) < 15 ? DateTime.UtcNow.AddHours(-random.Next(1, 48)) : null,
                CreatedBy = creator,
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                Classification = new MockClassification
                {
                    Level = classification,
                    ChangedBy = creator,
                    ChangedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    Justification = $"Set to {classification} based on content sensitivity"
                }
            };

            // Store the file path for content loading
            _fileContents[fileId] = filePath; // Store file path instead of content

            // Add versions
            file.Versions.AddRange(new[]
            {
                new MockFileVersion
                {
                    VersionNumber = 1,
                    CreatedBy = file.CreatedBy ?? "system",
                    CreatedAt = file.CreatedAt,
                    Size = file.Size - random.Next(100, 1000),
                    Comments = "Initial version"
                },
                new MockFileVersion
                {
                    VersionNumber = 2,
                    CreatedBy = file.CreatedBy ?? "system",
                    CreatedAt = file.CreatedAt.AddDays(random.Next(1, 30)),
                    Size = file.Size,
                    Comments = "Updated content and formatting",
                    IsCurrent = true
                }
            });

            // Add permissions
            file.Permissions.AddRange(new[]
            {
                new MockPermission
                {
                    PrincipalId = file.CreatedBy ?? "system",
                    PrincipalType = "User",
                    AccessLevel = "Delete"
                },
                new MockPermission
                {
                    PrincipalId = "File Managers",
                    PrincipalType = "Group",
                    AccessLevel = "Modify",
                    IsInherited = true,
                    InheritedFrom = "Parent Folder"
                }
            });

            files.Add(file);
        }

        _files.AddRange(files);
        _logger.LogInformation("Loaded {FileCount} files from mockupdata directory", files.Count);
    }

    private void InitializeQueues()
    {
        var errorQueue = new MockQueue { Name = "error" };
        var processingQueue = new MockQueue { Name = "processing" };
        var completedQueue = new MockQueue { Name = "completed" };

        // Add sample error messages
        errorQueue.Messages.AddRange(new[]
        {
            new MockQueueMessage
            {
                QueueName = "error",
                MessageId = "msg_001",
                MessageBody = JsonConvert.SerializeObject(new { FileId = "10000000-0000-0000-0000-000000000001", Action = "Lock" }),
                Status = "Failed",
                ErrorMessage = "File already locked by another user",
                RetryCount = 3,
                ItemId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Headers = new Dictionary<string, object> { ["ContentType"] = "application/json", ["Source"] = "FileService" }
            },
            new MockQueueMessage
            {
                QueueName = "error",
                MessageId = "msg_002",
                MessageBody = JsonConvert.SerializeObject(new { UserId = "invalid_user", Action = "GetPermissions" }),
                Status = "Failed",
                ErrorMessage = "User not found in Active Directory",
                RetryCount = 1,
                Headers = new Dictionary<string, object> { ["ContentType"] = "application/json", ["Source"] = "UserService" }
            }
        });

        _queues.AddRange([errorQueue, processingQueue, completedQueue]);
    }

    #endregion
} 