using Microsoft.EntityFrameworkCore.Migrations;

namespace EldredBrown.ProFootball.Net.Data.Migrations
{
	public partial class TeamSeasonsProcsAndFuncs : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			CreateFnGetTeamSeasonGames(migrationBuilder);
			CreateFnGetTeamSeasonScheduleData(migrationBuilder);
            CreateFnGetTeamSeasonScheduleProfile(migrationBuilder);
            CreateFnGetTeamSeasonScheduleTotals(migrationBuilder);
            CreateSpGetLeagueSeasonTotals(migrationBuilder);
            CreateSpGetTeamSeasonScheduleProfile(migrationBuilder);
            CreateSpGetTeamSeasonScheduleTotals(migrationBuilder);
            CreateSpGetTeamSeasonScheduleAverages(migrationBuilder);
            CreateSpGetRankingsOffensive(migrationBuilder);
            CreateSpGetRankingsDefensive(migrationBuilder);
            CreateSpGetRankingsTotal(migrationBuilder);
            CreateSpGetDataForRankingsUpdate(migrationBuilder);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP PROCEDURE sp_GetDataForRankingsUpdate");
			migrationBuilder.Sql("DROP PROCEDURE sp_GetRankingsTotal");
			migrationBuilder.Sql("DROP PROCEDURE sp_GetRankingsDefensive");
			migrationBuilder.Sql("DROP PROCEDURE sp_GetRankingsOffensive");
            migrationBuilder.Sql("DROP PROCEDURE sp_GetTeamSeasonScheduleAverages");
            migrationBuilder.Sql("DROP PROCEDURE sp_GetTeamSeasonScheduleTotals");
            migrationBuilder.Sql("DROP PROCEDURE sp_GetTeamSeasonScheduleProfile");
            migrationBuilder.Sql("DROP PROCEDURE sp_SpGetLeagueSeasonTotals");

