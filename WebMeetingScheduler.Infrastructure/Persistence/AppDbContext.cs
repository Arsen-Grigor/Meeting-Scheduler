using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Common;
using WebMeetingScheduler.Domain.Entities.Meetings;
using WebMeetingScheduler.Domain.Entities.Participants;
using WebMeetingScheduler.Domain.Interfaces;

namespace WebMeetingScheduler.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly IDateTime _dateTime;
    private readonly IDomainEventService _domainEventService;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IDateTime dateTime,
        IDomainEventService domainEventService) : base(options)
    {
        _dateTime = dateTime;
        _domainEventService = domainEventService;
    }

    public DbSet<Meeting> Meetings => Set<Meeting>();
    public DbSet<Participant> Participants => Set<Participant>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = ChangeTracker.Entries<Entity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        int result;
        try
        {
            result = await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            ex.Entries.Single().Reload();
            throw;
        }

        await DispatchDomainEvents(domainEvents, cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    private async Task DispatchDomainEvents(List<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _domainEventService.Publish(domainEvent);
        }
    }
}