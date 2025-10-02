using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Application.Participants.Commands;
using WebMeetingScheduler.Application.Participants.Queries;

namespace WebMeetingScheduler.Web.Controllers;

public record UpdateParticipantRequest(
    string? FullName, 
    string? Role, 
    string? Email, 
    Guid? MeetingId);
#if !DEBUG
[Authorize]
#endif
public class ParticipantsController : ApiControllerBase
{
    private readonly ILogger<ParticipantsController> _logger;

    public ParticipantsController(ILogger<ParticipantsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ParticipantDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ParticipantDto>?>> GetAllParticipants()
    {
        var query = new GetAllParticipantsQuery();
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ParticipantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ParticipantDto?>> GetParticipant(Guid id)
    {
        var query = new GetParticipantQuery(id);
        var result = await Mediator.Send(query);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{id:guid}/meetings")]
    [ProducesResponseType(typeof(List<MeetingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<MeetingDto>?>> GetParticipantMeetings(Guid id)
    {
        var query = new GetParticipantMeetingsQuery(id);
        var result = await Mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateParticipant([FromBody] CreateParticipantCommand command)
    {
        try
        {
            var participantId = await Mediator.Send(command);
            _logger.LogInformation("Participant created successfully: {ParticipantId}", participantId);
        
            return CreatedAtAction(
                nameof(GetParticipant),
                new { id = participantId },
                new { id = participantId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating participant");
            throw;
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateParticipant(Guid id, [FromBody] UpdateParticipantRequest request)
    {
        var command = new UpdateParticipantCommand(
            id, 
            request.FullName, 
            request.Role, 
            request.Email);
        
        await Mediator.Send(command);
        
        _logger.LogInformation("Participant updated successfully: {ParticipantId}", id);
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteParticipant(Guid id)
    {
        var command = new DeleteParticipantCommand(id);
        await Mediator.Send(command);
        
        _logger.LogInformation("Participant deleted successfully: {ParticipantId}", id);
        
        return NoContent();
    }
}