            migrationBuilder.Sql("DROP FUNCTION fn_GetTeamSeasonScheduleTotals");
			migrationBuilder.Sql("DROP FUNCTION fn_GetTeamSeasonScheduleProfile");
			migrationBuilder.Sql("DROP FUNCTION fn_GetTeamSeasonScheduleData");
			migrationBuilder.Sql("DROP FUNCTION fn_GetTeamSeasonGames");
		}

		private void CreateFnGetTeamSeasonGames(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION dbo.fn_GetTeamSeasonGames
(	
	@team_name varchar(50),
	@season_year int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		game.id,
		season_year AS season,
		host_name AS opponent,
		guest_score AS points_for,
		host_score AS points_against
	FROM
		dbo.Game as game
	WHERE guest_name = @team_name AND season_year = @season_year
	UNION
	SELECT
		game.id,
		season_year AS season,
		guest_name AS opponent,
		host_score AS points_for,
		guest_score AS points_against
	FROM
		dbo.Game as game
	WHERE host_name = @team_name AND season_year = @season_year
)
GO");
		}

        private void CreateFnGetTeamSeasonScheduleData(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION dbo.fn_GetTeamSeasonScheduleData 
(	
	@team_name varchar(50),
	@season_year int
)
RETURNS @tbl TABLE
(
	id						int,
	opponent				varchar(50),
	wins					int,
	losses					int,
	ties					int,
	winning_percentage		decimal(18,17),
	weighted_games			int,
	weighted_points_for		int,
	weighted_points_against	int
)
AS
BEGIN
	BEGIN
		INSERT @tbl

		SELECT
			tsg.id,
			tsg.opponent AS opponent,
			ts.wins AS wins,
			ts.losses AS losses,
			ts.ties AS ties,
			winning_percentage = 
				CASE
					WHEN games = 0 THEN NULL
					ELSE ((2 * CAST(wins as decimal(18,0)) + CAST(ties as decimal(18,0))) / (2 * CAST(games as decimal(18,0))))
				END,
			(ts.games - 1) AS weighted_games,
			(ts.points_for - tsg.points_against) AS weighted_points_for,
			(ts.points_against - tsg.points_for) AS weighted_points_against
		FROM dbo.TeamSeason AS ts
			INNER JOIN dbo.fn_GetTeamSeasonGames(@team_name, @season_year) AS tsg
				ON team_name = tsg.opponent
		WHERE
			ts.season_year = @season_year
	END

	RETURN
END
GO");
        }

        private void CreateFnGetTeamSeasonScheduleProfile(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION dbo.fn_GetTeamSeasonScheduleProfile 
(	
	@team_name varchar(50),
	@season_year int
)
RETURNS @tbl TABLE
(
	id									int,
	opponent							varchar(50),
	game_points_for						int,
	game_points_against					int,
	opponent_wins						int,
	opponent_losses						int,
	opponent_ties						int,
	opponent_winning_percentage			decimal(18,17),
	opponent_weighted_games				int,
	opponent_weighted_points_for		int,
	opponent_weighted_points_against	int
)
AS
BEGIN
	-- Add the SELECT statement with parameter references here
	INSERT @tbl

	SELECT
		tsg.id,
		tsg.opponent,
		tsg.points_for,
		tsg.points_against,
		tssd.wins,
		tssd.losses,
		tssd.ties,
		tssd.winning_percentage,
		tssd.weighted_games,
		tssd.weighted_points_for,
		tssd.weighted_points_against
	FROM dbo.fn_GetTeamSeasonGames(@team_name, @season_year) AS tsg
		INNER JOIN dbo.fn_GetTeamSeasonScheduleData(@team_name, @season_year) AS tssd
			ON tsg.id = tssd.id

	RETURN
END
GO");
		}

		private void CreateFnGetTeamSeasonScheduleTotals(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION dbo.fn_GetTeamSeasonScheduleTotals
(	
	@team_name varchar(50),
	@season_year int
)
RETURNS @tbl TABLE
(
	games						int,
	points_for					int,
	points_against				int,
	schedule_wins				int,
	schedule_losses				int,
	schedule_ties				int,
	schedule_winning_percentage	decimal(18,17),
	schedule_games				int,
	schedule_points_for			int,
	schedule_points_against		int
)
AS
BEGIN
	BEGIN
		INSERT @tbl

		SELECT
			COUNT(tssp.opponent) AS games,
			SUM(tssp.game_points_for) AS points_for,
			SUM(tssp.game_points_against) AS points_against,
			SUM(tssd.wins) AS schedule_wins,
			SUM(tssd.losses) AS schedule_losses,
			SUM(tssd.ties) AS schedule_ties,
			schedule_winning_percentage = 
				CASE		-- Prevent division by zero.
					WHEN (SUM(tssd.wins) + SUM(tssd.losses) + SUM(tssd.ties)) = 0
						THEN NULL
					ELSE (
						CAST(2 * SUM(tssd.wins) + SUM(tssd.ties) as decimal) / 
						(2 * (SUM(tssd.wins) + SUM(tssd.losses) + SUM(tssd.ties)))
					)
				END,
			SUM(tssd.weighted_games) AS schedule_games,
			SUM(tssd.weighted_points_for) AS schedule_points_for,
			SUM(tssd.weighted_points_against) AS schedule_points_against
		FROM dbo.fn_GetTeamSeasonScheduleProfile(@team_name, @season_year) AS tssp
			INNER JOIN dbo.fn_GetTeamSeasonScheduleData(@team_name, @season_year) AS tssd
				ON tssp.opponent = tssd.opponent
	END

	RETURN
END
GO");
		}

        private void CreateSpGetLeagueSeasonTotals(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetLeagueSeasonTotals
	@league_name varchar(50),
	@season_year int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		SUM(games) AS total_games,
		SUM(points_for) AS total_points,
		average_points =
			CASE
				WHEN SUM(games) = 0 THEN NULL
				ELSE ROUND(CAST(SUM(points_for) as float) / SUM(games), 2)
			END,
		ROUND(AVG(games), 0) AS week_count
	FROM
		dbo.TeamSeason
	WHERE
		league_name = @league_name
		AND
		season_year = @season_year
END
GO");
        }

        private void CreateSpGetTeamSeasonScheduleProfile(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetTeamSeasonScheduleProfile
	@team_name varchar(50),
	@season_year int
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
		SELECT name FROM dbo.Team WHERE name = @team_name
	)
	AND EXISTS (
		SELECT year FROM dbo.Season WHERE year = @season_year
	)
	BEGIN
		SELECT
			opponent,
			game_points_for,
			game_points_against,
			opponent_wins,
			opponent_losses,
			opponent_ties,
			opponent_winning_percentage,
			opponent_weighted_games,
			opponent_weighted_points_for,
			opponent_weighted_points_against
		FROM dbo.fn_GetTeamSeasonScheduleProfile(@team_name, @season_year)
	END
END
GO");
        }

        private void CreateSpGetTeamSeasonScheduleTotals(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetTeamSeasonScheduleTotals
	@team_name varchar(50),
	@season_year int
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
		SELECT name FROM dbo.Team WHERE name = @team_name
	)
	AND EXISTS (
		SELECT year FROM dbo.Season WHERE year = @season_year
	)
	BEGIN
		SELECT
			games,
			points_for,
			points_against,
			schedule_wins,
			schedule_losses,
			schedule_ties,
			schedule_winning_percentage,
			schedule_games,
			schedule_points_for,
			schedule_points_against
		FROM dbo.fn_GetTeamSeasonScheduleTotals(@team_name, @season_year)

	END
