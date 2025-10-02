using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebMeetingScheduler.Application.Authentication;
using WebMeetingScheduler.Application.Common;
using WebMeetingScheduler.Domain.Interfaces;
using WebMeetingScheduler.Infrastructure.Authentication;
using WebMeetingScheduler.Infrastructure.Persistence;
using WebMeetingScheduler.Infrastructure.Persistence.Repositories;
using WebMeetingScheduler.Infrastructure.Services;
using WebMeetingScheduler.Infrastructure.Settings;

namespace WebMeetingScheduler.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
        
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection string not found in configuration.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IDomainEventService, DomainEventService>();
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddScoped<IMeetingScheduler, MeetingSchedulerService>();
        services.AddScoped<IMeetingsRepository, MeetingsRepository>();
        services.AddScoped<IParticipantsRepository, ParticipantsRepository>();
        services.AddHttpClient<IAuthenticationService, KeycloakAuthenticationService>();

        return services;
    }
    
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        await AppDbContextSeed.SeedSampleDataAsync(context);
    }
}