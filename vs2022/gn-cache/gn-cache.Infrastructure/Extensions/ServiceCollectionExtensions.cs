using GnCache.Application.Services;
using GnCache.Domain;
using GnCache.Infrastructure.Models;
using GnCache.Infrastructure.Options;
using GnCache.Infrastructure.Scheduling;
using GnCache.Infrastructure.Services;
using GnCache.Infrastructure.Startup;
using GnMessaging.Events;
using GnMessaging.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace GnCache.Infrastructure.Extensions;

/// <summary>
/// Extensions pour l'enregistrement des services gn-cache dans le conteneur DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Enregistre tous les services du systeme de cache centralise.
    /// </summary>
    public static IServiceCollection AddGnCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Configuration
        services.Configure<CacheOptions>(
            configuration.GetSection(CacheOptions.SectionName));

        // L1 MemoryCache
        services.AddMemoryCache();

        // L2 Redis (StackExchange)
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "gn-cache:";
        });

        // HttpClient for API calls
        services.AddHttpClient("CacheApiClient", client =>
        {
            var baseUrl = configuration.GetSection("Cache")["ApiGatewayBaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
                client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register all cache entities
        var registrations = BuildCacheRegistrations();
        services.AddSingleton<IReadOnlyList<CacheEntityRegistration>>(registrations);

        // Core services
        services.AddSingleton<ICacheStatusService, CacheStatusService>();
        services.AddSingleton<ICacheEventPublisher, CacheEventPublisher>();
        services.AddSingleton<JsonFileCacheLoader>();
        services.AddSingleton<DatabaseCacheLoader>();

        // Register generic cache services for each entity
        foreach (var reg in registrations)
        {
            var serviceType = typeof(ICacheService<>).MakeGenericType(reg.EntityType);
            var implType = typeof(BaseCacheService<>).MakeGenericType(reg.EntityType);
            var capturedReg = reg;

            services.AddSingleton(serviceType, sp =>
                ActivatorUtilities.CreateInstance(sp, implType, capturedReg));
        }

        // Registry (depends on cache services being registered)
        services.AddSingleton<ICacheRegistry, CacheRegistry>();
        services.AddSingleton<ICacheScheduler, CacheSchedulerService>();

        // RabbitMQ handler for cache invalidation events
        services.AddMessageHandler<CacheInvalidatedEvent, CacheInvalidationHandler>();

        // Quartz scheduler
        services.AddQuartz(q =>
        {
            foreach (var reg in registrations)
            {
                var jobKey = new JobKey($"cache-refresh-{reg.CacheKey}", "cache-jobs");

                q.AddJob<CacheRefreshJob>(opts => opts
                    .WithIdentity(jobKey)
                    .UsingJobData(CacheRefreshJob.CacheKeyDataKey, reg.CacheKey)
                    .StoreDurably());

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"cache-trigger-{reg.CacheKey}", "cache-triggers")
                    .WithCronSchedule(reg.CronExpression));
            }
        });
        services.AddQuartzHostedService(opts => opts.WaitForJobsToComplete = true);

        // Startup service (seed + API load)
        services.AddHostedService<CacheStartupService>();

        return services;
    }

    /// <summary>
    /// Construit la liste des entites de cache a enregistrer.
    /// POINT D'EXTENSION : pour ajouter un nouveau cache, ajouter une entree ici.
    /// </summary>
    private static List<CacheEntityRegistration> BuildCacheRegistrations()
    {
        return
        [
            new CacheEntityRegistration
            {
                CacheKey = "regions",
                EntityType = typeof(Region),
                SourceApiUrl = "/api/geo/regions",
                CronExpression = "0 0 */12 * * ?",
                SeedFileName = "regions.json"
            },
            new CacheEntityRegistration
            {
                CacheKey = "prefectures",
                EntityType = typeof(Prefecture),
                SourceApiUrl = "/api/geo/prefectures",
                CronExpression = "0 0 */12 * * ?",
                SeedFileName = "prefectures.json"
            },
            new CacheEntityRegistration
            {
                CacheKey = "sous-prefectures",
                EntityType = typeof(SousPrefecture),
                SourceApiUrl = "/api/geo/sous-prefectures",
                CronExpression = "0 0 */12 * * ?",
                SeedFileName = "sous-prefectures.json"
            },
            new CacheEntityRegistration
            {
                CacheKey = "niveaux-classes",
                EntityType = typeof(NiveauClasse),
                SourceApiUrl = "/api/pedagogie/niveaux-classes",
                CronExpression = "0 0 */6 * * ?",
                SeedFileName = "niveaux-classes.json"
            },
            new CacheEntityRegistration
            {
                CacheKey = "matieres",
                EntityType = typeof(Matiere),
                SourceApiUrl = "/api/pedagogie/matieres",
                CronExpression = "0 0 */6 * * ?",
                SeedFileName = "matieres.json"
            },
            new CacheEntityRegistration
            {
                CacheKey = "annees-scolaires",
                EntityType = typeof(AnneeScolaire),
                SourceApiUrl = "/api/inscriptions/annees-scolaires",
                CronExpression = "0 0 0 * * ?",
                SeedFileName = "annees-scolaires.json"
            },
            new CacheEntityRegistration
            {
                CacheKey = "types-evaluation",
                EntityType = typeof(TypeEvaluation),
                SourceApiUrl = "/api/evaluations/types",
                CronExpression = "0 0 */12 * * ?",
                SeedFileName = "types-evaluation.json"
            },
            new CacheEntityRegistration
            {
                CacheKey = "services-parents",
                EntityType = typeof(ServiceParent),
                SourceApiUrl = "/api/services-premium/services-parents",
                CronExpression = "0 0 */12 * * ?",
                SeedFileName = "services-parents.json"
            }
        ];
    }
}
