using AutoMapper;
using FluentValidation;
using Set.Auth.Application.DTOs.Auth;
using Set.Auth.Application.Exceptions;
using Set.Auth.Application.Interfaces;
using Set.Auth.Domain.Entities;
using Set.Auth.Domain.Interfaces;
using Set.Auth.Domain.ValueObjects;

namespace Set.Auth.Application.Services;

/// <inheritdoc/>
/// <summary>
/// AuthService constructor to initialize dependencies
/// </summary>
/// <param name="userRepository"></param>
/// <param name="roleRepository"></param>
/// <param name="refreshTokenRepository"></param>
/// <param name="cacheService"></param>
/// <param name="passwordService"></param>
/// <param name="tokenService"></param>
/// <param name="mapper"></param>
/// <param name="loginValidator"></param>
/// <param name="registerValidator"></param>
public class AuthService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ICacheService cacheService,
    IPasswordService passwordService,
    ITokenService tokenService,
    IMapper mapper,
    IValidator<LoginRequestDto> loginValidator,
    IValidator<RegisterRequestDto> registerValidator) : IAuthService
{
    /// <inheritdoc/>
    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string? ipAddress = null, string? userAgent = null)
    {
        var validationResult = await loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new Exceptions.ValidationException(validationResult.Errors.GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()));

        var user = await userRepository.GetByEmailOrPhoneAsync(request.EmailOrPhone);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedException("Invalid credentials");
        }

        if (!passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid credentials");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);

        // Revoke existing tokens for this device if not remember me
        if (!request.RememberMe)
        {
            await refreshTokenRepository.RevokeByDeviceIdAsync(user.Id, request.DeviceId, ipAddress);
        }

        // Generate tokens
        var roles = user.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles.Where(ur => ur.IsActive)
            .SelectMany(ur => ur.Role.RolePermissions.Where(rp => rp.IsActive))
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        var accessToken = tokenService.GenerateAccessToken(user.Id, user.Email, roles, permissions);
        var refreshToken = tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            DeviceId = request.DeviceId,
            DeviceName = request.DeviceName,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(30) // 30 days
        };

        await refreshTokenRepository.CreateAsync(refreshTokenEntity);

        // Cache user data
        await cacheService.SetAsync($"user:{user.Id}", mapper.Map<UserDto>(user), TimeSpan.FromMinutes(15));

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = tokenService.GetTokenExpiration(accessToken),
            User = mapper.Map<UserDto>(user)
        };
    }

    /// <inheritdoc/>
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, string? ipAddress = null, string? userAgent = null)
    {
        var validationResult = await registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new Exceptions.ValidationException(validationResult.Errors.GroupBy(x => x.PropertyName).ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()));
        }

        // Check if email already exists
        if (await userRepository.EmailExistsAsync(request.Email))
        {
            throw new ConflictException("Email already exists");
        }

        // Check if phone number already exists
        if (!string.IsNullOrEmpty(request.PhoneNumber) && await userRepository.PhoneNumberExistsAsync(request.PhoneNumber))
        {
            throw new ConflictException("Phone number already exists");
        }

        // Validate email
        var email = Email.Create(request.Email);

        // Validate phone number if provided
        string? normalizedPhone = null;
        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            var phone = PhoneNumber.Create(request.PhoneNumber);
            normalizedPhone = phone.Value;
        }

        // Create user
        var user = mapper.Map<User>(request);
        user.Email = email.Value;
        user.PhoneNumber = normalizedPhone;
        user.PasswordHash = passwordService.HashPassword(request.Password);

        user = await userRepository.CreateAsync(user);

        // Assign default user role
        var defaultRole = await roleRepository.GetByNameAsync("User");
        if (defaultRole != null)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = defaultRole.Id,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            };

            user.UserRoles.Add(userRole);
            await userRepository.UpdateAsync(user);
        }

        // Generate tokens
        var roles = new List<string> { "User" };
        var permissions = defaultRole?.RolePermissions.Where(rp => rp.IsActive).Select(rp => rp.Permission.Name).ToList() ?? [];
        var accessToken = tokenService.GenerateAccessToken(user.Id, user.Email, roles, permissions);
        var refreshToken = tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            DeviceId = request.DeviceId,
            DeviceName = request.DeviceName,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await refreshTokenRepository.CreateAsync(refreshTokenEntity);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = tokenService.GetTokenExpiration(accessToken),
            User = mapper.Map<UserDto>(user)
        };
    }

    /// <inheritdoc/>
    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, string? ipAddress = null)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (refreshToken == null || !refreshToken.IsActive || refreshToken.DeviceId != request.DeviceId)
            throw new UnauthorizedException("Invalid refresh token");

        var user = refreshToken.User;
        if (!user.IsActive)
            throw new UnauthorizedException("User account is deactivated");

        // Generate new tokens
        var roles = user.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name).ToList();
        var permissions = user.UserRoles.Where(ur => ur.IsActive)
            .SelectMany(ur => ur.Role.RolePermissions.Where(rp => rp.IsActive))
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        var newAccessToken = tokenService.GenerateAccessToken(user.Id, user.Email, roles, permissions);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        // Revoke old token and create new one
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReplacedByToken = newRefreshToken;
        await refreshTokenRepository.UpdateAsync(refreshToken);

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            DeviceId = request.DeviceId,
            DeviceName = refreshToken.DeviceName,
            UserAgent = refreshToken.UserAgent,
            IpAddress = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        await refreshTokenRepository.CreateAsync(newRefreshTokenEntity);

        return new RefreshTokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = tokenService.GetTokenExpiration(newAccessToken)
        };
    }

    /// <inheritdoc/>
    public async Task LogoutAsync(LogoutRequestDto request, string? ipAddress = null)
    {
        await refreshTokenRepository.RevokeAsync(request.RefreshToken, ipAddress);
    }

    /// <inheritdoc/>
    public async Task LogoutAllDevicesAsync(LogoutAllRequestDto request, string? ipAddress = null)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (refreshToken != null)
        {
            await refreshTokenRepository.RevokeAllByUserIdAsync(refreshToken.UserId, ipAddress);
            await cacheService.RemoveAsync($"user:{refreshToken.UserId}");
        }
    }

    /// <inheritdoc/>
    public Task<bool> ValidateTokenAsync(string token)
    {
        return Task.FromResult(tokenService.ValidateToken(token));
    }
}
