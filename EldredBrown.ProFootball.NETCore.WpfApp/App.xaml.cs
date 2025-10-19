using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EldredBrown.ProFootball.NETCore.Data;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS.ProcessGameStrategy;
using EldredBrown.ProFootball.NETCore.WpfApp.Main;
using EldredBrown.ProFootball.NETCore.WpfApp.Properties;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.Rankings;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.SeasonStandings;
using EldredBrown.ProFootball.NETCore.WpfApp.UserControls.TeamSeasons;
using EldredBrown.ProFootball.NETCore.WpfApp.Windows.GameFinder;
using EldredBrown.ProFootball.NETCore.WpfApp.Windows.GamePredictor;
using EldredBrown.ProFootball.NETCore.WpfApp.Windows.Games;

namespace EldredBrown.ProFootball.NETCore.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider? ServiceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddDbContextPool<ProFootballDbContext>(options =>
            {
                options.UseSqlServer(Settings.Default.ProFootballDbConnectionString);
            });

            services.AddSingleton<MainWindow>();
            services.AddScoped<IMainWindowViewModel, MainWindowViewModel>();
            services.AddScoped<ITeamSeasonsControlViewModel, TeamSeasonsControlViewModel>();
            services.AddScoped<ISeasonStandingsControlViewModel, SeasonStandingsControlViewModel>();
            services.AddScoped<IRankingsControlViewModel, RankingsControlViewModel>();

            services.AddScoped<IGamesWindowFactory, GamesWindowFactory>();
            services.AddScoped<IGamesWindow, GamesWindow>();
            services.AddScoped<IGamesWindowViewModel, GamesWindowViewModel>();

            services.AddScoped<IGameFinderWindowFactory, GameFinderWindowFactory>();
            services.AddScoped<IGameFinderWindow, GameFinderWindow>();
            services.AddScoped<IGameFinderWindowViewModel, GameFinderWindowViewModel>();

            services.AddScoped<IGamePredictorWindowFactory, GamePredictorWindowFactory>();
            services.AddScoped<IGamePredictorWindow, GamePredictorWindow>();
            services.AddScoped<IGamePredictorWindowViewModel, GamePredictorWindowViewModel>();

            services.AddScoped<IMessageBoxService, MessageBoxService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IWeeklyUpdateService, WeeklyUpdateService>();
            services.AddScoped<IGamePredictorService, GamePredictorService>();

            services.AddScoped<IProcessGameStrategyFactory, ProcessGameStrategyFactory>();

            services.AddScoped<ISeasonRepository, SeasonRepository>();
            services.AddScoped<ITeamSeasonRepository, TeamSeasonRepository>();
            services.AddScoped<ITeamSeasonScheduleRepository, TeamSeasonScheduleRepository>();
            services.AddScoped<ISeasonStandingsRepository, SeasonStandingsRepository>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<ILeagueSeasonRepository, LeagueSeasonRepository>();
            services.AddScoped<ILeagueSeasonTotalsRepository, MockLeagueSeasonTotalsRepository>();
            services.AddScoped<ISharedRepository, SharedRepository>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            if (ServiceProvider is null)
            {
                throw new Exception($"{GetType()}: {nameof(ServiceProvider)}");
            }

            var mainWindow = ServiceProvider.GetService<MainWindow>();
            if (mainWindow is null)
            {
                throw new Exception($"{GetType()}: {nameof(MainWindow)} service could not be found.");
            }

            mainWindow.Show();
        }
    }
}
