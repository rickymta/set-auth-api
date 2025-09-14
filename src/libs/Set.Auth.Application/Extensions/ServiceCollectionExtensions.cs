using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Set.Auth.Application.Interfaces;
using Set.Auth.Application.Mappings;
using Set.Auth.Application.Services;
using Set.Auth.Application.DTOs.Auth;
using Set.Auth.Application.DTOs.User;
using Set.Auth.Application.Validators;

namespace Set.Auth.Application.Extensions;

/// <summary>
/// Extension methods for configuring application services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application layer services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(AuthMappingProfile));

        // FluentValidation - only register validators that exist
        services.AddScoped<IValidator<RegisterRequestDto>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
        services.AddScoped<IValidator<UpdateUserRequestDto>, UpdateUserRequestValidator>();
        services.AddScoped<IValidator<ChangePasswordRequestDto>, ChangePasswordRequestValidator>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();

        return services;
    }
}
