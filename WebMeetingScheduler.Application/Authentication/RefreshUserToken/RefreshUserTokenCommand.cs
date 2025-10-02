using MediatR;

namespace WebMeetingScheduler.Application.Authentication.RefreshUserToken;

public sealed record RefreshUserTokenCommand(string RefreshToken) : IRequest<RefreshUserTokenDto>;
