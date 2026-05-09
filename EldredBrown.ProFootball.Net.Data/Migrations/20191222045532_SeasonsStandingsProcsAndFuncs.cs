using Microsoft.EntityFrameworkCore.Migrations;

namespace EldredBrown.ProFootball.Net.Data.Migrations
{
    public partial class SeasonsStandingsProcsAndFuncs : Migration
    {
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			CreateSpGetSeasonStandings(migrationBuilder);
			CreateSpGetSeasonStandingsForLeague(migrationBuilder);
			CreateSpGetSeasonStandingsForConference(migrationBuilder);
			CreateSpGetSeasonStandingsForDivision(migrationBuilder);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE sp_GetSeasonStandingsForDivision");
            migrationBuilder.Sql("DROP PROCEDURE sp_GetSeasonStandingsForConference");
            migrationBuilder.Sql("DROP PROCEDURE sp_GetSeasonStandingsForLeague");
            migrationBuilder.Sql("DROP PROCEDURE sp_GetSeasonStandings");
        }

        private void CreateSpGetSeasonStandings(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetSeasonStandings
	@season_year int,
	@group_by_division bit
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN
		SELECT
			team_name as team,
			conference_name as conference,
			division_name as division,
			wins,
			losses,
			ties,
			winning_percentage = 
				CASE
					WHEN games = 0 THEN NULL
					ELSE ((2 * CAST(wins as decimal(18,0)) + CAST(ties as decimal(18,0))) / (2 * CAST(games as decimal(18,0))))
				END,
			points_for,
			points_against,
			avg_points_for =
				CASE
					WHEN games = 0 THEN NULL
					ELSE (CAST(points_for as decimal(18,0)) / CAST(games as decimal(18,0)))
				END,
			avg_points_against =
				CASE
					WHEN games = 0 THEN NULL
					ELSE (CAST(points_against as decimal(18,0)) / CAST(games as decimal(18,0)))
				END,
			expected_wins,
			expected_losses
		FROM dbo.TeamSeason AS ts
		WHERE season_year = @season_year
		ORDER BY
			CASE
				WHEN @group_by_division = 0 THEN conference_name
				WHEN @group_by_division = 1 THEN division_name
			END,
			winning_percentage DESC,
			wins DESC,
			losses ASC,
			expected_wins DESC,
			expected_losses ASC,
			team_name ASC

	END
END
GO");
        }

        private void CreateSpGetSeasonStandingsForLeague(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetSeasonStandingsForLeague
	@season_year int,
	@league_name varchar(5)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
		SELECT year FROM dbo.Season WHERE year = @season_year
	)
	AND EXISTS (
		SELECT short_name FROM dbo.League WHERE short_name = @league_name
	)
	BEGIN
		SELECT
			team_name as team,
			wins,
			losses,
			ties,
			winning_percentage = 
				CASE
					WHEN games = 0 THEN NULL
					ELSE ((2 * CAST(wins as decimal(18,0)) + CAST(ties as decimal(18,0))) / (2 * CAST(games as decimal(18,0))))
				END,
			points_for,
			points_against,
			avg_points_for =
				CASE
					WHEN games = 0 THEN NULL
					ELSE (CAST(points_for as decimal(18,0)) / CAST(games as decimal(18,0)))
				END,
			avg_points_against =
				CASE
					WHEN games = 0 THEN NULL
					ELSE (CAST(points_against as decimal(18,0)) / CAST(games as decimal(18,0)))
				END
		FROM
			dbo.TeamSeason as ts
		WHERE
			season_year = @season_year
			AND
			league_name = @league_name
		ORDER BY
			winning_percentage DESC,
			wins DESC,
			losses ASC,
			team_name ASC

	END
END
GO");
        }

        private void CreateSpGetSeasonStandingsForConference(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetSeasonStandingsForConference
	@season_year int,
	@conference_name varchar(5)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
		SELECT year FROM dbo.Season WHERE year = @season_year
	)
	AND EXISTS (
		SELECT short_name FROM dbo.Conference WHERE short_name = @conference_name
	)
	BEGIN
		SELECT
			team_name as team,
			wins,
			losses,
			ties,
			winning_percentage = 
				CASE
					WHEN games = 0 THEN NULL
					ELSE ((2 * CAST(wins as decimal(18,0)) + CAST(ties as decimal(18,0))) / (2 * CAST(games as decimal(18,0))))
				END,
			points_for,
			points_against,
			avg_points_for =
				CASE
					WHEN games = 0 THEN NULL
					ELSE (CAST(points_for as decimal(18,0)) / CAST(games as decimal(18,0)))
				END,
			avg_points_against =
				CASE
					WHEN games = 0 THEN NULL
					ELSE (CAST(points_against as decimal(18,0)) / CAST(games as decimal(18,0)))
				END
		FROM
			dbo.TeamSeason as ts
		WHERE
			season_year = @season_year
			AND
			conference_name = @conference_name
		ORDER BY
			winning_percentage DESC,
			wins DESC,
			losses ASC,
			team_name ASC

	END
END
GO");
        }

        private void CreateSpGetSeasonStandingsForDivision(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetSeasonStandingsForDivision
	@season_year int,
	@division_name varchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
		SELECT year FROM dbo.Season WHERE year = @season_year
	)
	AND EXISTS (
		SELECT name FROM dbo.Division WHERE name = @division_name
	)
	BEGIN
		SELECT
			team_name as team,
			wins,
			losses,
			ties,
			winning_percentage = 
				CASE
					WHEN games = 0 THEN NULL
					ELSE ((2 * CAST(wins as decimal(18,0)) + CAST(ties as decimal(18,0))) / (2 * CAST(games as decimal(18,0))))
				END,
			points_for,
			points_against,
			avg_points_for =
				CASE
					WHEN games = 0 THEN NULL
					ELSE (CAST(points_for as decimal(18,0)) / CAST(games as decimal(18,0)))
				END,
			avg_points_against =
				CASE
					WHEN games = 0 THEN NULL
					ELSE (CAST(points_against as decimal(18,0)) / CAST(games as decimal(18,0)))
				END
		FROM
			dbo.TeamSeason as ts
		WHERE
			season_year = @season_year
			AND
			division_name = @division_name
		ORDER BY
			winning_percentage DESC,
			wins DESC,
			losses ASC,
			team_name ASC

	END
END
GO");
        }
    }
}
