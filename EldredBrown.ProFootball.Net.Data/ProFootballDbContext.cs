using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data;

public partial class ProFootballDbContext : DbContext
{
    public ProFootballDbContext()
    {
    }

    public ProFootballDbContext(DbContextOptions<ProFootballDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<Conference> Conferences { get; set; }

    public virtual DbSet<Division> Divisions { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<League> Leagues { get; set; }

    public virtual DbSet<LeagueSeason> LeagueSeasons { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamSeason> TeamSeasons { get; set; }

    /// <summary>
    /// Gets or sets the TeamSeasonScheduleProfile data source.
    /// </summary>
    public virtual DbSet<TeamSeasonOpponentProfile>? TeamSeasonScheduleProfile { get; set; }

    /// <summary>
    /// Gets or sets the TeamSeasonScheduleTotals data source.
    /// </summary>
    public virtual DbSet<TeamSeasonScheduleTotals>? TeamSeasonScheduleTotals { get; set; }

    /// <summary>
    /// Gets or sets the TeamSeasonScheduleAverages data source.
    /// </summary>
    public virtual DbSet<TeamSeasonScheduleAverages>? TeamSeasonScheduleAverages { get; set; }

    /// <summary>
    /// Gets or sets the SeasonStandings data source.
    /// </summary>
    public virtual DbSet<SeasonTeamStanding>? SeasonStandings { get; set; }

    /// <summary>
    /// Gets or sets the OffensiveRankings data source.
    /// </summary>
    public virtual DbSet<RankingsOffensiveTeamSeason>? OffensiveRankings { get; set; }

    /// <summary>
    /// Gets or sets the DefensiveRankings data source.
    /// </summary>
    public virtual DbSet<RankingsDefensiveTeamSeason>? DefensiveRankings { get; set; }

    /// <summary>
    /// Gets or sets the TotalRankings data source.
    /// </summary>
    public virtual DbSet<RankingsTotalTeamSeason>? TotalRankings { get; set; }

    /// <summary>
    /// Gets or sets the LeagueSeasonTotals data source.
    /// </summary>
    public virtual DbSet<LeagueSeasonTotals>? LeagueSeasonTotals { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only configure if not already configured externally (e.g. from DI)
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("ProFootballDb"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Season>(entity =>
        {
            entity.ToTable("Season");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NumOfWeeksScheduled).HasColumnName("num_of_weeks_scheduled");
            entity.Property(e => e.NumOfWeeksCompleted).HasColumnName("num_of_weeks_completed");
        });

        modelBuilder.Entity<League>(entity =>
        {
            entity.ToTable("League");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ShortName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("short_name");
            entity.Property(e => e.LongName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("long_name");
            entity.Property(e => e.FirstSeasonId).HasColumnName("first_season_id");
            entity.Property(e => e.LastSeasonId).HasColumnName("last_season_id");

            entity.HasOne(d => d.FirstSeasonIdNavigation).WithMany(p => p.LeagueFirstSeasonIdNavigations)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.FirstSeasonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_League_Season_FirstSeasonId");

            entity.HasOne(d => d.LastSeasonIdNavigation).WithMany(p => p.LeagueLastSeasonIdNavigations)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.LastSeasonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_League_Season_LastSeasonId");

            entity.HasIndex(e => e.ShortName, "UQ_League_ShortName").IsUnique();
            entity.HasIndex(e => e.LongName, "UQ_League_LongName").IsUnique();
            entity.HasIndex(e => e.FirstSeasonId, "IX_FK_League_Season_FirstSeasonId");
            entity.HasIndex(e => e.LastSeasonId, "IX_FK_League_Season_LastSeasonId");
        });

        modelBuilder.Entity<Conference>(entity =>
        {
            entity.ToTable("Conference");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ShortName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("short_name");
            entity.Property(e => e.LongName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("long_name");
            entity.Property(e => e.LeagueId).HasColumnName("league_id");
            entity.Property(e => e.FirstSeasonId).HasColumnName("first_season_id");
            entity.Property(e => e.LastSeasonId).HasColumnName("last_season_id");

            entity.HasOne(d => d.LeagueIdNavigation).WithMany(p => p.Conferences)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.LeagueId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Conference_League_LeagueId");

            entity.HasOne(d => d.FirstSeasonIdNavigation).WithMany(p => p.ConferenceFirstSeasonIdNavigations)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.FirstSeasonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Conference_Season_FirstSeasonId");

            entity.HasOne(d => d.LastSeasonIdNavigation).WithMany(p => p.ConferenceLastSeasonIdNavigations)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.LastSeasonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Conference_Season_LastSeasonId");

            entity.HasIndex(e => e.ShortName, "UQ_Conference_ShortName").IsUnique();
            entity.HasIndex(e => e.LongName, "UQ_Conference_LongName").IsUnique();
            entity.HasIndex(e => e.LeagueId, "IX_FK_Conference_League_LeagueId");
            entity.HasIndex(e => e.FirstSeasonId, "IX_FK_Conference_Season_FirstSeasonId");
            entity.HasIndex(e => e.LastSeasonId, "IX_FK_Conference_Season_LastSeasonId");
        });

        modelBuilder.Entity<Division>(entity =>
        {
            entity.ToTable("Division");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.LeagueId).HasColumnName("league_id");
            entity.Property(e => e.ConferenceId).HasColumnName("conference_id");
            entity.Property(e => e.FirstSeasonId).HasColumnName("first_season_id");
            entity.Property(e => e.LastSeasonId).HasColumnName("last_season_id");

            entity.HasOne(d => d.LeagueIdNavigation).WithMany(p => p.Divisions)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.LeagueId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Division_League_LeagueId");

            entity.HasOne(d => d.ConferenceIdNavigation).WithMany(p => p.Divisions)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.ConferenceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Division_Conference_ConferenceId");

            entity.HasOne(d => d.FirstSeasonIdNavigation).WithMany(p => p.DivisionFirstSeasonIdNavigations)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.FirstSeasonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Division_Season_FirstSeasonId");

            entity.HasOne(d => d.LastSeasonIdNavigation).WithMany(p => p.DivisionLastSeasonIdNavigations)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.LastSeasonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Division_Season_LastSeasonId");

            entity.HasIndex(e => e.Name, "UQ_Division_Name").IsUnique();
            entity.HasIndex(e => e.LeagueId, "IX_FK_Division_League_LeagueId");
            entity.HasIndex(e => e.ConferenceId, "IX_FK_Division_Conference_ConferenceId");
            entity.HasIndex(e => e.FirstSeasonId, "IX_FK_Division_Season_FirstSeasonId");
            entity.HasIndex(e => e.LastSeasonId, "IX_FK_Division_Season_LastSeasonId");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("Team");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasIndex(e => e.Name, "UQ_Team_Name").IsUnique();
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("Game");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.Week).HasColumnName("week");
            entity.Property(e => e.GuestName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("guest_name");
            entity.Property(e => e.GuestScore).HasColumnName("guest_score");
            entity.Property(e => e.HostName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("host_name");
            entity.Property(e => e.HostScore).HasColumnName("host_score");
            entity.Property(e => e.IsPlayoff).HasColumnName("is_playoff");
            entity.Property(e => e.Notes)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("notes");

            entity.HasOne(d => d.SeasonIdNavigation).WithMany(p => p.Games)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.SeasonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Game_Season_SeasonId");

            entity.HasIndex(e => e.SeasonId, "IX_FK_Game_Season_SeasonId");
            entity.HasIndex(e => new { e.SeasonId, e.Week, e.GuestName, e.HostName }, "UQ_Game_Season_Week_Teams").IsUnique();
        });

        modelBuilder.Entity<LeagueSeason>(entity =>
        {
            entity.ToTable("LeagueSeason");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LeagueId).HasColumnName("league_id");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.TotalGames).HasColumnName("total_games");
            entity.Property(e => e.TotalPoints).HasColumnName("total_points");
            entity.Property(e => e.AveragePoints)
                .HasColumnType("decimal(18, 16)")
                .HasColumnName("average_points");

            entity.HasOne(d => d.LeagueIdNavigation).WithMany(p => p.LeagueSeasons)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.LeagueId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_LeagueSeason_League_LeagueId");

            entity.HasOne(d => d.SeasonIdNavigation).WithMany(p => p.LeagueSeasons)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.SeasonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_LeagueSeason_Season_SeasonId");

            entity.HasIndex(e => e.LeagueId, "IX_FK_LeagueSeason_League_LeagueId");
            entity.HasIndex(e => e.SeasonId, "IX_FK_LeagueSeason_Season_SeasonId");
            entity.HasIndex(e => new { e.LeagueId, e.SeasonId }, "UQ_LeagueSeason_LeagueId_SeasonId").IsUnique();
        });

        modelBuilder.Entity<TeamSeason>(entity =>
        {
            entity.ToTable("TeamSeason");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.LeagueId).HasColumnName("league_id");
            entity.Property(e => e.ConferenceId).HasColumnName("conference_id");
            entity.Property(e => e.DivisionId).HasColumnName("division_id");
            entity.Property(e => e.Games).HasColumnName("games");
            entity.Property(e => e.Wins).HasColumnName("wins");
            entity.Property(e => e.Losses).HasColumnName("losses");
            entity.Property(e => e.Ties).HasColumnName("ties");
            entity.Property(e => e.PointsFor).HasColumnName("points_for");
            entity.Property(e => e.PointsAgainst).HasColumnName("points_against");
            entity.Property(e => e.ExpectedWins)
                .HasColumnType("decimal(18, 16)")
                .HasColumnName("expected_wins");
            entity.Property(e => e.ExpectedLosses)
                .HasColumnType("decimal(18, 16)")
                .HasColumnName("expected_losses");
            entity.Property(e => e.OffensiveAverage)
                .HasColumnType("decimal(18, 15)")
                .HasColumnName("offensive_average");
            entity.Property(e => e.OffensiveFactor)
                .HasColumnType("decimal(18, 14)")
                .HasColumnName("offensive_factor");
            entity.Property(e => e.OffensiveIndex)
                .HasColumnType("decimal(18, 15)")
                .HasColumnName("offensive_index");
            entity.Property(e => e.DefensiveAverage)
                .HasColumnType("decimal(18, 15)")
                .HasColumnName("defensive_average");
            entity.Property(e => e.DefensiveFactor)
                .HasColumnType("decimal(18, 14)")
                .HasColumnName("defensive_factor");
            entity.Property(e => e.DefensiveIndex)
                .HasColumnType("decimal(18, 15)")
                .HasColumnName("defensive_index");
            entity.Property(e => e.FinalExpectedWinningPercentage)
                .HasColumnType("decimal(18, 17)")
                .HasColumnName("final_expected_winning_percentage");

            entity.HasOne(d => d.TeamIdNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeamSeason_Team_TeamId");

            entity.HasOne(d => d.SeasonIdNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.SeasonId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_TeamSeason_Season_SeasonId");

            entity.HasOne(d => d.LeagueIdNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.LeagueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeamSeason_League_LeagueId");

            entity.HasOne(d => d.ConferenceIdNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.ConferenceId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_TeamSeason_Conference_ConferenceId");

            entity.HasOne(d => d.DivisionIdNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_TeamSeason_Division_DivisionId");

            entity.HasIndex(e => e.TeamId, "IX_FK_TeamSeason_Team_TeamId");
            entity.HasIndex(e => e.SeasonId, "IX_FK_TeamSeason_Season_SeasonId");
            entity.HasIndex(e => e.LeagueId, "IX_FK_TeamSeason_League_LeagueId");
            entity.HasIndex(e => e.ConferenceId, "IX_FK_TeamSeason_Conference_ConferenceId");
            entity.HasIndex(e => e.DivisionId, "IX_FK_TeamSeason_Division_DivisionId");
            entity.HasIndex(e => new { e.TeamId, e.SeasonId }, "UQ_TeamSeason_LeagueId_SeasonId").IsUnique();
        });

        modelBuilder.Entity<TeamSeasonOpponentProfile>(entity =>
        {
            entity.ToTable("TeamSeasonScheduleProfile");

            entity.Property(e => e.Opponent)
                .HasColumnType("nvarchar(50)")
                .HasColumnName("opponent");

            entity.Property(e => e.GamePointsFor)
                .HasColumnType("int")
                .HasColumnName("game_points_for");

            entity.Property(e => e.GamePointsAgainst)
                .HasColumnType("int")
                .HasColumnName("game_points_against");

            entity.Property(e => e.OpponentWins)
                .HasColumnType("int")
                .HasColumnName("opponent_wins");

            entity.Property(e => e.OpponentLosses)
                .HasColumnType("int")
                .HasColumnName("opponent_losses");

            entity.Property(e => e.OpponentTies)
                .HasColumnType("int")
                .HasColumnName("opponent_ties");

            entity.Property(e => e.OpponentWinningPercentage)
                .HasColumnType("decimal(18,17)")
                .HasColumnName("opponent_winning_percentage");

            entity.Property(e => e.OpponentWeightedGames)
                .HasColumnType("int")
                .HasColumnName("opponent_weighted_games");

            entity.Property(e => e.OpponentWeightedPointsFor)
                .HasColumnType("int")
                .HasColumnName("opponent_weighted_points_for");

            entity.Property(e => e.OpponentWeightedPointsAgainst)
                .HasColumnType("int")
                .HasColumnName("opponent_weighted_points_against");

            entity.HasNoKey();
        });

        modelBuilder.Entity<TeamSeasonScheduleTotals>(entity =>
        {
            entity.ToTable("TeamSeasonScheduleTotals");

            entity.Property(e => e.Games)
                .HasColumnType("int")
                .HasColumnName("games");

            entity.Property(e => e.PointsFor)
                .HasColumnType("int")
                .HasColumnName("points_for");

            entity.Property(e => e.PointsAgainst)
                .HasColumnType("int")
                .HasColumnName("points_against");

            entity.Property(e => e.ScheduleWins)
                .HasColumnType("int")
                .HasColumnName("schedule_wins");

            entity.Property(e => e.ScheduleLosses)
                .HasColumnType("int")
                .HasColumnName("schedule_losses");

            entity.Property(e => e.ScheduleTies)
                .HasColumnType("int")
                .HasColumnName("schedule_ties");

            entity.Property(e => e.ScheduleWinningPercentage)
                .HasColumnType("decimal(18,17)")
                .HasColumnName("schedule_winning_percentage");

            entity.Property(e => e.ScheduleGames)
                .HasColumnType("int")
                .HasColumnName("schedule_games");

            entity.Property(e => e.SchedulePointsFor)
                .HasColumnType("int")
                .HasColumnName("schedule_points_for");

            entity.Property(e => e.SchedulePointsAgainst)
                .HasColumnType("int")
                .HasColumnName("schedule_points_against");

            entity.HasNoKey();
        });

        modelBuilder.Entity<TeamSeasonScheduleAverages>(entity =>
        {
            entity.ToTable("TeamSeasonScheduleAverages");

            entity.Property(e => e.PointsFor)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("avg_points_for");

            entity.Property(e => e.PointsAgainst)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("avg_points_against");

            entity.Property(e => e.SchedulePointsFor)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("avg_schedule_points_for");

            entity.Property(e => e.SchedulePointsAgainst)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("avg_schedule_points_against");

            entity.HasNoKey();
        });

        modelBuilder.Entity<LeagueSeasonTotals>(entity =>
        {
            entity.ToTable("LeagueSeasonTotals");

            entity.Property(e => e.TotalGames)
                .HasColumnType("int")
                .HasColumnName("total_games");

            entity.Property(e => e.TotalPoints)
                .HasColumnType("int")
                .HasColumnName("total_points");

            entity.Property(e => e.AveragePoints)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("average_points");

            entity.Property(e => e.WeekCount)
                .HasColumnType("int")
                .HasColumnName("week_count");

            entity.HasNoKey();
        });

        modelBuilder.Entity<SeasonTeamStanding>(entity =>
        {
            entity.ToTable("SeasonTeamStanding");

            entity.Property(e => e.Team)
                .HasColumnType("string")
                .HasColumnName("team");

            entity.Property(e => e.Wins)
                .HasColumnType("int")
                .HasColumnName("wins");

            entity.Property(e => e.Losses)
                .HasColumnType("int")
                .HasColumnName("losses");

            entity.Property(e => e.Ties)
                .HasColumnType("int")
                .HasColumnName("ties");

            entity.Property(e => e.WinningPercentage)
                .HasColumnType("decimal(18,17)")
                .HasColumnName("winning_percentage");

            entity.Property(e => e.PointsFor)
                .HasColumnType("int")
                .HasColumnName("points_for");

            entity.Property(e => e.PointsAgainst)
                .HasColumnType("int")
                .HasColumnName("points_against");

            entity.Property(e => e.AvgPointsFor)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("avg_points_for");

            entity.Property(e => e.AvgPointsAgainst)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("avg_points_against");

            entity.Property(e => e.ExpectedWins)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("expected_wins");

            entity.Property(e => e.ExpectedLosses)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("expected_losses");

            entity.HasNoKey();
        });

        modelBuilder.Entity<RankingsOffensiveTeamSeason>().HasNoKey();
        modelBuilder.Entity<RankingsDefensiveTeamSeason>().HasNoKey();
        modelBuilder.Entity<RankingsTotalTeamSeason>().HasNoKey();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
