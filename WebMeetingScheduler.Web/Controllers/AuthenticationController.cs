using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMeetingScheduler.Application.Authentication.LoginUser;
using WebMeetingScheduler.Application.Authentication.RefreshUserToken;

namespace WebMeetingScheduler.Web.Controllers;

[AllowAnonymous]
public class AuthenticationController : ApiControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(ILogger<AuthenticationController> logger)
    {
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginUserDto>> Login([FromBody] LoginUserCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            
            _logger.LogInformation("User logged in successfully: {Username}", command.Username);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed login attempt for username: {Username}", command.Username);
            throw;
        }
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(RefreshUserTokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RefreshUserTokenDto>> RefreshToken([FromBody] RefreshUserTokenCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            
            _logger.LogInformation("Access token refreshed successfully");
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to refresh token");
            throw;
        }
    }
}