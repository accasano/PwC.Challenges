using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace PwC.CarRental.Api.Auth.Commands;

public class RegisterUserCommandHandler(UserManager<IdentityUser> userManager) : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        return new RegisterUserResult(
            result.Succeeded,
            result.Errors.Select(e => e.Description)
        );
    }
}

public record RegisterUserCommand(string Email, string Password) : IRequest<RegisterUserResult>;

public record RegisterUserResult(bool Succeeded, IEnumerable<string> Errors);

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}