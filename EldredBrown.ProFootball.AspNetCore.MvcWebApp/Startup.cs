using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Conference;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Division;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.League;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Season;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonRankings;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.SeasonStandings;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Team;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason;
using EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.LeagueSeason;
using EldredBrown.ProFootball.Net.Data;
using EldredBrown.ProFootball.Net.Data.Repositories;
using EldredBrown.ProFootball.Net.Services;
using EldredBrown.ProFootball.Net.Services.GameServiceNS;
using EldredBrown.ProFootball.Net.Services.GameServiceNS.ProcessGameStrategy;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddDbContext<ProFootballDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ProFootballDb"));
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ProFootballDb"));
            });

            services.AddDefaultIdentity<IdentityUser>(opts => opts.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IConnectionStringProvider, DbContextConnectionStringProvider>();
            services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

            services.AddScoped<ISeasonRepository, SeasonRepository>();
            services.AddScoped<ILeagueRepository, LeagueRepository>();
            services.AddScoped<IConferenceRepository, ConferenceRepository>();
            services.AddScoped<IDivisionRepository, DivisionRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<ITeamSeasonRepository, TeamSeasonRepository>();
            services.AddScoped<ITeamSeasonScheduleRepository, TeamSeasonScheduleRepository>();
            services.AddScoped<ILeagueSeasonRepository, LeagueSeasonRepository>();
            services.AddScoped<ILeagueSeasonTotalsRepository, LeagueSeasonTotalsRepository>();
            services.AddScoped<ISeasonStandingsRepository, SeasonStandingsRepository>();
            services.AddScoped<ISeasonRankingsRepository, SeasonRankingsRepository>();
            services.AddScoped<ISharedRepository, SharedRepository>();

            services.AddScoped<IGameService, GameService>();
            services.AddScoped<IProcessGameStrategyFactory, ProcessGameStrategyFactory>();
            services.AddScoped<IWeeklyUpdateService, WeeklyUpdateService>();
            //services.AddScoped<IGamePredictorService, GamePredictorService>();

            services.AddScoped<ISeasonIndexViewModel, SeasonIndexViewModel>();
            services.AddScoped<ISeasonDetailsViewModel, SeasonDetailsViewModel>();
            services.AddScoped<ILeagueIndexViewModel, LeagueIndexViewModel>();
            services.AddScoped<ILeagueDetailsViewModel, LeagueDetailsViewModel>();
            services.AddScoped<ILeagueViewModelMapper, LeagueViewModelMapper>();
            services.AddScoped<IConferenceIndexViewModel, ConferenceIndexViewModel>();
            services.AddScoped<IConferenceDetailsViewModel, ConferenceDetailsViewModel>();
            services.AddScoped<IConferenceViewModelMapper, ConferenceViewModelMapper>();
            services.AddScoped<IDivisionIndexViewModel, DivisionIndexViewModel>();
            services.AddScoped<IDivisionDetailsViewModel, DivisionDetailsViewModel>();
            services.AddScoped<IDivisionViewModelMapper, DivisionViewModelMapper>();
            services.AddScoped<ITeamIndexViewModel, TeamIndexViewModel>();
            services.AddScoped<ITeamDetailsViewModel, TeamDetailsViewModel>();
            services.AddScoped<IGameIndexViewModel, GameIndexViewModel>();
            services.AddScoped<IGameDetailsViewModel, GameDetailsViewModel>();
            services.AddScoped<IGameViewModelMapper, GameViewModelMapper>();
            services.AddScoped<ILeagueSeasonIndexViewModel, LeagueSeasonIndexViewModel>();
            services.AddScoped<ILeagueSeasonDetailsViewModel, LeagueSeasonDetailsViewModel>();
            services.AddScoped<ILeagueSeasonViewModelMapper, LeagueSeasonViewModelMapper>();
            services.AddScoped<ITeamSeasonIndexViewModel, TeamSeasonIndexViewModel>();
            services.AddScoped<ITeamSeasonDetailsViewModel, TeamSeasonDetailsViewModel>();
            services.AddScoped<ITeamSeasonViewModelMapper, TeamSeasonViewModelMapper>();
            services.AddScoped<ISeasonStandingsIndexViewModel, SeasonStandingsIndexViewModel>();
            services.AddScoped<ISeasonRankingsIndexViewModel, SeasonRankingsIndexViewModel>();

            services.AddServiceLibrary();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            CreateUserRole(services).Wait();
        }

        private async Task CreateUserRole(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var roleCheck = await roleManager.RoleExistsAsync("Admin");
            if (!roleCheck)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var user = await userManager.FindByEmailAsync("eldred.brown@outlook.com");
            if (!(user is null))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
