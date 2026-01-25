# OneUni Backend API

<p align="center">
  <img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 9.0"/>
  <img src="https://img.shields.io/badge/PostgreSQL-18.1-4169E1?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL"/>
  <img src="https://img.shields.io/badge/Azure-Deployed-0078D4?style=for-the-badge&logo=microsoft-azure&logoColor=white" alt="Azure"/>
  <img src="https://img.shields.io/badge/License-Proprietary-red?style=for-the-badge" alt="License"/>
</p>

**OneUni** is a comprehensive university admissions and student management platform. This repository contains the backend API built with ASP.NET Core 9.0, providing secure authentication, student profile management, university data, and application processing capabilities.

---

## Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Environment Configuration](#environment-configuration)
  - [Database Setup](#database-setup)
  - [Running the Application](#running-the-application)
- [API Reference](#-api-reference)
- [Project Structure](#-project-structure)
- [Security](#-security)
- [Deployment](#-deployment)
- [Infrastructure](#-infrastructure)

---

## Features

### Authentication & Authorization
- **JWT-based authentication** with HttpOnly cookie storage
- **Google OAuth 2.0** integration with two-step signup flow
- **Refresh token rotation** for enhanced security
- **CSRF protection** using double-submit cookie pattern
- **Role-based access control** (Student, Mentor, Admin, University Representative)

### Student Profile Management
- Comprehensive student profiles with educational records
- Document upload support via Cloudinary
- Test score tracking (SAT, GRE, IELTS, TOEFL, etc.)

### University & Program Data
- University listings with rankings and details
- Department and program information
- Admission cycle management
- Merit formula calculations

### Application Processing
- University application submission and tracking
- Application status management
- Scholarship information

### Additional Features
- Mentorship system with session scheduling
- Audit logging for compliance
- Notification system
- Health check endpoints

---

## Architecture

The application follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                      Controllers (API)                       │
│         AuthController, ProfileController, etc.              │
├─────────────────────────────────────────────────────────────┤
│                        Services                              │
│    AuthService, OneProfileService, TokenService, etc.        │
├─────────────────────────────────────────────────────────────┤
│                      Repositories                            │
│      UnitOfWork, GenericRepository, UserRepository           │
├─────────────────────────────────────────────────────────────┤
│                   Data Access (EF Core)                      │
│              OneUniDbContext, Entity Configurations          │
├─────────────────────────────────────────────────────────────┤
│                   PostgreSQL Database                        │
│                 Azure Database for PostgreSQL                │
└─────────────────────────────────────────────────────────────┘
```

### Design Patterns Used
- **Repository Pattern** with Unit of Work
- **Dependency Injection** throughout
- **Result Pattern** for operation outcomes
- **DTO Pattern** for API contracts
- **Middleware Pipeline** for cross-cutting concerns

---

## Tech Stack

| Category | Technology |
|----------|------------|
| **Framework** | ASP.NET Core 9.0 |
| **Language** | C# 13 |
| **Database** | PostgreSQL 18.1 |
| **ORM** | Entity Framework Core 9.0 |
| **Authentication** | JWT Bearer Tokens, Google OAuth 2.0 |
| **Password Hashing** | BCrypt |
| **File Storage** | Cloudinary |
| **API Documentation** | Swagger / OpenAPI |
| **Hosting** | Azure App Service |
| **Database Hosting** | Azure Database for PostgreSQL Flexible Server |
| **CI/CD** | GitHub Actions |

---

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/) (local) or Azure PostgreSQL
- [Git](https://git-scm.com/)
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/abdull-ah-med/OneUniBackend.git
   cd OneUniBackend
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

### Environment Configuration

Create a `.env` file in the `OneUniBackend` directory:

```env
# Database Configuration
DB_HOST=your-db-host.postgres.database.azure.com
DB_PORT=5432
DB_NAME=oneuni
DB_USER=your_db_user
DB_PASSWORD=your_secure_password

# JWT Configuration
JWT_SECRET=your-super-secret-jwt-key-min-32-characters
JWT_ISSUER=OneUni
JWT_EXPIRES_IN_MINUTES=60
REFRESH_TOKEN_EXPIRES_IN_DAYS=7

# Google OAuth Configuration
GOOGLE_CLIENT_ID=your-google-client-id.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=your-google-client-secret
GOOGLE_REDIRECT_URI=https://your-backend-url/api/google-oauth/callback

# Frontend Configuration
FRONTEND_URL=https://oneuniversity.net
FRONTEND_BASE_URL=https://oneuniversity.net

# CORS Configuration
ALLOWED_ORIGINS=https://oneuniversity.net,https://your-dev-frontend.vercel.app

# Cookie Configuration (Production)
COOKIE_DOMAIN=.oneuniversity.net
COOKIE_SAMESITE=None
COOKIE_SECURE=true

# Cookie Configuration (Local Development)
# COOKIE_DOMAIN=
# COOKIE_SAMESITE=Lax
# COOKIE_SECURE=false
```

#### Environment Variable Reference

| Variable | Required | Description | Default |
|----------|----------|-------------|---------|
| `DB_HOST` | ✅ | PostgreSQL server hostname | - |
| `DB_PORT` | ❌ | PostgreSQL server port | `5432` |
| `DB_NAME` | ✅ | Database name | - |
| `DB_USER` | ✅ | Database username | - |
| `DB_PASSWORD` | ✅ | Database password | - |
| `JWT_SECRET` | ✅ | JWT signing key (min 32 chars) | - |
| `JWT_ISSUER` | ❌ | JWT issuer claim | `OneUni` |
| `JWT_EXPIRES_IN_MINUTES` | ❌ | Access token lifetime | `60` |
| `REFRESH_TOKEN_EXPIRES_IN_DAYS` | ❌ | Refresh token lifetime | `7` |
| `GOOGLE_CLIENT_ID` | ✅ | Google OAuth client ID | - |
| `GOOGLE_CLIENT_SECRET` | ✅ | Google OAuth client secret | - |
| `GOOGLE_REDIRECT_URI` | ✅ | OAuth callback URL | - |
| `FRONTEND_URL` | ❌ | Frontend URL (optional) | - |
| `FRONTEND_BASE_URL` | ✅ | Frontend base URL for redirects | - |
| `ALLOWED_ORIGINS` | ✅ | Comma-separated CORS origins | - |
| `COOKIE_DOMAIN` | ❌ | Cookie domain (for subdomains) | - |
| `COOKIE_SAMESITE` | ❌ | SameSite attribute | `Lax` |
| `COOKIE_SECURE` | ❌ | Secure cookie flag | `auto` |

### Database Setup

1. **Create the database** (if not using Azure)
   ```bash
   createdb oneuni
   ```

2. **Run migrations**
   ```bash
   cd OneUniBackend
   dotnet ef database update
   ```

### Running the Application

```bash
cd OneUniBackend
dotnet run
```

The API will be available at:
- **HTTP**: `http://localhost:5162`
- **HTTPS**: `https://localhost:7162`

Verify the API is running:
```bash
curl http://localhost:5162/api/health
```

---

## 📖 API Reference

### Base URL
- **Development**: `http://localhost:5162/api`
- **Production**: `https://api.oneuniversity.net/api`

### Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/auth/register` | Register new user | ❌ |
| `POST` | `/auth/login` | User login | ❌ |
| `POST` | `/auth/refresh` | Refresh access token | ❌ |
| `POST` | `/auth/logout` | User logout | ✅ |
| `GET` | `/auth/me` | Get current user | ✅ |

### Google OAuth Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/google-oauth/callback` | OAuth callback handler | ❌ |
| `POST` | `/google-oauth/complete-signup` | Complete OAuth signup | ❌* |

*Requires temporary token from OAuth flow

### Profile Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `GET` | `/profile` | Get user profile | ✅ |
| `PUT` | `/profile` | Update user profile | ✅ |

### Health Check

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/health` | API health status |
| `GET` | `/api/health` | Detailed health check |

---

## 📁 Project Structure

```
OneUniBackend/
├── Common/                    # Shared utilities
│   ├── PagedResult.cs         # Pagination wrapper
│   └── Result.cs              # Operation result pattern
├── Configuration/             # App configuration classes
│   ├── CloudinarySettings.cs
│   └── JWTSettings.cs
├── Controllers/               # API Controllers
│   ├── Auth/
│   │   ├── AuthController.cs
│   │   └── GoogleOAuth.cs
│   └── ProfileController.cs
├── Data/                      # Database context
│   ├── Configurations/        # Entity configurations
│   └── OneUniDbContext.cs
├── DTOs/                      # Data Transfer Objects
│   ├── Application/
│   ├── Auth/
│   ├── Common/
│   ├── Mentor/
│   ├── Profile/
│   ├── University/
│   └── User/
├── Entities/                  # Domain entities
│   ├── User.cs
│   ├── StudentProfile.cs
│   ├── University.cs
│   ├── Application.cs
│   └── ...
├── Enums/                     # Enumeration types
│   ├── ApplicationStatus.cs
│   ├── UserRole.cs
│   └── ...
├── Extensions/                # Service extensions
│   ├── AuthExtensions.cs
│   ├── ConfigurationExtensions.cs
│   ├── CorsExtensions.cs
│   ├── DatabaseExtensions.cs
│   └── ...
├── ExternalIntegrations/      # External API controllers
│   ├── HealthController.cs
│   └── UniversitiesController.cs
├── Interfaces/                # Contracts
│   ├── Repositories/
│   └── Services/
├── Middleware/                # Custom middleware
│   ├── CorrelationIdMiddleware.cs
│   ├── CsrfProtectionMiddleware.cs
│   └── ExceptionHandlingMiddleware.cs
├── Migrations/                # EF Core migrations
├── Repositories/              # Data access layer
│   ├── GenericRepository.cs
│   ├── UnitOfWork.cs
│   └── ...
├── Services/                  # Business logic
│   ├── AuthService.cs
│   ├── CookieService.cs
│   ├── GoogleOAuthService.cs
│   ├── OneProfileService.cs
│   ├── PasswordService.cs
│   └── TokenService.cs
├── Utils/                     # Utility classes
│   └── ClaimsPrincipalExtensions.cs
├── Program.cs                 # Application entry point
├── appsettings.json           # Configuration
└── OneUniBackend.csproj       # Project file
```

---

## Security

### Authentication Flow

1. **Standard Login**: Email/password → JWT tokens set in HttpOnly cookies
2. **Google OAuth**: 
   - User initiates OAuth → Google callback → Temporary token issued
   - User selects role → Complete signup → Full JWT tokens issued

### Security Measures

- **HttpOnly Cookies**: Tokens stored in HttpOnly cookies to prevent XSS attacks
- **CSRF Protection**: Double-submit cookie pattern for state-changing requests
- **Password Hashing**: BCrypt with automatic salt generation
- **Token Rotation**: Refresh tokens are rotated on each use
- **CORS Configuration**: Strict origin validation
- **Input Validation**: Model validation on all endpoints
- **SQL Injection Prevention**: Parameterized queries via EF Core

### CSRF-Exempt Endpoints

The following endpoints are exempt from CSRF validation:
- `/api/auth/register`
- `/api/auth/login`
- `/api/auth/refresh`
- `/api/google-oauth/complete-signup`

---

## Deployment

### Azure App Service Deployment

The application is deployed to Azure App Service using GitHub Actions CI/CD.

#### Deployment Configuration

```yaml
# .github/workflows/main_oneuni-backend-30958.yml
name: Deploy to Azure

on:
  push:
    branches:
      - main

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      - run: dotnet build --configuration Release
      - run: dotnet publish -c Release -o ./publish
      - uses: azure/webapps-deploy@v3
        with:
          app-name: 'oneuni-backend-30958'
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ./publish
```

#### Environment Variables in Azure

Configure the following in Azure App Service → Configuration → Application settings:
- All variables from the [Environment Configuration](#environment-configuration) section

---

## Infrastructure

### Azure Resources

| Resource | Type | Region | Description |
|----------|------|--------|-------------|
| `oneuni-backend-30958` | App Service | North Europe | Backend API hosting |
| `oneuni-db-30958` | PostgreSQL Flexible Server | North Europe | Database server |
| `OneUni-Plan-v0` | App Service Plan | North Europe | Hosting plan (B1ms) |
| `oneuniversity.net` | App Service Domain | Global | Custom domain |
| `oneuniversity.net` | DNS Zone | Global | DNS management |
| `OneUniEmail` | Communication Service | Global | Email services |

### Database Configuration

- **Server**: `oneuni-db-30958.postgres.database.azure.com`
- **Version**: PostgreSQL 18.1
- **Tier**: Burstable B1ms (1 vCore, 2 GiB RAM)
- **Storage**: 128 GiB
- **High Availability**: Not enabled (dev/test)
- **Backup Retention**: 7 days

### Domain Configuration

- **Pre-Production API**: `https://api.oneuniversity.net`
- **Pre-Production Frontend**: `https://oneuniversity.net`

---

## Development Notes

### Running Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove
```

### Testing Endpoints

A Postman collection is available at:
```
OneUniBackend/docs/OneProfile-Postman-Collection.json
```

### Logging

Logs are output to console with the following levels:
- `Information`: General application flow
- `Warning`: Validation failures, failed auth attempts
- `Error`: Exceptions, system errors

---

## Contributing

This is a private repository. Contact the project maintainers for contribution guidelines.

---

## License

Proprietary - All rights reserved.

---
