# 7DTD Server Management Web App

A modern web application for monitoring and controlling your 7 Days to Die dedicated server. Built with Blazor WebAssembly for a responsive, client-side experience.

## Overview

This static web application provides a user-friendly interface to:
- Monitor your 7 Days to Die server status
- View currently connected players
- Control server operations (start/stop)
- Access server management features

The application is designed to work seamlessly with the 7DTD server infrastructure deployed on Azure, providing real-time monitoring and control capabilities through an intuitive web interface.

## Technology Stack

- **Framework**: .NET 8.0
- **UI Framework**: Blazor WebAssembly
- **Language**: C#
- **Styling**: Custom CSS with dark theme
- **Fonts**: Inter (Google Fonts)
- **Deployment**: Static Web App (Azure compatible)

## Prerequisites

Before running this application, ensure you have:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A modern web browser (Chrome, Firefox, Safari, Edge)
- Internet connection (for Google Fonts and initial package restore)

## Getting Started

### 1. Clone and Navigate

```bash
git clone https://github.com/tgreer812/7dtd_azure_vm.git
cd 7dtd_azure_vm/App
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Application

```bash
dotnet build
```

### 4. Run Locally

```bash
dotnet run
```

The application will be available at:
- HTTP: `http://localhost:5250`
- Development server with hot reload support

### 5. Development Mode

For development with automatic rebuilds:

```bash
dotnet watch run
```

## Project Structure

```
App/
├── wwwroot/                    # Static web assets
│   ├── css/
│   │   └── app.css            # Global styles
│   └── index.html             # Main HTML template
├── Layout/                     # Layout components
│   ├── MainLayout.razor       # Primary layout with header/nav
│   ├── MainLayout.razor.css   # Layout-specific styles
│   ├── NavMenu.razor          # Navigation menu component
│   └── NavMenu.razor.css      # Navigation styles
├── Pages/                      # Page components
│   ├── Home.razor             # Home/welcome page
│   └── ServerStatus.razor     # Server monitoring page
├── App.razor                   # Root application component
├── Program.cs                  # Application entry point
├── _Imports.razor             # Global using statements
├── App.csproj                 # Project configuration
└── Properties/
    └── launchSettings.json    # Development server settings
```

## Features

### Current Functionality

- **Home Page** (`/`)
  - Welcome message and application overview
  - Clean, branded interface

- **Server Status Page** (`/server`)
  - Real-time server status display
  - Connected players list
  - Server control buttons (Start/Stop)
  - Player count indicator

### User Interface

- **Dark Theme**: Easy on the eyes with a professional gaming aesthetic
- **Red Accent Color** (#e04b3d): Consistent branding throughout
- **Responsive Design**: Works on desktop and mobile devices
- **Clean Navigation**: Simple header-based navigation between pages

### Technical Features

- **Client-Side Rendering**: Fast, responsive user experience
- **Component-Based Architecture**: Modular and maintainable code
- **CSS Isolation**: Scoped styles for better maintainability
- **Blazor Router**: SPA-style navigation without page refreshes

## Development Workflow

### Making Changes

1. **Components**: Add new `.razor` files in appropriate folders
2. **Styles**: Use component-scoped CSS (`.razor.css`) or global styles in `wwwroot/css/`
3. **Pages**: Add new pages in the `Pages/` folder with `@page` directive
4. **Services**: Register services in `Program.cs`

### Code Style

- Follow C# and Blazor naming conventions
- Use component-scoped CSS when possible
- Keep components focused and single-responsibility
- Use async/await for all asynchronous operations

### Hot Reload

Use `dotnet watch run` for development to automatically rebuild and refresh the browser when code changes are detected.

## API Integration

### Current State

The application currently uses mock data for demonstration purposes:
- Server status is hardcoded to "Online"
- Player list contains sample data
- Server control buttons simulate actions

### Future Integration

To connect to a real 7DTD server:

1. **HTTP Client**: Configured in `Program.cs` for API calls
2. **Server Endpoints**: Replace mock methods in `ServerStatus.razor`
3. **Real-time Updates**: Consider SignalR for live data
4. **Authentication**: Add security for server control operations

Example API integration:

```csharp
private async Task<string> GetServerStatus()
{
    try 
    {
        var response = await Http.GetFromJsonAsync<ServerStatusResponse>("/api/server/status");
        return response?.Status ?? "Unknown";
    }
    catch 
    {
        return "Offline";
    }
}
```

## Deployment

### Build for Production

```bash
dotnet publish -c Release -o publish
```

The output in `publish/wwwroot/` contains all static files needed for deployment.

### Azure Static Web Apps

This application is designed for Azure Static Web Apps deployment:

1. **Build Configuration**: Uses standard Blazor WebAssembly output
2. **Routing**: Supports client-side routing with fallback to index.html
3. **Static Assets**: All resources are self-contained

### Local Testing

Test the production build locally:

```bash
dotnet publish -c Release
cd bin/Release/net8.0/publish/wwwroot
python -m http.server 8000  # Or any static file server
```

## Configuration

### Launch Settings

Development server configuration in `Properties/launchSettings.json`:
- Default port: 5250
- Browser launch: Enabled
- Debug proxy: Configured for Blazor debugging

### Application Settings

For production configurations, consider:
- API base URLs
- Feature flags
- Authentication settings
- Logging configuration

## Troubleshooting

### Common Issues

1. **Build Errors**
   - Ensure .NET 8.0 SDK is installed
   - Run `dotnet restore` to restore packages

2. **Runtime Errors**
   - Check browser console for JavaScript errors
   - Verify all dependencies are correctly referenced

3. **Styling Issues**
   - Clear browser cache
   - Check CSS file references in components

### Performance Optimization

- Enable Blazor WebAssembly compression
- Use lazy loading for large components
- Optimize images and static assets
- Consider server-side prerendering for better SEO

## Contributing

When contributing to this application:

1. Follow the existing code structure and patterns
2. Test changes locally before submitting
3. Update this README if adding new features
4. Ensure responsive design works on mobile devices
5. Maintain the dark theme and red accent color scheme

## Related Documentation

- [7DTD Server Scripts](../7dtd/Scripts/README.md) - Server management scripts
- [7DTD Server Setup](../7dtd/README.md) - Server installation and configuration
- [Azure Deployment](../Deployment/) - Infrastructure deployment templates

## License

This project is part of the 7DTD Azure VM repository. See the main repository LICENSE file for details.