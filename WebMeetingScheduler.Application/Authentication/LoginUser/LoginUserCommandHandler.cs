using MediatR;

namespace WebMeetingScheduler.Application.Authentication.LoginUser;

public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserDto>
{
    private readonly IAuthenticationService _keycloakAuthService;

    public LoginUserCommandHandler(IAuthenticationService keycloakAuthService)
    {
        _keycloakAuthService = keycloakAuthService;
    }

    public async Task<LoginUserDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        (string accessToken, string refreshToken) tokens = await _keycloakAuthService
            .RequestTokenUsingPasswordGrantAsync(request.Username, request.Password);

        return new LoginUserDto(tokens.accessToken, tokens.refreshToken);
    }
}
