# Tier 2 Management UI - Hebrew Interface

## Quick Deployment on Port 4202

### Prerequisites
1. **Install Node.js** (version 18 or higher):
   - Download from: https://nodejs.org/
   - Verify installation: `node --version` and `npm --version`

### Option 1: Automatic Setup (Windows)
```bash
# Run the batch file (Windows)
serve.bat
```

### Option 2: Manual Setup
```bash
# Install dependencies
npm install

# Start development server on port 4202
npm start
# OR
ng serve --port 4202
```

### Option 3: Global Angular CLI
```bash
# Install Angular CLI globally (if not installed)
npm install -g @angular/cli@18

# Install project dependencies
npm install

# Serve on port 4202
ng serve --port 4202
```

## Access the Application
- **URL**: http://localhost:4202
- **Default Route**: Redirects to `/dashboard`
- **Language**: Hebrew (עברית) with RTL layout

## Available Routes
- `/dashboard` - לוח בקרה (Hebrew Dashboard)
- `/files` - ניהול קבצים (File Management)
- `/users` - ניהול משתמשים (User Management) 
- `/queues` - ניהול תורים (Queue Management)

## Hebrew Features
- ✅ Right-to-Left (RTL) layout
- ✅ Hebrew navigation menu
- ✅ Hebrew fonts (Noto Sans Hebrew)
- ✅ Israeli date/time formatting (he-IL)
- ✅ Material Design with RTL support
- ✅ Hebrew error messages

## Backend Integration
To connect with the backend APIs:
1. Start the Mock Collaboration Server: `cd ../MockCollaborationServer && dotnet run`
2. Start the Main API: `cd ../Tier2Management.API && dotnet run`
3. The Angular app is configured to connect to: http://localhost:7000

## Troubleshooting
- **Node.js not found**: Install Node.js from nodejs.org
- **npm not found**: Node.js installation includes npm
- **Port 4202 in use**: Change port in angular.json or use `ng serve --port 4203`
- **Dependencies missing**: Run `npm install`

## Development
- The application uses Angular 18 standalone components
- All components support Hebrew RTL layout
- Material Design components are configured for Hebrew
- Hot reload is enabled for development 