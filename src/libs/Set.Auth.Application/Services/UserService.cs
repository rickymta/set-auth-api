using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Set.Auth.Application.DTOs.Auth;
using Set.Auth.Application.DTOs.User;
using Set.Auth.Application.Exceptions;
using Set.Auth.Application.Interfaces;
using Set.Auth.Domain.Entities;
using Set.Auth.Domain.Interfaces;
using Set.Auth.Domain.ValueObjects;

namespace Set.Auth.Application.Services;

/// <inheritdoc/>
/// <summary>
/// UserService constructor to initialize dependencies
/// </summary>
/// <param name="userRepository"></param>
/// <param name="roleRepository"></param>
/// <param name="permissionRepository"></param>
/// <param name="cacheService"></param>
/// <param name="passwordService"></param>
/// <param name="mapper"></param>
/// <param name="updateUserValidator"></param>
/// <param name="changePasswordValidator"></param>
public class UserService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    ICacheService cacheService,
    IPasswordService passwordService,
    IStorageService storageService,
    IMapper mapper,
    IValidator<UpdateUserRequestDto> updateUserValidator,
    IValidator<ChangePasswordRequestDto> changePasswordValidator) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IPermissionRepository _permissionRepository = permissionRepository;
    private readonly ICacheService _cacheService = cacheService;
    private readonly IPasswordService _passwordService = passwordService;
    private readonly IStorageService _storageService = storageService;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<UpdateUserRequestDto> _updateUserValidator = updateUserValidator;
    private readonly IValidator<ChangePasswordRequestDto> _changePasswordValidator = changePasswordValidator;

    /// <inheritdoc/>
    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        // Try cache first
        var cachedUser = await _cacheService.GetAsync<UserDto>($"user:{userId}");
        if (cachedUser != null)
            return cachedUser;

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return null;

        var userDto = _mapper.Map<UserDto>(user);
        
        // Cache for 15 minutes
        await _cacheService.SetAsync($"user:{userId}", userDto, TimeSpan.FromMinutes(15));
        
        return userDto;
    }

    /// <inheritdoc/>
    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
    {
        return await GetUserByIdAsync(userId);
    }

    /// <inheritdoc/>
    public async Task<UpdateUserResponseDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request)
    {
        var validationResult = await _updateUserValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new Exceptions.ValidationException(validationResult.Errors.GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()));

        var user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User", userId);

        // Validate phone number if provided
        string? normalizedPhone = null;
        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            var phone = PhoneNumber.Create(request.PhoneNumber);
            normalizedPhone = phone.Value;

            // Check if phone number already exists for another user
            var existingUser = await _userRepository.GetByPhoneNumberAsync(normalizedPhone);
            if (existingUser != null && existingUser.Id != userId)
                throw new ConflictException("Phone number already exists");
        }

        // Update user properties
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = normalizedPhone;
        user.Avatar = request.Avatar;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        // Clear cache
        await _cacheService.RemoveAsync($"user:{userId}");

        return _mapper.Map<UpdateUserResponseDto>(user);
    }

    /// <inheritdoc/>
    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request)
    {
        var validationResult = await _changePasswordValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new Exceptions.ValidationException(validationResult.Errors.GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()));

        var user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User", userId);

        // Verify current password
        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedException("Current password is incorrect");

        // Hash new password
        user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeactivateUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User", userId);
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        // Clear cache
        await _cacheService.RemoveAsync($"user:{userId}");

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ActivateUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException("User", userId);
        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        // Clear cache
        await _cacheService.RemoveAsync($"user:{userId}");

        return true;
    }

    /// <inheritdoc/>
    public async Task<UserListDto> GetUsersPagedAsync(UserFilterDto filter)
    {
        // For now, implement basic filtering and pagination
        // In a real implementation, this would be done at the repository level
        var allUsers = await _userRepository.GetAllAsync();
        
        // Apply filters
        var filteredUsers = allUsers.AsQueryable();
        
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            filteredUsers = filteredUsers.Where(u => 
                u.FirstName.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.LastName.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }
        
        if (filter.IsActive.HasValue)
        {
            filteredUsers = filteredUsers.Where(u => u.IsActive == filter.IsActive.Value);
        }
        
        if (filter.IsEmailVerified.HasValue)
        {
            filteredUsers = filteredUsers.Where(u => u.IsEmailVerified == filter.IsEmailVerified.Value);
        }
        
        if (filter.IsPhoneVerified.HasValue)
        {
            filteredUsers = filteredUsers.Where(u => u.IsPhoneVerified == filter.IsPhoneVerified.Value);
        }
        
        if (filter.CreatedFrom.HasValue)
        {
            filteredUsers = filteredUsers.Where(u => u.CreatedAt >= filter.CreatedFrom.Value);
        }
        
        if (filter.CreatedTo.HasValue)
        {
            filteredUsers = filteredUsers.Where(u => u.CreatedAt <= filter.CreatedTo.Value);
        }
        
        if (!string.IsNullOrEmpty(filter.Role))
        {
            filteredUsers = filteredUsers.Where(u => 
                u.UserRoles.Any(ur => ur.Role.Name.Equals(filter.Role, StringComparison.OrdinalIgnoreCase) && ur.IsActive));
        }
        
        // Apply sorting
        filteredUsers = filter.SortBy.ToLower() switch
        {
            "firstname" => filter.SortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase) 
                ? filteredUsers.OrderByDescending(u => u.FirstName)
                : filteredUsers.OrderBy(u => u.FirstName),
            "lastname" => filter.SortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                ? filteredUsers.OrderByDescending(u => u.LastName)
                : filteredUsers.OrderBy(u => u.LastName),
            "email" => filter.SortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                ? filteredUsers.OrderByDescending(u => u.Email)
                : filteredUsers.OrderBy(u => u.Email),
            "createdat" => filter.SortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                ? filteredUsers.OrderByDescending(u => u.CreatedAt)
                : filteredUsers.OrderBy(u => u.CreatedAt),
            "lastloginat" => filter.SortDirection.Equals("desc", StringComparison.CurrentCultureIgnoreCase)
                ? filteredUsers.OrderByDescending(u => u.LastLoginAt)
                : filteredUsers.OrderBy(u => u.LastLoginAt),
            _ => filteredUsers.OrderByDescending(u => u.CreatedAt)
        };
        
        var totalCount = filteredUsers.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize);
        
        var users = filteredUsers
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        var userListItems = new List<UserListItemDto>();
        foreach (var user in users)
        {
            var userItem = _mapper.Map<UserListItemDto>(user);
            userItem.Roles = [.. user.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name)];
            userListItems.Add(userItem);
        }

        return new UserListDto
        {
            Users = userListItems,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalPages = totalPages
        };
    }

    /// <inheritdoc/>
    public async Task<UserDto> CreateUserAsync(UserCreateDto createDto)
    {
        // Validate email doesn't exist
        if (await _userRepository.EmailExistsAsync(createDto.Email))
        {
            throw new ConflictException($"Email '{createDto.Email}' is already in use");
        }

        // Validate phone number if provided
        string? normalizedPhone = null;
        if (!string.IsNullOrEmpty(createDto.PhoneNumber))
        {
            var phone = PhoneNumber.Create(createDto.PhoneNumber);
            normalizedPhone = phone.Value;

            if (await _userRepository.PhoneNumberExistsAsync(normalizedPhone))
            {
                throw new ConflictException($"Phone number '{createDto.PhoneNumber}' is already in use");
            }
        }

        // Validate role IDs exist
        foreach (var roleId in createDto.RoleIds)
        {
            if (!await _roleRepository.ExistsAsync(roleId))
            {
                throw new Exceptions.ValidationException($"Role with ID '{roleId}' not found");
            }
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = createDto.Email,
            PhoneNumber = normalizedPhone,
            PasswordHash = _passwordService.HashPassword(createDto.Password),
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            Avatar = createDto.Avatar,
            IsEmailVerified = createDto.IsEmailVerified,
            IsPhoneVerified = createDto.IsPhoneVerified,
            IsActive = createDto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Add roles
        foreach (var roleId in createDto.RoleIds)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            });
        }

        var createdUser = await _userRepository.CreateAsync(user);
        return await GetUserByIdAsync(createdUser.Id) ?? throw new InvalidOperationException("Failed to create user");
    }

    /// <inheritdoc/>
    public async Task<UserDto> UpdateUserAdminAsync(Guid userId, UserAdminUpdateDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException($"User with ID '{userId}' not found");

        // Validate email doesn't exist (excluding current user)
        var existingUser = await _userRepository.GetByEmailAsync(updateDto.Email);
        if (existingUser != null && existingUser.Id != userId)
        {
            throw new ConflictException($"Email '{updateDto.Email}' is already in use");
        }

        // Validate phone number if provided
        string? normalizedPhone = null;
        if (!string.IsNullOrEmpty(updateDto.PhoneNumber))
        {
            var phone = PhoneNumber.Create(updateDto.PhoneNumber);
            normalizedPhone = phone.Value;

            var existingPhoneUser = await _userRepository.GetByPhoneNumberAsync(normalizedPhone);
            if (existingPhoneUser != null && existingPhoneUser.Id != userId)
            {
                throw new ConflictException($"Phone number '{updateDto.PhoneNumber}' is already in use");
            }
        }

        // Validate role IDs exist
        foreach (var roleId in updateDto.RoleIds)
        {
            if (!await _roleRepository.ExistsAsync(roleId))
            {
                throw new Exceptions.ValidationException($"Role with ID '{roleId}' not found");
            }
        }

        // Update user properties
        user.Email = updateDto.Email;
        user.PhoneNumber = normalizedPhone;
        user.FirstName = updateDto.FirstName;
        user.LastName = updateDto.LastName;
        user.Avatar = updateDto.Avatar;
        user.IsEmailVerified = updateDto.IsEmailVerified;
        user.IsPhoneVerified = updateDto.IsPhoneVerified;
        user.IsActive = updateDto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        // Update user roles
        var currentRoleIds = user.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.RoleId).ToList();
        
        // Deactivate removed roles
        var rolesToRemove = currentRoleIds.Except(updateDto.RoleIds);
        foreach (var roleId in rolesToRemove)
        {
            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId && ur.IsActive);
            if (userRole != null)
            {
                userRole.IsActive = false;
            }
        }

        // Add new roles
        var rolesToAdd = updateDto.RoleIds.Except(currentRoleIds);
        foreach (var roleId in rolesToAdd)
        {
            user.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            });
        }

        await _userRepository.UpdateAsync(user);

        // Clear cache
        await _cacheService.RemoveAsync($"user:{userId}");

        return await GetUserByIdAsync(userId) ?? throw new InvalidOperationException("Failed to update user");
    }

    /// <inheritdoc/>
    public async Task DeleteUserAsync(Guid userId)
    {
        _ = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException($"User with ID '{userId}' not found");
        await _userRepository.DeleteAsync(userId);

        // Clear cache
        await _cacheService.RemoveAsync($"user:{userId}");
    }

    /// <inheritdoc/>
    public async Task BulkUserOperationAsync(BulkUserDto bulkDto)
    {
        switch (bulkDto.Operation.ToLower())
        {
            case "activate":
                foreach (var userId in bulkDto.UserIds)
                {
                    await ActivateUserAsync(userId);
                }
                break;
            case "deactivate":
                foreach (var userId in bulkDto.UserIds)
                {
                    await DeactivateUserAsync(userId);
                }
                break;
            case "delete":
                foreach (var userId in bulkDto.UserIds)
                {
                    await DeleteUserAsync(userId);
                }
                break;
            default:
                throw new Exceptions.ValidationException($"Invalid operation '{bulkDto.Operation}'. Supported operations: activate, deactivate, delete");
        }
    }

    /// <inheritdoc/>
    public async Task<UserStatsDto> GetUserStatsAsync()
    {
        var allUsers = await _userRepository.GetAllAsync();
        var today = DateTime.UtcNow.Date;
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        return new UserStatsDto
        {
            TotalUsers = allUsers.Count(),
            ActiveUsers = allUsers.Count(u => u.IsActive),
            InactiveUsers = allUsers.Count(u => !u.IsActive),
            EmailVerifiedUsers = allUsers.Count(u => u.IsEmailVerified),
            PhoneVerifiedUsers = allUsers.Count(u => u.IsPhoneVerified),
            NewUsersLast30Days = allUsers.Count(u => u.CreatedAt >= thirtyDaysAgo),
            ActiveUsersToday = allUsers.Count(u => u.LastLoginAt?.Date == today)
        };
    }

    /// <inheritdoc/>
    public async Task<bool> UserExistsAsync(Guid userId)
    {
        return await _userRepository.ExistsAsync(userId);
    }

    /// <inheritdoc/>
    public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        return existingUser != null && (excludeUserId == null || existingUser.Id != excludeUserId);
    }

    /// <inheritdoc/>
    public async Task<bool> PhoneNumberExistsAsync(string phoneNumber, Guid? excludeUserId = null)
    {
        var phone = PhoneNumber.Create(phoneNumber);
        var existingUser = await _userRepository.GetByPhoneNumberAsync(phone.Value);
        return existingUser != null && (excludeUserId == null || existingUser.Id != excludeUserId);
    }

    /// <inheritdoc/>
    public async Task<bool> CanPerformUserOperationAsync(Guid userId, string operation)
    {
        // Get user permissions
        var userPermissions = await _permissionRepository.GetByUserIdAsync(userId);

        // Define required permissions for each operation
        var requiredPermissions = operation.ToLower() switch
        {
            "create" => ["users.create", "users.manage"],
            "read" => ["users.read", "users.manage"],
            "update" => ["users.update", "users.manage"],
            "delete" => ["users.delete", "users.manage"],
            "bulk" => ["users.bulk", "users.manage"],
            _ => new[] { "users.manage" }
        };

        return userPermissions.Any(p => requiredPermissions.Contains(p.Name) && p.IsActive);
    }

    /// <inheritdoc/>
    public async Task<string> UploadAvatarAsync(Guid userId, IFormFile avatarFile)
    {
        if (avatarFile == null || avatarFile.Length == 0)
            throw new Set.Auth.Application.Exceptions.ValidationException("Avatar file is required.");

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
        
        if (!allowedExtensions.Any(ext => ext == fileExtension))
            throw new Set.Auth.Application.Exceptions.ValidationException("Only JPG, JPEG, PNG, and GIF files are allowed.");

        // Validate file size (max 5MB)
        const int maxFileSize = 5 * 1024 * 1024; // 5MB
        if (avatarFile.Length > maxFileSize)
            throw new Set.Auth.Application.Exceptions.ValidationException("File size cannot exceed 5MB.");

        // Check if user exists
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException($"User with ID {userId} not found.");

        // Delete old avatar if exists
        if (!string.IsNullOrEmpty(user.Avatar))
        {
            try
            {
                await _storageService.DeleteImageAsync(user.Avatar);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the upload
                Console.WriteLine($"Failed to delete old avatar: {ex.Message}");
            }
        }

        // Upload new avatar
        var avatarUrl = await _storageService.UploadImageAsync(avatarFile, "avatars");

        // Update user's avatar URL
        user.Avatar = avatarUrl;
        await _userRepository.UpdateAsync(user);

        // Clear cache
        await _cacheService.RemoveAsync($"user:{userId}");

        return avatarUrl;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAvatarAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException($"User with ID {userId} not found.");

        if (string.IsNullOrEmpty(user.Avatar))
            return false; // No avatar to delete

        try
        {
            // Delete from storage
            await _storageService.DeleteImageAsync(user.Avatar);

            // Update user record
            user.Avatar = null;
            await _userRepository.UpdateAsync(user);

            // Clear cache
            await _cacheService.RemoveAsync($"user:{userId}");

            return true;
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Failed to delete avatar: {ex.Message}");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<Stream> GetAvatarAsync(string filename)
    {
        return await _storageService.GetImageAsync(filename);
    }
}
