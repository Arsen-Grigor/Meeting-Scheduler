using MediatR;

namespace WebMeetingScheduler.Application.Authentication.LoginUser;
    
public sealed record LoginUserCommand(string Username, string Password) : IRequest<LoginUserDto>;