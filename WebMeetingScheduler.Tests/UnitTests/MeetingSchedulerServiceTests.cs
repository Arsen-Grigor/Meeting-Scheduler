using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;
using WebMeetingScheduler.Domain.Exceptions;
using WebMeetingScheduler.Infrastructure.Exceptions;
using WebMeetingScheduler.Infrastructure.Services;
using Xunit;

namespace WebMeetingScheduler.Tests.Unit.Services;

public class MeetingSchedulerServiceTests
{
    private readonly MeetingSchedulerService _sut;

    public MeetingSchedulerServiceTests()
    {
        _sut = new MeetingSchedulerService();
    }

    #region Happy Path Tests

    public class HappyPathTests : MeetingSchedulerServiceTests
    {
        [Fact]
        public void TryScheduleMeeting_NoConflictingMeetings_ReturnsEarliestSlotWithinWorkingHours()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var earliestStart = DateTime.UtcNow.Date.AddDays(1).AddHours(8);
            var latestEnd = DateTime.UtcNow.Date.AddDays(1).AddHours(18);
            var durationMinutes = 60;

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
            Assert.True(result.Value.Hour >= MeetingConstants.Meeting.WorkingHoursStart);
            Assert.True(result.Value.AddMinutes(durationMinutes).Hour <= MeetingConstants.Meeting.WorkingHoursEnd);
        }

