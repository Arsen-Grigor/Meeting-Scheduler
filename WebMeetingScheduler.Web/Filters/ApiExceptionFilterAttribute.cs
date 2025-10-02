using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebMeetingScheduler.Infrastructure.Exceptions;

namespace WebMeetingScheduler.Web.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public ApiExceptionFilterAttribute()
    {
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(MeetingNotFoundException), HandleMeetingNotFoundException },
            { typeof(ParticipantNotFoundException), HandleParticipantNotFoundException },
            { typeof(NoAvailableTimeSlotException), HandleNoAvailableTimeSlotException },
            { typeof(InvalidSchedulingParametersException), HandleInvalidSchedulingParametersException },
            { typeof(OutsideWorkingHoursException), HandleOutsideWorkingHoursException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
        };
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();
        
        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);
            return;
        }

        if (!context.ModelState.IsValid)
        {
            HandleInvalidModelStateException(context);
            return;
        }

        HandleUnknownException(context);
    }

    private void HandleMeetingNotFoundException(ExceptionContext context)
    {
        var exception = (MeetingNotFoundException)context.Exception;

        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Meeting not found",
            Status = StatusCodes.Status404NotFound,
            Detail = exception.Message
        };

        context.Result = new NotFoundObjectResult(details);
        context.ExceptionHandled = true;
    }

    private void HandleParticipantNotFoundException(ExceptionContext context)
    {
        var exception = (ParticipantNotFoundException)context.Exception;

        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "Participant not found",
            Status = StatusCodes.Status404NotFound,
            Detail = exception.Message
        };

        context.Result = new NotFoundObjectResult(details);
        context.ExceptionHandled = true;
    }

    private void HandleNoAvailableTimeSlotException(ExceptionContext context)
    {
        var exception = (NoAvailableTimeSlotException)context.Exception;

        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "No available time slot",
            Status = StatusCodes.Status400BadRequest,
            Detail = exception.Message
        };

        var response = new
        {
            details.Type,
            details.Title,
            details.Status,
            details.Detail,
            RequestedDuration = exception.RequestedDurationMinutes,
            TimeWindow = new
            {
                Start = exception.EarliestStart,
                End = exception.LatestEnd
            },
            Suggestions = exception.Suggestions
        };

        context.Result = new BadRequestObjectResult(response);
        context.ExceptionHandled = true;
    }

    private void HandleInvalidSchedulingParametersException(ExceptionContext context)
    {
        var exception = (InvalidSchedulingParametersException)context.Exception;

        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Invalid scheduling parameters",
            Status = StatusCodes.Status400BadRequest,
            Detail = exception.Message
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    private void HandleOutsideWorkingHoursException(ExceptionContext context)
    {
        var exception = (OutsideWorkingHoursException)context.Exception;

        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Outside working hours",
            Status = StatusCodes.Status400BadRequest,
            Detail = exception.Message
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    private void HandleInvalidModelStateException(ExceptionContext context)
    {
        var details = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    private void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
            Title = "Unauthorized",
            Status = StatusCodes.Status401Unauthorized,
            Detail = "You are not authorized to access this resource."
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        context.ExceptionHandled = true;
    }

    private void HandleUnknownException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "An error occurred while processing your request.",
            Status = StatusCodes.Status500InternalServerError,
            Detail = context.Exception.Message
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }
}