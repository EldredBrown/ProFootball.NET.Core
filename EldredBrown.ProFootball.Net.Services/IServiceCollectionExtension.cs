using Microsoft.Extensions.DependencyInjection;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services.GameServiceNS;
using EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy;

namespace EldredBrown.ProFootball.Net.Services
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddServiceLibrary(this IServiceCollection services)
        {
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IProcessGameStrategyFactory, ProcessGameStrategyFactory>();
            services.AddScoped<IWeeklyUpdateService, WeeklyUpdateService>();

            services.AddSingleton<ILeagueSeasonTotalsRepository, MockLeagueSeasonTotalsRepository>();

            return services;
        }
    }
}