        [Fact]
        public void TryScheduleMeeting_WithCustomDuration30Minutes_FindsAvailableSlot()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var baseDate = DateTime.UtcNow.Date.AddDays(1);
            var earliestStart = baseDate.AddHours(10);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 30;

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
            var expectedEnd = result.Value.AddMinutes(durationMinutes);
            Assert.True(expectedEnd <= latestEnd);
        }

        [Fact]
        public void TryScheduleMeeting_WithCustomDuration120Minutes_FindsAvailableSlot()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var baseDate = DateTime.UtcNow.Date.AddDays(1);
            var earliestStart = baseDate.AddHours(9);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 120;

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
            var expectedEnd = result.Value.AddMinutes(durationMinutes);
            Assert.True(expectedEnd.Hour <= MeetingConstants.Meeting.WorkingHoursEnd);
        }

        [Fact]
        public void TryScheduleMeeting_ScheduleBetweenTwoMeetings_ReturnsSlotBetweenMeetings()
        {
            var participant1 = (ParticipantId)Guid.NewGuid();
            var baseDate = DateTime.UtcNow.Date.AddDays(1);

            var meeting1 = CreateScheduledMeeting(
                baseDate.AddHours(10),
                baseDate.AddHours(11),
                participant1);

            var meeting2 = CreateScheduledMeeting(
                baseDate.AddHours(14),
                baseDate.AddHours(15),
                participant1);

            var existingMeetings = new List<Meeting> { meeting1, meeting2 };
            var participantIds = new List<ParticipantId> { participant1 };
            var earliestStart = baseDate.AddHours(10);
            var latestEnd = baseDate.AddHours(20);
            var durationMinutes = 60;

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
            Assert.True(result.Value >= meeting1.EndTime);
            Assert.True(result.Value.AddMinutes(durationMinutes) <= meeting2.StartTime);
        }

        [Fact]
        public void TryScheduleMeeting_WithOverlapTolerance_FindsSlotWithMinorOverlap()
        {
            var participant1 = (ParticipantId)Guid.NewGuid();
            var baseDate = DateTime.UtcNow.Date.AddDays(1);

            var meeting1 = CreateScheduledMeeting(
                baseDate.AddHours(10),
                baseDate.AddHours(11),
                participant1);

            var meeting2 = CreateScheduledMeeting(
                baseDate.AddHours(11).AddMinutes(10),
                baseDate.AddHours(12).AddMinutes(10),
                participant1);

            var existingMeetings = new List<Meeting> { meeting1, meeting2 };
            var participantIds = new List<ParticipantId> { participant1 };
            var earliestStart = baseDate.AddHours(10);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 60;

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
        }

        [Fact]
        public void TryScheduleMeeting_ShortDuration15Minutes_FitsInSmallGap()
        {
            var participant1 = (ParticipantId)Guid.NewGuid();
            var baseDate = DateTime.UtcNow.Date.AddDays(1);

            var meeting1 = CreateScheduledMeeting(
                baseDate.AddHours(10),
                baseDate.AddHours(11),
                participant1);

            var meeting2 = CreateScheduledMeeting(
                baseDate.AddHours(11).AddMinutes(30),
                baseDate.AddHours(12),
                participant1);

            var existingMeetings = new List<Meeting> { meeting1, meeting2 };
            var participantIds = new List<ParticipantId> { participant1 };
            var earliestStart = baseDate.AddHours(10);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 15;

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
            Assert.True(result.Value >= meeting1.EndTime);
            Assert.True(result.Value.AddMinutes(durationMinutes) <= meeting2.StartTime);
        }
    }

    #endregion

    #region Validation Tests

    public class ValidationTests : MeetingSchedulerServiceTests
    {
        [Fact]
        public void TryScheduleMeeting_DurationTooShort_ThrowsInvalidSchedulingParametersException()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var earliestStart = DateTime.UtcNow.AddDays(1);
            var latestEnd = earliestStart.AddHours(8);
            var durationMinutes = 10;

            var exception = Assert.Throws<InvalidSchedulingParametersException>(() =>
                _sut.TryScheduleMeeting(
                    existingMeetings,
                    participantIds,
                    durationMinutes,
                    earliestStart,
                    latestEnd));

            Assert.Contains("minimum allowed duration", exception.Message);
            Assert.Contains("15", exception.Message);
        }

        [Fact]
        public void TryScheduleMeeting_DurationTooLong_ThrowsInvalidSchedulingParametersException()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var earliestStart = DateTime.UtcNow.AddDays(1);
            var latestEnd = earliestStart.AddHours(8);
            var durationMinutes = 200;

            var exception = Assert.Throws<InvalidSchedulingParametersException>(() =>
                _sut.TryScheduleMeeting(
                    existingMeetings,
                    participantIds,
                    durationMinutes,
                    earliestStart,
                    latestEnd));

            Assert.Contains("maximum allowed duration", exception.Message);
            Assert.Contains("180", exception.Message);
        }

        [Fact]
        public void TryScheduleMeeting_DurationExactlyMinimum15Minutes_Succeeds()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var baseDate = DateTime.UtcNow.Date.AddDays(1);
            var earliestStart = baseDate.AddHours(10);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 15;

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
        }

        [Fact]
        public void TryScheduleMeeting_DurationExactlyMaximum180Minutes_Succeeds()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var baseDate = DateTime.UtcNow.Date.AddDays(1);
            var earliestStart = baseDate.AddHours(9);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 180;

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
        }

        [Fact]
        public void TryScheduleMeeting_EarliestStartAfterLatestEnd_ThrowsInvalidSchedulingParametersException()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var earliestStart = DateTime.UtcNow.AddDays(1).AddHours(5);
            var latestEnd = DateTime.UtcNow.AddDays(1).AddHours(2);
            var durationMinutes = 60;

            var exception = Assert.Throws<InvalidSchedulingParametersException>(() =>
                _sut.TryScheduleMeeting(
                    existingMeetings,
                    participantIds,
                    durationMinutes,
                    earliestStart,
                    latestEnd));

            Assert.Contains("must be before latest end time", exception.Message);
        }

        [Fact]
        public void TryScheduleMeeting_TimeWindowSmallerThanDuration_ThrowsInvalidSchedulingParametersException()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var baseDate = DateTime.UtcNow.Date.AddDays(1);
            var earliestStart = baseDate.AddHours(10);
            var latestEnd = baseDate.AddHours(10).AddMinutes(30);
            var durationMinutes = 60;

            var exception = Assert.Throws<InvalidSchedulingParametersException>(() =>
                _sut.TryScheduleMeeting(
                    existingMeetings,
                    participantIds,
                    durationMinutes,
                    earliestStart,
                    latestEnd));

            Assert.Contains("Time window", exception.Message);
            Assert.Contains("smaller than the requested duration", exception.Message);
        }

        [Fact]
        public void TryScheduleMeeting_SchedulingInPast_ThrowsInvalidSchedulingParametersException()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var earliestStart = DateTime.UtcNow.AddDays(-1); // Yesterday
            var latestEnd = earliestStart.AddHours(8);
            var durationMinutes = 60;

            var exception = Assert.Throws<InvalidSchedulingParametersException>(() =>
                _sut.TryScheduleMeeting(
                    existingMeetings,
                    participantIds,
                    durationMinutes,
                    earliestStart,
                    latestEnd));

            Assert.Contains("Cannot schedule meetings in the past", exception.Message);
        }
    }

    #endregion

    #region Working Hours Tests

    public class WorkingHoursTests : MeetingSchedulerServiceTests
    {
        [Fact]
        public void TryScheduleMeeting_RequestBeforeWorkingHours_AdjustsToWorkingHoursStart()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var baseDate = DateTime.UtcNow.Date.AddDays(1);
            var earliestStart = baseDate.AddHours(7);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 60;

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
            Assert.True(result.Value.Hour >= MeetingConstants.Meeting.WorkingHoursStart);
        }

        [Fact]
        public void TryScheduleMeeting_MeetingExtendsToNextDayIfDoesNotFitToday_ReturnsNextDaySlot()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var baseDate = DateTime.UtcNow.Date.AddDays(1);
            var earliestStart = baseDate.AddHours(16).AddMinutes(30); // Late in the day
            var latestEnd = baseDate.AddDays(2).AddHours(17);
            var durationMinutes = 120; // 2 hours - won't fit in remaining work hours

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
            Assert.True(result.Value.Date > baseDate.Date);
            Assert.Equal(MeetingConstants.Meeting.WorkingHoursStart, result.Value.Hour);
        }

        [Fact]
        public void TryScheduleMeeting_CustomDuration45MinutesNearEndOfDay_AdjustsToNextDay()
        {
            var existingMeetings = new List<Meeting>();
            var participantIds = new List<ParticipantId> { (ParticipantId)Guid.NewGuid() };
            var baseDate = DateTime.UtcNow.Date.AddDays(1);
            var earliestStart = baseDate.AddHours(16).AddMinutes(30);
            var latestEnd = baseDate.AddDays(2).AddHours(17);
            var durationMinutes = 45;

            var result = _sut.TryScheduleMeeting(
                existingMeetings,
                participantIds,
                durationMinutes,
                earliestStart,
                latestEnd);

            Assert.NotNull(result);
            Assert.True(result.Value.Date > baseDate.Date);
        }
    }

    #endregion

    #region No Available Slot Tests

    public class NoAvailableSlotTests : MeetingSchedulerServiceTests
    {
        [Fact]
        public void TryScheduleMeeting_AllSlotsOccupied_ThrowsNoAvailableTimeSlotException()
        {
            var participant1 = (ParticipantId)Guid.NewGuid();
            var baseDate = DateTime.UtcNow.Date.AddDays(1);

            var meetings = new List<Meeting>();
            for (int hour = 9; hour < 17; hour += 2)
            {
                meetings.Add(CreateScheduledMeeting(
                    baseDate.AddHours(hour),
                    baseDate.AddHours(hour + 2),
                    participant1));
            }

            var participantIds = new List<ParticipantId> { participant1 };
            var earliestStart = baseDate.AddHours(9);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 60;

            var exception = Assert.Throws<NoAvailableTimeSlotException>(() =>
                _sut.TryScheduleMeeting(
                    meetings,
                    participantIds,
                    durationMinutes,
                    earliestStart,
                    latestEnd));

            Assert.Equal(durationMinutes, exception.RequestedDurationMinutes);
            Assert.NotEmpty(exception.Suggestions);
        }

        [Fact]
        public void TryScheduleMeeting_NoAvailableSlotForCustomDuration_ProvidesSuggestions()
        {
            var participant1 = (ParticipantId)Guid.NewGuid();
            var baseDate = DateTime.UtcNow.Date.AddDays(1);

            var meetings = new List<Meeting>
            {
                CreateScheduledMeeting(baseDate.AddHours(9), baseDate.AddHours(12), participant1),
                CreateScheduledMeeting(baseDate.AddHours(13), baseDate.AddHours(17), participant1)
            };

            var participantIds = new List<ParticipantId> { participant1 };
            var earliestStart = baseDate.AddHours(9);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 120;

            var exception = Assert.Throws<NoAvailableTimeSlotException>(() =>
                _sut.TryScheduleMeeting(
                    meetings,
                    participantIds,
                    durationMinutes,
                    earliestStart,
                    latestEnd));

            Assert.Equal(120, exception.RequestedDurationMinutes);
            Assert.NotEmpty(exception.Suggestions);
            Assert.Contains(exception.Suggestions, s => s.Contains("reducing meeting duration"));
        }

        [Fact]
        public void TryScheduleMeeting_MultipleParticipantsAllBusy_SuggestsReducingParticipants()
        {
            var participant1 = (ParticipantId)Guid.NewGuid();
            var participant2 = (ParticipantId)Guid.NewGuid();
            var baseDate = DateTime.UtcNow.Date.AddDays(1);

            var meetings = new List<Meeting>
            {
                CreateScheduledMeeting(baseDate.AddHours(9), baseDate.AddHours(12), participant1),
                CreateScheduledMeeting(baseDate.AddHours(13), baseDate.AddHours(17), participant1),
                CreateScheduledMeeting(baseDate.AddHours(10), baseDate.AddHours(13), participant2),
                CreateScheduledMeeting(baseDate.AddHours(14), baseDate.AddHours(17), participant2)
            };

            var participantIds = new List<ParticipantId> { participant1, participant2 };
            var earliestStart = baseDate.AddHours(9);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 60;

            var exception = Assert.Throws<NoAvailableTimeSlotException>(() =>
                _sut.TryScheduleMeeting(
                    meetings,
                    participantIds,
                    durationMinutes,
                    earliestStart,
                    latestEnd));

            Assert.NotEmpty(exception.Suggestions);
            Assert.Contains(exception.Suggestions, 
                s => s.Contains("reducing the number of participants") || s.Contains("splitting"));
        }

        [Fact]
        public void TryScheduleMeeting_RequestedDuration90Minutes_NoSlotAvailable_SuggestsAlternatives()
        {
            var participant1 = (ParticipantId)Guid.NewGuid();
            var baseDate = DateTime.UtcNow.Date.AddDays(1);

            var meetings = new List<Meeting>
            {
                CreateScheduledMeeting(baseDate.AddHours(9), baseDate.AddHours(10), participant1),
                CreateScheduledMeeting(baseDate.AddHours(11), baseDate.AddHours(13), participant1),
                CreateScheduledMeeting(baseDate.AddHours(14), baseDate.AddHours(17), participant1)
            };

            var participantIds = new List<ParticipantId> { participant1 };
            var earliestStart = baseDate.AddHours(9);
            var latestEnd = baseDate.AddHours(17);
            var durationMinutes = 90;

            var exception = Assert.Throws<NoAvailableTimeSlotException>(() =>
                _sut.TryScheduleMeeting(
                    meetings,
                    participantIds,
                    durationMinutes,
                    earliestStart,
                    latestEnd));

            Assert.Equal(90, exception.RequestedDurationMinutes);
            Assert.Contains(exception.Suggestions, s => s.Contains("60 minutes or less"));
        }
    }

    #endregion

    #region Test Helper Methods

    private static Meeting CreateScheduledMeeting(
        DateTime startTime,
        DateTime endTime,
        params ParticipantId[] participantIds)
    {
        var meeting = Meeting.Create(
            Guid.NewGuid(),
            "Test Meeting",
            "Test Description");

        meeting.Schedule(startTime, endTime);

        foreach (var participantId in participantIds)
        {
            meeting.AddParticipant(participantId);
        }

        return meeting;
    }

    #endregion
}