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

    public virtual DbSet<Association> Associations { get; set; }

    public virtual DbSet<Game> Games { get; set; }

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
    public virtual DbSet<StandingsTeamSeason>? SeasonStandings { get; set; }

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
    /// Gets or sets the AssociationSeasonTotals data source.
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

            entity.Property(e => e.Year).HasColumnName("year");
            entity.HasKey(e => e.Year);
        });

        modelBuilder.Entity<Association>(entity =>
        {
            entity.ToTable("Association");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LongName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("long_name");
            entity.Property(e => e.ShortName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("short_name");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.FirstSeasonYear).HasColumnName("first_season_year");
            entity.Property(e => e.LastSeasonYear).HasColumnName("last_season_year");

            entity.HasOne(d => d.ParentIdNavigation).WithMany(p => p.ChildAssociations)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Association_Association_ParentId");

            entity.HasOne(d => d.FirstSeasonYearNavigation).WithMany(p => p.AssociationsFirstSeasonOf)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.FirstSeasonYear)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Association_Season_FirstSeasonYear");

            entity.HasOne(d => d.LastSeasonYearNavigation).WithMany(p => p.AssociationsLastSeasonOf)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.LastSeasonYear)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Association_Season_LastSeasonYear");

            entity.HasIndex(e => e.ShortName, "UQ_Association_ShortName").IsUnique();
            entity.HasIndex(e => e.LongName, "UQ_Association_LongName").IsUnique();
            entity.HasIndex(e => e.ParentId, "IX_FK_Association_Association_ParentId");
            entity.HasIndex(e => e.FirstSeasonYear, "IX_FK_Association_Season_FirstSeasonYear");
            entity.HasIndex(e => e.LastSeasonYear, "IX_FK_Association_Season_LastSeasonYear");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("Team");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasIndex(e => e.Name, "UQ_Team_Name").IsUnique();
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("Game");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SeasonYear).HasColumnName("season_year");
            entity.Property(e => e.LeagueId).HasColumnName("league_id");
            entity.Property(e => e.Week).HasColumnName("week");
            entity.Property(e => e.GuestName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("guest_name");
            entity.Property(e => e.GuestScore).HasColumnName("guest_score");
            entity.Property(e => e.HostName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("host_name");
            entity.Property(e => e.HostScore).HasColumnName("host_score");
            entity.Property(e => e.IsPlayoff).HasColumnName("is_playoff");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("notes");

            entity.HasOne(d => d.SeasonYearNavigation).WithMany(p => p.Games)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.SeasonYear)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Game_Season_SeasonYear");

            entity.HasOne(d => d.LeagueIdNavigation).WithMany(p => p.Games)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.LeagueId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Game_Association_LeagueId");

            entity.HasIndex(e => e.SeasonYear, "IX_FK_Game_Season_SeasonYear");
            entity.HasIndex(e => e.LeagueId, "IX_FK_Game_Association_LeagueId");
            entity.HasIndex(e => new { e.SeasonYear, e.Week, e.GuestName, e.HostName }, "UQ_Game_Season_Week_Teams").IsUnique();
        });

        modelBuilder.Entity<LeagueSeason>(entity =>
        {
            entity.ToTable("LeagueSeason");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LeagueId).HasColumnName("league_id");
            entity.Property(e => e.SeasonYear).HasColumnName("season_year");
            entity.Property(e => e.NumOfWeeksScheduled).HasColumnName("num_of_weeks_scheduled");
            entity.Property(e => e.NumOfWeeksCompleted).HasColumnName("num_of_weeks_completed");
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

            entity.HasOne(d => d.SeasonYearNavigation).WithMany(p => p.LeagueSeasons)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.SeasonYear)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_LeagueSeason_Season_SeasonYear");

            entity.HasIndex(e => e.LeagueId, "IX_FK_LeagueSeason_League_LeagueId");
            entity.HasIndex(e => e.SeasonYear, "IX_FK_LeagueSeason_Season_SeasonYear");
            entity.HasIndex(e => new { e.LeagueId, e.SeasonYear }, "UQ_LeagueSeason_LeagueId_SeasonYear").IsUnique();
        });

        modelBuilder.Entity<TeamSeason>(entity =>
        {
            entity.ToTable("TeamSeason");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.SeasonYear).HasColumnName("season_year");
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

            entity.HasOne(d => d.SeasonYearNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.SeasonYear)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_TeamSeason_Season_SeasonYear");

            entity.HasOne(d => d.LeagueIdNavigation).WithMany(p => p.TeamSeasonsLeagueOf)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.LeagueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeamSeason_Association_LeagueId");

            entity.HasOne(d => d.ConferenceIdNavigation).WithMany(p => p.TeamSeasonsConferenceOf)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.ConferenceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeamSeason_Association_ConferenceId");

            entity.HasOne(d => d.DivisionIdNavigation).WithMany(p => p.TeamSeasonsDivisionOf)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TeamSeason_Association_DivisionId");

            entity.HasIndex(e => e.TeamId, "IX_FK_TeamSeason_Team_TeamId");
            entity.HasIndex(e => e.SeasonYear, "IX_FK_TeamSeason_Season_SeasonYear");
            entity.HasIndex(e => e.LeagueId, "IX_FK_TeamSeason_Association_LeagueId");
            entity.HasIndex(e => e.ConferenceId, "IX_FK_TeamSeason_Association_ConferenceId");
            entity.HasIndex(e => e.DivisionId, "IX_FK_TeamSeason_Association_DivisionId");
            entity.HasIndex(e => new { e.TeamId, e.SeasonYear }, "UQ_TeamSeason_AssociationId_SeasonYear").IsUnique();
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

        modelBuilder.Entity<StandingsTeamSeason>(entity =>
        {
            entity.ToTable("SeasonStandings");

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

        modelBuilder.Entity<RankingsOffensiveTeamSeason>(entity =>
        {
            entity.ToTable("OffensiveRankings");

            entity.Property(e => e.TeamName)
                .HasColumnType("string")
                .HasColumnName("team_name");

            entity.Property(e => e.Wins)
                .HasColumnType("int")
                .HasColumnName("wins");

            entity.Property(e => e.Losses)
                .HasColumnType("int")
                .HasColumnName("losses");

            entity.Property(e => e.Ties)
                .HasColumnType("int")
                .HasColumnName("ties");

            entity.Property(e => e.OffensiveAverage)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("offensive_average");

            entity.Property(e => e.OffensiveFactor)
                .HasColumnType("decimal(18,15)")
                .HasColumnName("offensive_factor");

            entity.Property(e => e.OffensiveIndex)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("offensive_index");

            entity.Property(e => e.DefensiveAverage)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("defensive_average");

            entity.Property(e => e.DefensiveFactor)
                .HasColumnType("decimal(18,15)")
                .HasColumnName("defensive_factor");

            entity.Property(e => e.DefensiveIndex)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("defensive_index");

            entity.Property(e => e.FinalExpectedWinningPercentage)
                .HasColumnType("decimal(18,17)")
                .HasColumnName("final_expected_winning_percentage");

            entity.HasNoKey();
        });

        modelBuilder.Entity<RankingsDefensiveTeamSeason>(entity =>
        {
            entity.ToTable("DefensiveRankings");

            entity.Property(e => e.TeamName)
                .HasColumnType("string")
                .HasColumnName("team_name");

            entity.Property(e => e.Wins)
                .HasColumnType("int")
                .HasColumnName("wins");

            entity.Property(e => e.Losses)
                .HasColumnType("int")
                .HasColumnName("losses");

            entity.Property(e => e.Ties)
                .HasColumnType("int")
                .HasColumnName("ties");

            entity.Property(e => e.OffensiveAverage)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("offensive_average");

            entity.Property(e => e.OffensiveFactor)
                .HasColumnType("decimal(18,15)")
                .HasColumnName("offensive_factor");

            entity.Property(e => e.OffensiveIndex)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("offensive_index");

            entity.Property(e => e.DefensiveAverage)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("defensive_average");

            entity.Property(e => e.DefensiveFactor)
                .HasColumnType("decimal(18,15)")
                .HasColumnName("defensive_factor");

            entity.Property(e => e.DefensiveIndex)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("defensive_index");

            entity.Property(e => e.FinalExpectedWinningPercentage)
                .HasColumnType("decimal(18,17)")
                .HasColumnName("final_expected_winning_percentage");

            entity.HasNoKey();
        });

        modelBuilder.Entity<RankingsTotalTeamSeason>(entity =>
        {
            entity.ToTable("TotalRankings");

            entity.Property(e => e.TeamName)
                .HasColumnType("string")
                .HasColumnName("team_name");

            entity.Property(e => e.Wins)
                .HasColumnType("int")
                .HasColumnName("wins");

            entity.Property(e => e.Losses)
                .HasColumnType("int")
                .HasColumnName("losses");

            entity.Property(e => e.Ties)
                .HasColumnType("int")
                .HasColumnName("ties");

            entity.Property(e => e.OffensiveAverage)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("offensive_average");

            entity.Property(e => e.OffensiveFactor)
                .HasColumnType("decimal(18,15)")
                .HasColumnName("offensive_factor");

            entity.Property(e => e.OffensiveIndex)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("offensive_index");

            entity.Property(e => e.DefensiveAverage)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("defensive_average");

            entity.Property(e => e.DefensiveFactor)
                .HasColumnType("decimal(18,15)")
                .HasColumnName("defensive_factor");

            entity.Property(e => e.DefensiveIndex)
                .HasColumnType("decimal(18,16)")
                .HasColumnName("defensive_index");

            entity.Property(e => e.FinalExpectedWinningPercentage)
                .HasColumnType("decimal(18,17)")
                .HasColumnName("final_expected_winning_percentage");

            entity.HasNoKey();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
