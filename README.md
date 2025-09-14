# Set.Auth.Api

Comprehensive Authentication and Authorization API built with .NET 8 using Domain-Driven Design (DDD) architecture.

## Features

### 🔐 Authentication & Authorization
- **User Authentication**: Login/Register with email or Vietnamese phone number
- **Multi-device Support**: Login on multiple devices with device management
- **JWT Tokens**: Access token and refresh token implementation
- **Role-based Access Control (RBAC)**: Comprehensive permission-based authorization
- **Dynamic Authorization**: Custom authorization attributes for fine-grained access control
- **Permission Management**: Full CRUD operations for permissions with resource/action structure

### 👥 User Management (Admin)
- **User CRUD**: Complete user lifecycle management
- **User Filtering**: Advanced search and pagination
- **Bulk Operations**: Activate/deactivate multiple users
- **User Statistics**: Comprehensive user analytics
- **Role Assignment**: Assign/remove roles from users

### 🛡️ Role & Permission System
- **Role Management**: Full CRUD operations for roles
- **Permission Assignment**: Assign permissions to roles
- **Dynamic Permissions**: Runtime permission checking
- **Hierarchical Authorization**: Role-based with permission-level granularity
- **Permission Grouping**: Organize permissions by resource

### 🚀 Performance & Caching
- **Redis Caching**: High-performance caching for user data and sessions
- **MySQL Database**: Entity Framework Core with MySQL provider
- **Password Security**: Secure password hashing with PBKDF2
- **Device Management**: Logout from specific devices or all devices
- **Comprehensive Logging**: Structured logging with Serilog

## Architecture

The project follows Domain-Driven Design principles with clean architecture:

```
├── src/
│   ├── libs/
│   │   ├── Set.Auth.Domain/          # Domain layer (Entities, Value Objects, Interfaces)
│   │   ├── Set.Auth.Application/     # Application layer (Services, DTOs, Validators)
│   │   └── Set.Auth.Infrastructure/  # Infrastructure layer (Data, Repositories, External Services)
│   └── presentation/
│       └── Set.Auth.Api/             # Presentation layer (Controllers, Middleware, Authorization)
└── tests/                            # Unit and integration tests
```

## Technologies Used

- **.NET 8**: Latest .NET framework
- **Entity Framework Core**: ORM with MySQL provider (Pomelo)
- **MySQL**: Primary database
- **Redis**: Caching and session storage
- **JWT**: Authentication tokens
- **AutoMapper**: Object mapping
- **FluentValidation**: Input validation
- **Serilog**: Structured logging
- **Swagger/OpenAPI**: API documentation
- **Custom Authorization**: Dynamic permission-based access control

## Authorization System

### Dynamic Authorization Attributes
```csharp
// Permission-based authorization
[RequirePermission("users", "read")]
public async Task<ActionResult> GetUsers() { }

// Role-based authorization
[RequireRole("Admin")]
public async Task<ActionResult> AdminOnlyEndpoint() { }

// Multiple permission requirements
[RequireAnyPermission("users:read", "users:manage")]
public async Task<ActionResult> FlexibleAccess() { }

// Combined requirements
[RequirePermissionAndRole("system", "config", "Admin")]
public async Task<ActionResult> RestrictedAccess() { }
```

### Built-in Authorization Policies
- **AdminOnly**: Requires Admin role
- **UserManagement**: User administration permissions
- **RoleManagement**: Role administration permissions
- **PermissionManagement**: Permission administration permissions
- **DataRead/DataWrite**: Data access permissions

## Prerequisites

- .NET 8 SDK
- MySQL Server
- Redis Server
- Visual Studio 2022 or VS Code

## Configuration

Update `appsettings.json` with your database and Redis connection strings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SetAuth;User=root;Password=yourpassword;",
    "Redis": "localhost:6379"
  },
  "JWT": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast256BitsLongForHMACSecurityRequirements",
    "Issuer": "SetAuthAPI",
    "Audience": "SetAuthAPI",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 30
  }
}
```

## Database Setup

1. Install EF Core tools:
```bash
dotnet tool install --global dotnet-ef
```

2. Create and apply migrations:
```bash
dotnet ef migrations add InitialCreate --project src/libs/Set.Auth.Infrastructure --startup-project src/presentation/Set.Auth.Api
dotnet ef database update --project src/libs/Set.Auth.Infrastructure --startup-project src/presentation/Set.Auth.Api
```

## Running the Application

```bash
cd src/presentation/Set.Auth.Api
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7000`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:7000/swagger`

## API Endpoints

### 🔐 Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User login
- `POST /api/auth/refresh-token` - Refresh access token
- `POST /api/auth/logout` - Logout from current device
- `POST /api/auth/logout-all` - Logout from all devices
- `GET /api/auth/validate-token` - Validate token

### 👤 User Management
- `GET /api/user/profile` - Get current user profile
- `PUT /api/user/profile` - Update user profile
- `POST /api/user/change-password` - Change password