END
GO");
        }

        private void CreateSpGetTeamSeasonScheduleAverages(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetTeamSeasonScheduleAverages
	@team_name varchar(50),
	@season_year int
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
		SELECT name FROM dbo.Team WHERE name = @team_name
	)
	AND EXISTS (
		SELECT year FROM dbo.Season WHERE year = @season_year
	)
	BEGIN
		SELECT
			avg_points_for = 
				CASE
					WHEN games = 0 THEN NULL
					ELSE CAST(points_for as decimal(18,0)) / CAST(games as decimal(18,0))
				END,
			avg_points_against = 
				CASE
					WHEN games = 0 THEN NULL
					ELSE CAST(points_against as decimal(18,0)) / CAST(games as decimal(18,0))
				END,
			avg_schedule_points_for = 
				CASE
					WHEN schedule_games = 0 THEN NULL
					ELSE CAST(schedule_points_for as decimal(18,0)) / CAST(schedule_games as decimal(18,0))
				END,
			avg_schedule_points_against = 
				CASE
					WHEN schedule_games = 0 THEN NULL
					ELSE CAST(schedule_points_against as decimal(18,0)) / CAST(schedule_games as decimal(18,0))
				END
		FROM
			dbo.fn_GetTeamSeasonScheduleTotals(@team_name, @season_year)

	END
END
GO");
        }

        private void CreateSpGetRankingsOffensive(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetRankingsOffensive
	@season_year int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		team_name AS team,
		wins,
		losses,
		ties,
		offensive_average,
		offensive_factor,
		offensive_index
	FROM
		dbo.TeamSeason AS ts
	WHERE
		season_year = @season_year
	ORDER BY
		offensive_index DESC
END
GO");
        }

        private void CreateSpGetRankingsDefensive(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetRankingsDefensive
	@season_year int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		team_name AS team,
		wins,
		losses,
		ties,
		defensive_average,
		defensive_factor,
		defensive_index
	FROM
		dbo.TeamSeason AS ts
	WHERE
		season_year = @season_year
	ORDER BY
		defensive_index ASC
END
GO");
        }

        private void CreateSpGetRankingsTotal(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE dbo.sp_GetRankingsTotal
	@season_year int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		team_name AS team,
		wins,
		losses,
		ties,
		offensive_average,
		offensive_factor,
		offensive_index,
		defensive_average,
		defensive_factor,
		defensive_index,
		final_expected_winning_percentage
	FROM
		dbo.TeamSeason AS ts
	WHERE
		season_year = @season_year
	ORDER BY
		final_expected_winning_percentage DESC

END
GO");
        }

        private void CreateSpGetDataForRankingsUpdate(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetDataForRankingsUpdate]
	@team_name varchar(50),
	@league_name varchar(50),
	@season_year int
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
		SELECT name FROM dbo.Team WHERE name = @team_name
	)
	AND EXISTS (
		SELECT short_name FROM dbo.League WHERE short_name = @league_name
	)
	AND EXISTS (
		SELECT year FROM dbo.Season WHERE year = @season_year
	)
	BEGIN
		SELECT
			games,
			points_for,
			points_against,
			schedule_wins,
			schedule_losses,
			schedule_ties,
			schedule_winning_percentage,
			schedule_games,
			schedule_points_for,
			schedule_points_against
		FROM dbo.fn_GetTeamSeasonScheduleTotals(@team_name, @season_year)

		SELECT
			avg_points_for = 
				CASE
					WHEN games = 0 THEN NULL
					ELSE CAST(points_for as decimal(18,0)) / CAST(games as decimal(18,0))
				END,
			avg_points_against = 
				CASE
					WHEN games = 0 THEN NULL
					ELSE CAST(points_against as decimal(18,0)) / CAST(games as decimal(18,0))
				END,
			avg_schedule_points_for = 
				CASE
					WHEN schedule_games = 0 THEN NULL
					ELSE CAST(schedule_points_for as decimal(18,0)) / CAST(schedule_games as decimal(18,0))
				END,
			avg_schedule_points_against = 
				CASE
					WHEN schedule_games = 0 THEN NULL
					ELSE CAST(schedule_points_against as decimal(18,0)) / CAST(schedule_games as decimal(18,0))
				END
		FROM
			dbo.fn_GetTeamSeasonScheduleTotals(@team_name, @season_year)

		SELECT
			id,
			league_name,
			season_year,
			total_games,
			total_points,
			average_points
		FROM dbo.LeagueSeason
		WHERE league_name = @league_name AND season_year = @season_year

	END
END
GO");
        }
	}
}
