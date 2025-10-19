using Microsoft.EntityFrameworkCore.Migrations;

namespace EldredBrown.ProFootball.NETCore.Data.Migrations
{
	public partial class TeamSeasonsProcsAndFuncs : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			CreateFnGetGamesByTeamAndSeason(migrationBuilder);
			CreateFnGetTeamSeasonOpponentProfiles(migrationBuilder);
			CreateFnGetTeamSeasonScheduleProfile(migrationBuilder);
			CreateFnGetTeamSeasonScheduleTotals(migrationBuilder);
			CreateSpGetTeamSeasonScheduleProfile(migrationBuilder);
			CreateSpGetTeamSeasonScheduleTotals(migrationBuilder);
			CreateSpGetTeamSeasonScheduleAverages(migrationBuilder);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP PROCEDURE sp_GetTeamSeasonScheduleAverages");
			migrationBuilder.Sql("DROP PROCEDURE sp_GetTeamSeasonScheduleTotals");
			migrationBuilder.Sql("DROP PROCEDURE sp_GetTeamSeasonScheduleProfile");
			migrationBuilder.Sql("DROP FUNCTION fn_GetTeamSeasonScheduleTotals");
			migrationBuilder.Sql("DROP FUNCTION fn_GetTeamSeasonScheduleProfile");
			migrationBuilder.Sql("DROP FUNCTION fn_GetTeamSeasonOpponentProfiles");
			migrationBuilder.Sql("DROP FUNCTION fn_GetGamesByTeamAndSeason");
		}

		private void CreateFnGetGamesByTeamAndSeason(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION fn_GetGamesByTeamAndSeason
(	
	@teamName nvarchar(max),
	@seasonYear int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		ID,
		GuestName AS TeamName,
		GuestScore AS TeamScore,
		HostName AS OpponentName,
		HostScore AS OpponentScore,
		WinnerName,
		WinnerScore,
		LoserName,
		LoserScore
	FROM dbo.Games
	WHERE GuestName = @teamName AND SeasonYear = @seasonYear
	UNION
	SELECT
		ID,
		HostName AS TeamName,
		HostScore AS TeamScore,
		GuestName AS OpponentName,
		GuestScore AS OpponentScore,
		WinnerName,
		WinnerScore,
		LoserName,
		LoserScore
	FROM dbo.Games
	WHERE HostName = @teamName AND SeasonYear = @seasonYear
)
GO");
		}

		private void CreateFnGetTeamSeasonOpponentProfiles(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION fn_GetTeamSeasonOpponentProfiles
(	
	@teamName nvarchar(max), 
	@seasonYear int
)
RETURNS TABLE
AS
RETURN
(
	SELECT
		gbts.ID,
        gbts.OpponentName AS Opponent,
        gbts.TeamScore AS GamePointsFor,
        gbts.OpponentScore AS GamePointsAgainst,
		CASE
			WHEN gbts.WinnerName = gbts.OpponentName
				THEN ts.Wins - 1
			ELSE ts.Wins
		END AS OpponentWins,
		CASE
			WHEN gbts.LoserName = gbts.OpponentName
				THEN ts.Losses - 1
			ELSE ts.Losses
		END AS OpponentLosses,
		CASE
			WHEN gbts.WinnerName IS NULL AND gbts.LoserName IS NULL
				THEN ts.Ties - 1
			ELSE ts.Ties
		END AS OpponentTies,
		CASE
			WHEN ts.Games = 0
				THEN ts.Games
			ELSE ts.Games - 1
		END AS OpponentGames,
        ts.PointsFor - gbts.OpponentScore AS OpponentPointsFor,
        ts.PointsAgainst - gbts.TeamScore AS OpponentPointsAgainst
	FROM dbo.fn_GetGamesByTeamAndSeason(@teamName, @seasonYear) AS gbts
		LEFT JOIN dbo.TeamSeasons AS ts ON gbts.OpponentName = ts.TeamName AND ts.SeasonYear = @seasonYear
)
GO");
		}

		private void CreateFnGetTeamSeasonScheduleProfile(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION fn_GetTeamSeasonScheduleProfile 
(	
	@teamName nvarchar(max), 
	@seasonYear int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		tsop.ID,
        tsop.Opponent,
        tsop.GamePointsFor,
        tsop.GamePointsAgainst,
		tsop.OpponentWins,
		tsop.OpponentLosses,
		tsop.OpponentTies,
		CASE
			WHEN tsop.OpponentGames = 0
				THEN NULL
			ELSE (2 * CAST(tsop.OpponentWins AS float) + CAST(tsop.OpponentTies AS float)) / (2 * CAST(tsop.OpponentGames AS float))
		END AS OpponentWinningPercentage,        
		CASE
			WHEN tsop.OpponentGames = 0
				THEN NULL
			ELSE CAST(tsop.OpponentGames AS float) * CAST(s.NumOfWeeksCompleted AS float) / CAST(tsop.OpponentGames AS float)
		END AS OpponentWeightedGames,
		CASE
			WHEN tsop.OpponentGames = 0
				THEN NULL
			ELSE CAST(tsop.OpponentPointsFor AS float) * CAST(s.NumOfWeeksCompleted AS float) / CAST(tsop.OpponentGames AS float)
		END AS OpponentWeightedPointsFor,
		CASE
			WHEN tsop.OpponentGames = 0
				THEN NULL
			ELSE CAST(tsop.OpponentPointsAgainst AS float) * CAST(s.NumOfWeeksCompleted AS float) / CAST(tsop.OpponentGames AS float)
		END AS OpponentWeightedPointsAgainst
	FROM
		dbo.fn_GetTeamSeasonOpponentProfiles(@teamName, @seasonYear) AS tsop,
		dbo.Seasons AS s
	WHERE s.Year = @seasonYear
)
GO");
		}

		private void CreateFnGetTeamSeasonScheduleTotals(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION fn_GetTeamSeasonScheduleTotals 
(	
	@teamName nvarchar(max), 
	@seasonYear int
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
        COUNT(Opponent) AS Games,
        SUM(GamePointsFor) AS PointsFor,
        SUM(GamePointsAgainst) AS PointsAgainst,
		SUM(OpponentWins) AS ScheduleWins,
		SUM(OpponentLosses) AS ScheduleLosses,
		SUM(OpponentTies) AS ScheduleTies,
		SUM(OpponentWeightedGames) AS ScheduleGames,
		SUM(OpponentWeightedPointsFor) AS SchedulePointsFor,
		SUM(OpponentWeightedPointsAgainst) AS SchedulePointsAgainst
	FROM
		dbo.fn_GetTeamSeasonScheduleProfile(@teamName, @seasonYear)
)
GO");
		}

		private void CreateSpGetTeamSeasonScheduleAverages(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetTeamSeasonScheduleAverages 
	@teamName nvarchar(max), 
	@seasonYear int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		CASE
			WHEN Games = 0
				THEN NULL
			ELSE
				CAST(PointsFor AS float) / CAST(Games AS float)
		END AS PointsFor,
		CASE
			WHEN Games = 0
				THEN NULL
			ELSE
				CAST(PointsAgainst AS float) / CAST(Games AS float)
		END AS PointsAgainst,
		CASE
			WHEN ScheduleGames = 0
				THEN NULL
			ELSE
				SchedulePointsFor / ScheduleGames
		END AS SchedulePointsFor,
		CASE
			WHEN ScheduleGames = 0
				THEN NULL
			ELSE
				SchedulePointsAgainst / ScheduleGames
		END AS SchedulePointsAgainst
	FROM dbo.fn_GetTeamSeasonScheduleTotals(@teamName, @seasonYear)
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
CREATE PROCEDURE sp_GetTeamSeasonScheduleProfile 
	@teamName nvarchar(max), 
	@seasonYear int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		ID,
        Opponent,
        GamePointsFor,
        GamePointsAgainst,
		OpponentWins,
		OpponentLosses,
		OpponentTies,
		OpponentWinningPercentage,        
		OpponentWeightedGames,
		OpponentWeightedPointsFor,
		OpponentWeightedPointsAgainst
	FROM
		dbo.fn_GetTeamSeasonScheduleProfile(@teamName, @seasonYear)
	ORDER BY ID
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
CREATE PROCEDURE sp_GetTeamSeasonScheduleTotals
	@teamName nvarchar(max), 
	@seasonYear int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
        Games,
        PointsFor,
        PointsAgainst,
		ScheduleWins,
		ScheduleLosses,
		ScheduleTies,
		CASE
			WHEN ScheduleWins + ScheduleLosses + ScheduleTies = 0
				THEN NULL
			ELSE (2 * CAST(ScheduleWins AS float) + CAST(ScheduleTies AS float)) / (2 * (CAST(ScheduleWins AS float) + CAST(ScheduleLosses AS float) + CAST(ScheduleTies AS float)))
		END AS ScheduleWinningPercentage,
		ScheduleGames,
		SchedulePointsFor,
		SchedulePointsAgainst
	FROM
		dbo.fn_GetTeamSeasonScheduleTotals(@teamName, @seasonYear)
END
GO");
		}
	}
}
