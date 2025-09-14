using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Set.Auth.Application.Interfaces;
using System.Security.Claims;

namespace Set.Auth.Api.Attributes;

/// <summary>
/// Authorization attribute that checks if a user has a specific permission
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly string _resource;
    private readonly string _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequirePermissionAttribute"/> class
    /// </summary>
    /// <param name="resource">The resource name</param>
    /// <param name="action">The action name</param>
    public RequirePermissionAttribute(string resource, string action)
    {
        _resource = resource;
        _action = action;
    }

    /// <summary>
    /// Performs authorization check
    /// </summary>
    /// <param name="context">Authorization filter context</param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Allow anonymous access if specified
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
        {
            return;
        }

        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get permission service
        var permissionService = context.HttpContext.RequestServices.GetService<IPermissionService>();
        if (permissionService == null)
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }

        try
        {
            // Check if user has the required permission
            var hasPermission = await permissionService.UserHasPermissionAsync(userId, _resource, _action);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
        catch
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }
    }
}

/// <summary>
/// Authorization attribute that checks if a user has any of the specified permissions
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireAnyPermissionAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly string[] _permissions;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireAnyPermissionAttribute"/> class
    /// </summary>
    /// <param name="permissions">The permission names</param>
    public RequireAnyPermissionAttribute(params string[] permissions)
    {
        _permissions = permissions;
    }

    /// <summary>
    /// Performs authorization check
    /// </summary>
    /// <param name="context">Authorization filter context</param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Allow anonymous access if specified
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
        {
            return;
        }

        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get permission service
        var permissionService = context.HttpContext.RequestServices.GetService<IPermissionService>();
        if (permissionService == null)
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }

        try
        {
            // Check if user has any of the required permissions
            var hasAnyPermission = await permissionService.UserHasAnyPermissionAsync(userId, _permissions);
            if (!hasAnyPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
        catch
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }
    }
}

/// <summary>
/// Authorization attribute that checks if a user has all of the specified permissions
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireAllPermissionsAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly string[] _permissions;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireAllPermissionsAttribute"/> class
    /// </summary>
    /// <param name="permissions">The permission names</param>
    public RequireAllPermissionsAttribute(params string[] permissions)
    {
        _permissions = permissions;
    }

    /// <summary>
    /// Performs authorization check
    /// </summary>
    /// <param name="context">Authorization filter context</param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Allow anonymous access if specified
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
        {
            return;
        }

        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get permission service
        var permissionService = context.HttpContext.RequestServices.GetService<IPermissionService>();
        if (permissionService == null)
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }

        try
        {
            // Check if user has all of the required permissions
            var hasAllPermissions = await permissionService.UserHasAllPermissionsAsync(userId, _permissions);
            if (!hasAllPermissions)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
        catch
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }
    }
}

/// <summary>
/// Authorization attribute that checks if a user has a specific role
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly string _roleName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequireRoleAttribute"/> class
    /// </summary>
    /// <param name="roleName">The role name</param>
    public RequireRoleAttribute(string roleName)
    {
        _roleName = roleName;
    }

    /// <summary>
    /// Performs authorization check
    /// </summary>
    /// <param name="context">Authorization filter context</param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Allow anonymous access if specified
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
        {
            return;
        }

        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get role service
        var roleService = context.HttpContext.RequestServices.GetService<IRoleService>();
        if (roleService == null)
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }

        try
        {
            // Check if user has the required role
            var userRoles = await roleService.GetRolesByUserIdAsync(userId);
            var hasRole = userRoles.Any(r => r.Name.Equals(_roleName, StringComparison.OrdinalIgnoreCase));
            
            if (!hasRole)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
        catch
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }
    }
}

/// <summary>
/// Authorization attribute that combines permission and role requirements
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequirePermissionAndRoleAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private readonly string _resource;
    private readonly string _action;
    private readonly string _roleName;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequirePermissionAndRoleAttribute"/> class
    /// </summary>
    /// <param name="resource">The resource name</param>
    /// <param name="action">The action name</param>
    /// <param name="roleName">The role name</param>
    public RequirePermissionAndRoleAttribute(string resource, string action, string roleName)
    {
        _resource = resource;
        _action = action;
        _roleName = roleName;
    }

    /// <summary>
    /// Performs authorization check
    /// </summary>
    /// <param name="context">Authorization filter context</param>
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Allow anonymous access if specified
        if (context.ActionDescriptor.EndpointMetadata.Any(em => em is AllowAnonymousAttribute))
        {
            return;
        }

        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get services
        var permissionService = context.HttpContext.RequestServices.GetService<IPermissionService>();
        var roleService = context.HttpContext.RequestServices.GetService<IRoleService>();
        
        if (permissionService == null || roleService == null)
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }

        try
        {
            // Check if user has the required permission and role
            var hasPermission = await permissionService.UserHasPermissionAsync(userId, _resource, _action);
            var userRoles = await roleService.GetRolesByUserIdAsync(userId);
            var hasRole = userRoles.Any(r => r.Name.Equals(_roleName, StringComparison.OrdinalIgnoreCase));
            
            if (!hasPermission || !hasRole)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
        catch
        {
            context.Result = new StatusCodeResult(500); // Internal server error
            return;
        }
    }
}
