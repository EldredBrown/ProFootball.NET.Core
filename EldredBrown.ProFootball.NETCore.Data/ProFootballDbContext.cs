using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data
{
    /// <summary>
    /// Represents the ProFootball database.
    /// </summary>
    public class ProFootballDbContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFootballDbContext"/> class.
        /// </summary>
        /// <param name="options"></param>
        public ProFootballDbContext(DbContextOptions<ProFootballDbContext> options)
            : base(options)
        {

        }

        /// <summary>
        /// Gets or sets the Leagues data source.
        /// </summary>
        public DbSet<League>? Leagues { get; set; }

        /// <summary>
        /// Gets or sets the Conferences data source.
        /// </summary>
        public DbSet<Conference>? Conferences { get; set; }

        /// <summary>
        /// Gets or sets the Divisions data source.
        /// </summary>
        public DbSet<Division>? Divisions { get; set; }

        /// <summary>
        /// Gets or sets the Teams data source.
        /// </summary>
        public DbSet<Team>? Teams { get; set; }

        /// <summary>
        /// Gets or sets the Seasons data source.
        /// </summary>
        public DbSet<Season>? Seasons { get; set; }

        /// <summary>
        /// Gets or sets the Games data source.
        /// </summary>
        public DbSet<Game>? Games { get; set; }

        /// <summary>
        /// Gets or sets the LeagueSeasons data source.
        /// </summary>
        public DbSet<LeagueSeason>? LeagueSeasons { get; set; }

        /// <summary>
        /// Gets or sets the TeamSeasons data source.
        /// </summary>
        public DbSet<TeamSeason>? TeamSeasons { get; set; }

        /// <summary>
        /// Gets or sets the TeamSeasonScheduleProfile data source.
        /// </summary>
        public DbSet<TeamSeasonOpponentProfile>? TeamSeasonScheduleProfile { get; set; }

        /// <summary>
        /// Gets or sets the TeamSeasonScheduleTotals data source.
        /// </summary>
        public DbSet<TeamSeasonScheduleTotals>? TeamSeasonScheduleTotals { get; set; }

        /// <summary>
        /// Gets or sets the TeamSeasonScheduleAverages data source.
        /// </summary>
        public DbSet<TeamSeasonScheduleAverages>? TeamSeasonScheduleAverages { get; set; }

        /// <summary>
        /// Gets or sets the SeasonStandings data source.
        /// </summary>
        public DbSet<SeasonTeamStanding>? SeasonStandings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TeamSeasonOpponentProfile>().HasNoKey();
            modelBuilder.Entity<TeamSeasonScheduleTotals>().HasNoKey();
            modelBuilder.Entity<TeamSeasonScheduleAverages>().HasNoKey();
            modelBuilder.Entity<SeasonTeamStanding>().HasNoKey();
        }
    }
}
