using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Application.Meetings.Commands;
using WebMeetingScheduler.Application.Meetings.Queries;

namespace WebMeetingScheduler.Web.Controllers;

public record CreateMeetingRequest(string Title, string Description, int Duration, DateTime Start, DateTime End);
public record UpdateMeetingRequest(string? Title, string? Description);

#if !DEBUG
[Authorize]
#endif
public class MeetingsController : ApiControllerBase
{
    private readonly ILogger<MeetingsController> _logger;

    public MeetingsController(ILogger<MeetingsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MeetingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MeetingDto>?>> GetAllMeetings()
    {
        var query = new GetAllMeetingsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MeetingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MeetingDto?>> GetMeeting(Guid id)
    {
        var query = new GetMeetingQuery(id);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateMeeting([FromBody] CreateMeetingRequest request)
    {
        try
        {
            var command = new CreateMeetingCommand(
                request.Title,
                request.Description,
                request.Duration,
                request.Start,
                request.End);
            var meetingId = await Mediator.Send(command);
        
            _logger.LogInformation("Meeting created successfully: {MeetingId}", meetingId);
        
            return CreatedAtAction(
                nameof(GetMeeting),
                new { id = meetingId },
                new { id = meetingId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating meeting");
            throw;
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateMeeting(Guid id, [FromBody] UpdateMeetingRequest request)
    {
        var command = new UpdateMeetingCommand(id, request.Title, request.Description);
        await Mediator.Send(command);
        
        _logger.LogInformation("Meeting updated successfully: {MeetingId}", id);
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteMeeting(Guid id)
    {
        var command = new DeleteMeetingCommand(id);
        await Mediator.Send(command);
        
        _logger.LogInformation("Meeting deleted successfully: {MeetingId}", id);
        
        return NoContent();
    }

    [HttpPost("{meetingId:guid}/participants/{participantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddParticipant(Guid meetingId, Guid participantId)
    {
        var command = new AddParticipantCommand(participantId, meetingId);
        await Mediator.Send(command);
        
        _logger.LogInformation(
            "Participant {ParticipantId} added to meeting {MeetingId}", 
            participantId, 
            meetingId);
        
        return NoContent();
    }

    [HttpDelete("{meetingId:guid}/participants/{participantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveParticipant(Guid meetingId, Guid participantId)
    {
        var command = new RemoveParticipantCommand(participantId, meetingId);
        await Mediator.Send(command);
        
        _logger.LogInformation(
            "Participant {ParticipantId} removed from meeting {MeetingId}", 
            participantId, 
            meetingId);
        
        return NoContent();
    }
}