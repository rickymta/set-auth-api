using AutoMapper;
using Set.Auth.Application.DTOs.Auth;
using Set.Auth.Application.DTOs.User;
using Set.Auth.Application.DTOs.Role;
using Set.Auth.Application.DTOs.Permission;
using Set.Auth.Domain.Entities;

namespace Set.Auth.Application.Mappings;

/// <summary>
/// AutoMapper profile for authentication-related mappings between entities and DTOs
/// </summary>
public class AuthMappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthMappingProfile"/> class and configures the mappings
    /// </summary>
    public AuthMappingProfile()
    {
        // Map User entity to UserDto with roles and permissions
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => 
                src.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name)))
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src =>
                src.UserRoles.Where(ur => ur.IsActive)
                    .SelectMany(ur => ur.Role.RolePermissions.Where(rp => rp.IsActive))
                    .Select(rp => rp.Permission.Name)
                    .Distinct()));

        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsPhoneVerified, opt => opt.MapFrom(src => false));

        CreateMap<User, UpdateUserResponseDto>();

        CreateMap<UpdateUserRequestDto, User>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.Avatar))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // User Management DTOs
        CreateMap<User, UserListItemDto>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => 
                src.UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role.Name)));

        CreateMap<UserCreateDto, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.IsPhoneVerified, opt => opt.MapFrom(src => false));

        CreateMap<User, UserCreateDto>().ReverseMap();
        CreateMap<User, UserAdminUpdateDto>().ReverseMap();

        // Role DTOs
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => 
                src.RolePermissions.Where(rp => rp.IsActive).Select(rp => rp.Permission.Name)))
            .ForMember(dest => dest.UserCount, opt => opt.MapFrom(src => 
                src.UserRoles.Count(ur => ur.IsActive)));

        CreateMap<RoleCreateDto, Role>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<Role, RoleCreateDto>().ReverseMap();
        CreateMap<Role, RoleUpdateDto>().ReverseMap();

        // Permission DTOs
        CreateMap<Permission, PermissionDto>()
            .ForMember(dest => dest.RoleCount, opt => opt.MapFrom(src => 
                src.RolePermissions.Count(rp => rp.IsActive)));

        CreateMap<PermissionCreateDto, Permission>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<Permission, PermissionCreateDto>().ReverseMap();
        CreateMap<Permission, PermissionUpdateDto>().ReverseMap();
    }
}
