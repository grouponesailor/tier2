# Tier 2 Management System

## Overview
A comprehensive administrative tool for corporate file management platform with Hebrew RTL interface. The system provides Help Desk Personnel, Knowledge Managers, and Development Team with tools to manage file locks, recover deleted items, verify permissions, and monitor RabbitMQ queues.

## Architecture
- **Frontend**: Angular 18 with Hebrew RTL support and Material Design
- **Backend**: .NET 8 Web API
- **Mock Server**: .NET 8 collaboration server simulator
- **Authentication**: ADFS integration
- **Messaging**: RabbitMQ
- **Logging**: Serilog

## Project Structure

### Backend (.NET 8 API)
```
Tier2Management.API/
├── Controllers/
│   ├── FilesController.cs          # File management operations
│   ├── DeletedItemsController.cs   # Deleted item recovery
│   ├── UsersController.cs          # User and AD operations
│   └── QueuesController.cs         # RabbitMQ queue management
├── Services/
│   ├── IFileManagementService.cs   # File operations interface
│   ├── FileManagementService.cs    # File operations implementation
│   ├── IUserManagementService.cs   # User operations interface
│   ├── UserManagementService.cs    # User operations implementation
│   ├── IQueueManagementService.cs  # Queue operations interface
│   └── QueueManagementService.cs   # Queue operations implementation
├── Models/                         # Entity models
├── DTOs/                          # Data transfer objects
├── Data/                          # Entity Framework context
└── Program.cs                     # Application configuration
```

### Mock Collaboration Server (.NET 8)
```
MockCollaborationServer/
├── Controllers/
│   ├── FilesController.cs         # Mock file operations
│   ├── DeletedItemsController.cs  # Mock deleted items
│   ├── UsersController.cs         # Mock user operations
│   └── QueuesController.cs        # Mock queue operations
├── Models/                        # Mock data models
├── Services/
│   └── MockDataService.cs         # In-memory data management
└── Program.cs                     # Mock server configuration
```

### Frontend (Angular 18 Hebrew UI)
```
tier2-management-ui/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   ├── dashboard/         # Hebrew dashboard component
│   │   │   ├── file-management/   # File operations (planned)
│   │   │   ├── user-management/   # User operations (planned)
│   │   │   └── queue-management/  # Queue operations (planned)
│   │   ├── services/              # API communication services (planned)
│   │   ├── models/                # TypeScript interfaces (planned)
│   │   ├── app.component.ts       # Main app with Hebrew navigation
│   │   ├── app.config.ts          # Angular configuration with Hebrew locale
│   │   └── app.routes.ts          # Routing configuration
│   ├── styles.scss                # Global Hebrew RTL styles
│   └── index.html                 # HTML with Hebrew RTL support
├── angular.json                   # Angular project configuration
├── package.json                   # Dependencies
└── tsconfig.json                  # TypeScript configuration
```

## Implementation Status

### ✅ Completed
1. **Backend API Structure**
   - All controllers implemented with Hebrew error messages
   - Service layer with HTTP client integration to mock server
   - Complete DTOs for all operations
   - Entity models with audit trails
   - Program.cs configured with CORS, Serilog, health checks

2. **Mock Collaboration Server**
   - Complete implementation with realistic sample data
   - All required endpoints matching specification
   - In-memory data management
   - Swagger documentation

3. **Angular Project Foundation**
   - Project structure created
   - Hebrew RTL configuration
   - Main app component with Hebrew navigation
   - Dashboard component with Hebrew interface
   - Global styles with RTL support

### 🚧 In Progress / Next Steps
1. **Angular Frontend Completion**
   - Install Node.js and npm dependencies
   - Create remaining components (file-management, user-management, queue-management)
   - Implement API services
   - Add form validation and error handling

2. **Integration Testing**
   - Test API endpoints with mock server
   - Verify Hebrew UI rendering
   - Test CORS configuration

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- Visual Studio or VS Code

### Backend Setup
1. **Start Mock Collaboration Server**
   ```bash
   cd MockCollaborationServer
   dotnet run
   # Server runs on https://localhost:7001
   ```

2. **Start Main API**
   ```bash
   cd Tier2Management.API
   dotnet run
   # API runs on https://localhost:7000
   ```

### Frontend Setup
1. **Install Dependencies**
   ```bash
   cd tier2-management-ui
   npm install
   ```

2. **Start Development Server**
   ```bash
   npm start
   # UI runs on http://localhost:4200
   ```

## API Endpoints

### Files Management
- `GET /api/files` - List files with search and pagination
- `POST /api/files/unlock` - שחרור נעילת קובץ (Release file lock)
- `GET /api/midur/getItemPermissions/{itemId}` - שליפת הרשאות פריט (Get item permissions)
- `GET /api/files/{id}/versions` - Get file versions
- `POST /api/files/{id}/restore-version` - Restore file version

### Deleted Items
- `GET /api/deleted-items` - List deleted items
- `POST /api/deleted-items/{id}/restore` - Restore deleted item

### Users Management
- `GET /api/users` - Search users
- `GET /api/users/{username}` - Get user details
- `GET /api/users/{username}/ad-groups` - Get user AD groups
- `GET /api/users/{username}/check-access/{itemId}` - Check user permissions

### Queue Management
- `GET /api/queues` - List all queues
- `GET /api/queues/error/count` - Get error queue count
- `GET /api/queues/{queueName}/search/{itemId}` - Find item in queue
- `POST /api/queues/transfer` - Transfer messages between queues

## Hebrew UI Features
- **RTL Layout**: Complete right-to-left interface
- **Hebrew Fonts**: Noto Sans Hebrew font family
- **Date/Number Formatting**: Israeli locale (he-IL)
- **Navigation**: Hebrew menu items and labels
- **Error Messages**: Hebrew error and success messages
- **Material Design**: RTL-compatible Material Design components

## Configuration

### API Configuration (appsettings.json)
```json
{
  "MockCollaborationServer": {
    "BaseUrl": "https://localhost:7001"
  },
  "RabbitMQ": {
    "ConnectionString": "amqp://localhost:5672",
    "Username": "guest",
    "Password": "guest"
  },
  "ActiveDirectory": {
    "Domain": "corp.company.com",
    "LdapPath": "LDAP://corp.company.com"
  }
}
```

### Angular Configuration
- Hebrew locale (he-IL)
- RTL direction
- Material Design theme
- API base URL: http://localhost:7000

## Security Features
- ADFS authentication integration
- Role-based access control (3 levels)
- Audit logging for all operations
- CORS configuration for Angular frontend
- Input validation and sanitization

## Monitoring & Logging
- Serilog structured logging
- Health check endpoints
- Error queue monitoring
- System status dashboard
- Performance metrics

## Development Notes
- All Hebrew text uses proper RTL formatting
- Error messages are user-friendly in Hebrew
- API responses include Hebrew descriptions
- Mock server provides realistic test data
- Components use standalone Angular 18 pattern

## Next Development Phase
1. Complete Angular component implementation
2. Add real-time notifications with SignalR
3. Implement comprehensive error handling
4. Add unit and integration tests
5. Deploy to staging environment
6. User acceptance testing with Hebrew speakers 