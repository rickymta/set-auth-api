using FluentValidation;
using Set.Auth.Application.DTOs.User;

namespace Set.Auth.Application.Validators;

/// <summary>
/// Update user request validator
/// </summary>
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequestDto>
{
    /// <summary>
    /// Constructor to define validation rules
    /// </summary>
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .Must(BeValidVietnamesePhoneNumber)
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Invalid Vietnamese phone number format");
    }

    /// <summary>
    /// Be valid Vietnamese phone number
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    private bool BeValidVietnamesePhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber)) return true;
        
        try
        {
            Set.Auth.Domain.ValueObjects.PhoneNumber.Create(phoneNumber);
            return true;
        }
        catch
        {
            return false;
        }
    }
}


