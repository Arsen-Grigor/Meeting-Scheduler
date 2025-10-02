using MediatR;

namespace WebMeetingScheduler.Application.Authentication.RefreshUserToken
{
    public sealed class RefreshUserTokenCommandHandler : IRequestHandler<RefreshUserTokenCommand, RefreshUserTokenDto>
    {
        private readonly IAuthenticationService _keycloakAuthService;

        public RefreshUserTokenCommandHandler(IAuthenticationService keycloakAuthService)
        {
            _keycloakAuthService = keycloakAuthService;
        }

        public async Task<RefreshUserTokenDto> Handle(RefreshUserTokenCommand request, CancellationToken cancellationToken)
        {
            (string accessToken, string refreshToken) tokens = await _keycloakAuthService
                .RefreshAccessTokenAsync(request.RefreshToken);

            return new RefreshUserTokenDto(tokens.accessToken, tokens.refreshToken);
        }
    }
}