### 👥 User Administration (Admin Only)
- `GET /api/user` - Get all users with pagination/filtering
- `GET /api/user/{id}` - Get user by ID
- `POST /api/user` - Create new user
- `PUT /api/user/{id}` - Update user
- `DELETE /api/user/{id}` - Delete user
- `POST /api/user/{id}/activate` - Activate user
- `POST /api/user/{id}/deactivate` - Deactivate user
- `GET /api/user/statistics` - Get user statistics
- `POST /api/user/bulk` - Bulk user operations
- `GET /api/user/{id}/roles` - Get user roles
- `POST /api/user/{id}/roles` - Assign roles to user
- `DELETE /api/user/{id}/roles` - Remove roles from user

### 🛡️ Role Management (Admin Only)
- `GET /api/role` - Get all roles with pagination
- `GET /api/role/{id}` - Get role by ID
- `POST /api/role` - Create new role
- `PUT /api/role/{id}` - Update role
- `DELETE /api/role/{id}` - Delete role
- `POST /api/role/{id}/activate` - Activate role
- `POST /api/role/{id}/deactivate` - Deactivate role
- `POST /api/role/assign-permissions` - Assign permissions to role
- `POST /api/role/assign-user` - Assign roles to user
- `GET /api/role/user/{userId}` - Get user roles
- `POST /api/role/bulk` - Bulk role operations

### 🔑 Permission Management (Admin Only)
- `GET /api/permission` - Get all permissions with filtering
- `GET /api/permission/all` - Get all permissions (simple list)
- `GET /api/permission/grouped` - Get permissions grouped by resource
- `GET /api/permission/{id}` - Get permission by ID
- `POST /api/permission` - Create new permission
- `PUT /api/permission/{id}` - Update permission
- `DELETE /api/permission/{id}` - Delete permission
- `POST /api/permission/{id}/activate` - Activate permission
- `POST /api/permission/{id}/deactivate` - Deactivate permission
- `GET /api/permission/role/{roleId}` - Get role permissions
- `GET /api/permission/user/{userId}` - Get user permissions
- `POST /api/permission/assign` - Assign permissions to role
- `POST /api/permission/bulk` - Bulk permission operations
- `GET /api/permission/check/{userId}` - Check user permission
- `POST /api/permission/check/{userId}/any` - Check user has any permissions
- `POST /api/permission/check/{userId}/all` - Check user has all permissions

## Request/Response Examples

### Register
```json
POST /api/auth/register
{
  "email": "user@example.com",
  "phoneNumber": "+84901234567",
  "password": "SecurePass123!",
  "confirmPassword": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "deviceId": "device-unique-id",
  "deviceName": "iPhone 15"
}
```

### Login
```json
POST /api/auth/login
{
  "emailOrPhone": "user@example.com",
  "password": "SecurePass123!",
  "deviceId": "device-unique-id",
  "deviceName": "iPhone 15",
  "rememberMe": true
}
```

### Response
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "base64-encoded-refresh-token",
  "expiresAt": "2025-08-09T13:00:00Z",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roles": ["User"],
    "permissions": ["profile.read", "profile.write"]
  }
}
```

## Default Users, Roles & Permissions

The system creates default roles and permissions on startup:

### Default Roles
- **Admin**: Full system access with all permissions
- **Manager**: Management-level access for user and data operations
- **User**: Basic user permissions for profile management
- **Viewer**: Read-only access to data

### Default Permissions by Resource
- **Users**: `users:read`, `users:create`, `users:update`, `users:delete`, `users:manage`, `users:assign`
- **Roles**: `roles:read`, `roles:create`, `roles:update`, `roles:delete`, `roles:manage`, `roles:assign`
- **Permissions**: `permissions:read`, `permissions:create`, `permissions:update`, `permissions:delete`, `permissions:manage`, `permissions:assign`
- **Data**: `data:read`, `data:write`, `data:delete`, `data:export`, `data:import`
- **System**: `system:admin`, `system:config`, `system:logs`, `system:backup`, `system:restore`
- **Reports**: `reports:view`, `reports:create`, `reports:export`, `reports:schedule`
- **Audit**: `audit:view`, `audit:export`, `audit:manage`

### Permission Structure
Permissions follow the format: `resource:action`
- **Resource**: The entity or area being accessed (users, roles, permissions, data, etc.)
- **Action**: The operation being performed (read, create, update, delete, manage, etc.)

### Role-Permission Mapping
- **Admin**: All permissions
- **Manager**: User management, data operations, reports
- **User**: Profile management permissions
- **Viewer**: Read-only permissions

## Phone Number Validation

The system supports Vietnamese phone number formats:
- Mobile operators: Viettel, Vinaphone, Mobifone, Vietnamobile
- Formats: `+84xxxxxxxxx`, `84xxxxxxxxx`, `0xxxxxxxxx`
- Auto-normalization to international format

## Security Features

- **Password Requirements**: Minimum 8 characters with uppercase, lowercase, number, and special character
- **JWT Security**: RS256 algorithm with configurable expiration
- **Refresh Token Rotation**: New refresh token issued on each refresh
- **Device Tracking**: Track and manage user sessions by device
- **IP Logging**: Log IP addresses for security auditing

## Development

### Building
```bash
dotnet build
```

### Testing
```bash
dotnet test
```

### Code Quality
- Follow DDD principles
- Use dependency injection
- Implement proper exception handling
- Write comprehensive unit tests
- Follow SOLID principles

## License

This project is licensed under the MIT License.
