using Set.Auth.Application.Interfaces;
using Set.Auth.Application.Services;
using Set.Auth.Infrastructure.Repositories;
using Set.Auth.Infrastructure.Services;
using Set.Auth.Domain.Interfaces;

namespace Set.Auth.Api.Extensions;

/// <summary>
/// Extension methods for registering authorization services
/// </summary>
public static class AuthorizationServiceExtensions
{
    /// <summary>
    /// Registers authorization policies only (services are registered in their respective layers)
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
    {
        // Authorization services are already registered in Application and Infrastructure layers
        // This method is kept for backward compatibility but doesn't register services
        return services;
    }

    /// <summary>
    /// Configures authorization policies for the application
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"))
            .AddPolicy("UserManagement", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || 
                    context.User.HasClaim("permission", "users:manage")))
            .AddPolicy("RoleManagement", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || 
                    context.User.HasClaim("permission", "roles:manage")))
            .AddPolicy("PermissionManagement", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || 
                    context.User.HasClaim("permission", "permissions:manage")))
            .AddPolicy("SelfManagement", policy =>
                policy.RequireAuthenticatedUser())
            .AddPolicy("SystemAdmin", policy =>
                policy.RequireRole("Admin")
                      .RequireClaim("permission", "system:admin"))
            .AddPolicy("DataRead", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || 
                    context.User.IsInRole("Manager") ||
                    context.User.HasClaim("permission", "data:read")))
            .AddPolicy("DataWrite", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || 
                    context.User.IsInRole("Manager") ||
                    context.User.HasClaim("permission", "data:write")))
            .AddPolicy("ReportAccess", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") || 
                    context.User.IsInRole("Manager") ||
                    context.User.HasClaim("permission", "reports:view")));

        return services;
    }
}

/// <summary>
/// Constants for common permission names
/// </summary>
public static class Permissions
{
    /// <summary>
    /// User management permissions
    /// </summary>
    public static class Users
    {
        public const string Read = "users:read";
        public const string Create = "users:create";
        public const string Update = "users:update";
        public const string Delete = "users:delete";
        public const string Manage = "users:manage";
        public const string Assign = "users:assign";
    }

    /// <summary>
    /// Role management permissions
    /// </summary>
    public static class Roles
    {
        public const string Read = "roles:read";
        public const string Create = "roles:create";
        public const string Update = "roles:update";
        public const string Delete = "roles:delete";
        public const string Manage = "roles:manage";
        public const string Assign = "roles:assign";
    }

    /// <summary>
    /// Permission management permissions
    /// </summary>
    public static class PermissionManagement
    {
        public const string Read = "permissions:read";
        public const string Create = "permissions:create";
        public const string Update = "permissions:update";
        public const string Delete = "permissions:delete";
        public const string Manage = "permissions:manage";
        public const string Assign = "permissions:assign";
    }

    /// <summary>
    /// Data access permissions
    /// </summary>
    public static class Data
    {
        public const string Read = "data:read";
        public const string Write = "data:write";
        public const string Delete = "data:delete";
        public const string Export = "data:export";
        public const string Import = "data:import";
    }

    /// <summary>
    /// System administration permissions
    /// </summary>
    public static class System
    {
        public const string Admin = "system:admin";
        public const string Config = "system:config";
        public const string Logs = "system:logs";
        public const string Backup = "system:backup";
        public const string Restore = "system:restore";
    }

    /// <summary>
    /// Report permissions
    /// </summary>
    public static class Reports
    {
        public const string View = "reports:view";
        public const string Create = "reports:create";
        public const string Export = "reports:export";
        public const string Schedule = "reports:schedule";
    }

    /// <summary>
    /// Audit permissions
    /// </summary>
    public static class Audit
    {
        public const string View = "audit:view";
        public const string Export = "audit:export";
        public const string Manage = "audit:manage";
    }
}

/// <summary>
/// Constants for common role names
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string User = "User";
    public const string Viewer = "Viewer";
    public const string Editor = "Editor";
    public const string Moderator = "Moderator";
    public const string Support = "Support";
}

/// <summary>
/// Constants for common resource names
/// </summary>
public static class Resources
{
    public const string Users = "users";
    public const string Roles = "roles";
    public const string Permissions = "permissions";
    public const string Data = "data";
    public const string System = "system";
    public const string Reports = "reports";
    public const string Audit = "audit";
    public const string Settings = "settings";
    public const string Files = "files";
    public const string Dashboard = "dashboard";
}

/// <summary>
/// Constants for common actions
/// </summary>
public static class Actions
{
    public const string Read = "read";
    public const string Create = "create";
    public const string Update = "update";
    public const string Delete = "delete";
    public const string Manage = "manage";
    public const string Assign = "assign";
    public const string View = "view";
    public const string Export = "export";
    public const string Import = "import";
    public const string Execute = "execute";
    public const string Configure = "configure";
    public const string Approve = "approve";
    public const string Reject = "reject";
}
