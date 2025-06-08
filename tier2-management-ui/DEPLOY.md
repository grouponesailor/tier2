# 🚀 Deploy Tier 2 Management System on Port 4202

## Step-by-Step Deployment Guide

### Step 1: Install Node.js (Required)
If Node.js is not installed:
1. Download from: https://nodejs.org/
2. Install the LTS version (18.x or higher)
3. Verify installation:
   ```bash
   node --version
   npm --version
   ```

### Step 2: Deploy Angular Application
Choose one of these methods:

#### Method A: Quick Start (Recommended)
```bash
# In the tier2-management-ui directory
npm install && npm start
```

#### Method B: Using Batch File (Windows)
```bash
# Double-click or run in terminal
serve.bat
```

#### Method C: Manual Commands
```bash
# Install dependencies
npm install

# Start on port 4202
ng serve --port 4202
```

### Step 3: Access the Application
1. Open browser to: **http://localhost:4202**
2. You'll see the Hebrew dashboard with RTL layout
3. Navigate using the Hebrew menu: לוח בקרה, ניהול קבצים, etc.

### Step 4: Start Backend Services (Optional)
For full functionality, start the backend:

#### Terminal 1 - Mock Server
```bash
cd ../MockCollaborationServer
dotnet run
# Runs on https://localhost:7001
```

#### Terminal 2 - Main API
```bash
cd ../Tier2Management.API
dotnet run
# Runs on https://localhost:7000
```

## 🌐 Application URLs
- **Frontend**: http://localhost:4202
- **Main API**: https://localhost:7000
- **Mock Server**: https://localhost:7001
- **API Documentation**: https://localhost:7000/swagger

## 📱 Hebrew Interface Features
- ✅ **RTL Layout**: Complete right-to-left interface
- ✅ **Hebrew Navigation**: תפריט ניווט with Hebrew labels
- ✅ **Hebrew Dashboard**: לוח בקרה with system statistics
- ✅ **Hebrew Fonts**: Noto Sans Hebrew font family
- ✅ **Israeli Localization**: Date/time formatting in Hebrew

## 🔧 Configuration
The application is pre-configured for:
- Port: 4202
- Hebrew locale: he-IL
- RTL direction: enabled
- API endpoint: http://localhost:7000
- Material Design: RTL-compatible

## 📋 Available Pages
1. **Dashboard** (`/dashboard`) - לוח בקרה
   - System overview with Hebrew statistics
   - Error queue monitoring
   - Quick action buttons

2. **File Management** (`/files`) - ניהול קבצים (planned)
3. **User Management** (`/users`) - ניהול משתמשים (planned)  
4. **Queue Management** (`/queues`) - ניהול תורים (planned)

## 🐛 Troubleshooting
| Issue | Solution |
|-------|----------|
| Node.js not found | Install from nodejs.org |
| Port 4202 in use | Use `ng serve --port 4203` |
| Dependencies missing | Run `npm install` |
| Build errors | Check Node.js version (needs 18+) |
| Hebrew not displaying | Check if Noto Sans Hebrew font loaded |

## 🎯 Next Steps
After deployment:
1. Verify Hebrew interface loads correctly
2. Test navigation between pages
3. Check responsive design on mobile
4. Test with backend APIs if running
5. Review dashboard statistics display

## 📞 Support
If you encounter issues:
1. Check the browser console for errors
2. Verify Node.js version compatibility
3. Ensure all dependencies are installed
4. Check port availability 