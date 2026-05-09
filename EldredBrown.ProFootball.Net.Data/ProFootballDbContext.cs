using System;
using System.Collections.Generic;
using EldredBrown.ProFootball.Net.Data.Models;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Conference> Conferences { get; set; }

    public virtual DbSet<Division> Divisions { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<League> Leagues { get; set; }

    public virtual DbSet<LeagueSeason> LeagueSeasons { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ProFootballDb;Trusted_Connection=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Season>(entity =>
        {
            entity.ToTable("Season");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.NumOfWeeksScheduled).HasColumnName("num_of_weeks_scheduled");
            entity.Property(e => e.NumOfWeeksCompleted).HasColumnName("num_of_weeks_completed");

            entity.HasIndex(e => e.Year, "UQ_Season_Year").IsUnique();
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
            entity.Property(e => e.FirstSeasonYear).HasColumnName("first_season_year");
            entity.Property(e => e.LastSeasonYear).HasColumnName("last_season_year");

            entity.HasIndex(e => e.ShortName, "UQ_League_ShortName").IsUnique();

            entity.HasIndex(e => e.LongName, "UQ_League_LongName").IsUnique();

            entity.HasIndex(e => e.FirstSeasonYear, "IX_FK_League_Season_FirstSeasonYear");

            entity.HasIndex(e => e.LastSeasonYear, "IX_FK_League_Season_LastSeasonYear");

            entity.HasOne(d => d.FirstSeasonYearNavigation).WithMany(p => p.LeagueFirstSeasonYearNavigations)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.FirstSeasonYear)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_League_Season_FirstSeasonYear");

            entity.HasOne(d => d.LastSeasonYearNavigation).WithMany(p => p.LeagueLastSeasonYearNavigations)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.LastSeasonYear)
                .HasConstraintName("FK_League_Season_LastSeasonYear");
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
            entity.Property(e => e.LeagueName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("league_name");
            entity.Property(e => e.FirstSeasonYear).HasColumnName("first_season_year");
            entity.Property(e => e.LastSeasonYear).HasColumnName("last_season_year");

            entity.HasIndex(e => e.ShortName, "UQ_Conference_ShortName").IsUnique();

            entity.HasIndex(e => e.LongName, "UQ_Conference_LongName").IsUnique();

            entity.HasIndex(e => e.LeagueName, "IX_FK_Conference_League_LeagueName");

            entity.HasIndex(e => e.FirstSeasonYear, "IX_FK_Conference_Season_FirstSeasonYear");

            entity.HasIndex(e => e.LastSeasonYear, "IX_FK_Conference_Season_LastSeasonYear");

            entity.HasOne(d => d.LeagueNameNavigation).WithMany(p => p.Conferences)
                .HasPrincipalKey(p => p.ShortName)
                .HasForeignKey(d => d.LeagueName)
                .HasConstraintName("FK_Conference_League_LeagueName");

            entity.HasOne(d => d.FirstSeasonYearNavigation).WithMany(p => p.ConferenceFirstSeasonYearNavigations)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.FirstSeasonYear)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Conference_Season_FirstSeasonYear");

            entity.HasOne(d => d.LastSeasonYearNavigation).WithMany(p => p.ConferenceLastSeasonYearNavigations)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.LastSeasonYear)
                .HasConstraintName("FK_Conference_Season_LastSeasonYear");
        });

        modelBuilder.Entity<Division>(entity =>
        {
            entity.ToTable("Division");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.LeagueName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("league_name");
            entity.Property(e => e.ConferenceName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("conference_name");
            entity.Property(e => e.FirstSeasonYear).HasColumnName("first_season_year");
            entity.Property(e => e.LastSeasonYear).HasColumnName("last_season_year");

            entity.HasIndex(e => e.Name, "UQ_Division_Name").IsUnique();

            entity.HasIndex(e => e.LeagueName, "IX_FK_Division_League_LeagueName");

            entity.HasIndex(e => e.ConferenceName, "IX_FK_Division_Conference_ConferenceName");

            entity.HasIndex(e => e.FirstSeasonYear, "IX_FK_Division_Season_FirstSeasonYear");

            entity.HasIndex(e => e.LastSeasonYear, "IX_FK_Division_Season_LastSeasonYear");

            entity.HasOne(d => d.LeagueNameNavigation).WithMany(p => p.Divisions)
                .HasPrincipalKey(p => p.ShortName)
                .HasForeignKey(d => d.LeagueName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Division_League_LeagueName");

            entity.HasOne(d => d.ConferenceNameNavigation).WithMany(p => p.Divisions)
                .HasPrincipalKey(p => p.ShortName)
                .HasForeignKey(d => d.ConferenceName)
                .HasConstraintName("FK_Division_Conference_ConferenceName");

            entity.HasOne(d => d.FirstSeasonYearNavigation).WithMany(p => p.DivisionFirstSeasonYearNavigations)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.FirstSeasonYear)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Division_Season_FirstSeasonYear");

            entity.HasOne(d => d.LastSeasonYearNavigation).WithMany(p => p.DivisionLastSeasonYearNavigations)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.LastSeasonYear)
                .HasConstraintName("FK_Division_Season_LastSeasonYear");
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
            entity.Property(e => e.SeasonYear).HasColumnName("season_year");
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

            entity.HasIndex(e => e.SeasonYear, "IX_FK_Game_Season_SeasonYear");

            entity.HasIndex(e => new { e.SeasonYear, e.Week, e.GuestName, e.HostName }, "UQ_Game_Season_Week_Teams").IsUnique();

            entity.HasOne(d => d.SeasonYearNavigation).WithMany(p => p.Games)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.SeasonYear)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Game_Season_SeasonYear");
        });

        modelBuilder.Entity<LeagueSeason>(entity =>
        {
            entity.ToTable("LeagueSeason");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LeagueName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("league_name");
            entity.Property(e => e.SeasonYear).HasColumnName("season_year");
            entity.Property(e => e.TotalGames).HasColumnName("total_games");
            entity.Property(e => e.TotalPoints).HasColumnName("total_points");
            entity.Property(e => e.AveragePoints)
                .HasColumnType("decimal(18, 16)")
                .HasColumnName("average_points");

            entity.HasIndex(e => e.LeagueName, "IX_FK_LeagueSeason_League_LeagueName");

            entity.HasIndex(e => e.SeasonYear, "IX_FK_LeagueSeason_Season_SeasonYear");

            entity.HasIndex(e => new { e.LeagueName, e.SeasonYear }, "UQ_LeagueSeason_LeagueId_SeasonId").IsUnique();

            entity.HasOne(d => d.LeagueNameNavigation).WithMany(p => p.LeagueSeasons)
                .HasPrincipalKey(p => p.ShortName)
                .HasForeignKey(d => d.LeagueName)
                .HasConstraintName("FK_LeagueSeason_League_LeagueName");

            entity.HasOne(d => d.SeasonYearNavigation).WithMany(p => p.LeagueSeasons)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.SeasonYear)
                .HasConstraintName("FK_LeagueSeason_Season_SeasonYear");
        });

        modelBuilder.Entity<TeamSeason>(entity =>
        {
            entity.ToTable("TeamSeason");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TeamName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("team_name");
            entity.Property(e => e.SeasonYear).HasColumnName("season_year");
            entity.Property(e => e.LeagueName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("league_name");
            entity.Property(e => e.ConferenceName)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("conference_name");
            entity.Property(e => e.DivisionName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("division_name");
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

            entity.HasIndex(e => e.TeamName, "IX_FK_TeamSeason_Team_TeamName");

            entity.HasIndex(e => e.SeasonYear, "IX_FK_TeamSeason_Season_SeasonId");

            entity.HasIndex(e => e.LeagueName, "IX_FK_TeamSeason_League_LeagueName");

            entity.HasIndex(e => e.ConferenceName, "IX_FK_TeamSeason_Conference_ConferenceName");

            entity.HasIndex(e => e.DivisionName, "IX_FK_TeamSeason_Division_DivisionName");

            entity.HasIndex(e => new { e.TeamName, e.SeasonYear }, "UQ_TeamSeason_LeagueId_SeasonId").IsUnique();

            entity.HasOne(d => d.TeamNameNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.Name)
                .HasForeignKey(d => d.TeamName)
                .HasConstraintName("FK_TeamSeason_Team_TeamName");

            entity.HasOne(d => d.SeasonYearNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.Year)
                .HasForeignKey(d => d.SeasonYear)
                .HasConstraintName("FK_TeamSeason_Season_SeasonYear");

            entity.HasOne(d => d.LeagueNameNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.ShortName)
                .HasForeignKey(d => d.LeagueName)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeamSeason_League_LeagueName");

            entity.HasOne(d => d.ConferenceNameNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.ShortName)
                .HasForeignKey(d => d.ConferenceName)
                .HasConstraintName("FK_TeamSeason_Conference_ConferenceName");

            entity.HasOne(d => d.DivisionNameNavigation).WithMany(p => p.TeamSeasons)
                .HasPrincipalKey(p => p.Name)
                .HasForeignKey(d => d.DivisionName)
                .HasConstraintName("FK_TeamSeason_Division_DivisionName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
