
## Prerequisites

- .NET 9.0 SDK
- PostgreSQL 15+ installed and running
- Visual Studio Code or Visual Studio 2022+ (optional)

## Required Packages

The project uses the following NuGet packages:

```xml
<PackageReference Include="EFCore.NamingConventions" Version="9.0.0" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.7" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.9" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.4" />
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package EFCore.NamingConventions
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## Getting Started

### Installing Dependencies

1. Install the .NET 9.0 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
2. Install PostgreSQL from [postgresql.org](https://www.postgresql.org/download/)
3. Install the required packages:
```bash
dotnet restore
```

### Database Setup

1. Start PostgreSQL service:
```bash
# macOS
brew services start postgresql

# Linux
sudo service postgresql start

# Windows
net start postgresql
```

2. Create a new database:
```bash
psql -U postgres
CREATE DATABASE oneuni;
```

3. Apply migrations (if any):
```bash
dotnet ef database update
```

### Configuration

1. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=oneuni;Username=your_username;Password=your_password"
  }
}
```

2. For development, you can use `appsettings.Development.json` to override settings.

### Running the Application

1. Clone the repository:
```bash
git clone [your-repository-url]
cd OneUni/v1/OneUniBackend
```

2. Set up environment:
```bash
# Development environment
cp appsettings.json appsettings.Development.json
# Edit appsettings.Development.json with your local settings
```

3. Build the project:
```bash
dotnet build
```

4. Run the application:
```bash
# Development mode with hot reload
dotnet watch run

# Standard run
dotnet run
```

5. Verify the application is running:
- API will be available at:
  - HTTPS: `https://localhost:5001`
  - HTTP: `http://localhost:5000`
- Swagger documentation: `https://localhost:5001/swagger`

### Development Tools

1. Install Entity Framework Core tools:
```bash
dotnet tool install --global dotnet-ef
```

## Project Structure

- `Controllers/` - API endpoint controllers
- `Models/` - Database entities and data models
- `Services/` - Business logic implementation
- `Interfaces/` - Service and repository interfaces
- `ExternalIntegrations/` - External service integrations
- `Middleware/` - Custom middleware components
- `Utils/` - Utility classes and helper functions

