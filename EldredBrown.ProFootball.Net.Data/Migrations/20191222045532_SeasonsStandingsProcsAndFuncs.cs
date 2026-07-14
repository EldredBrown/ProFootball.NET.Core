using Microsoft.EntityFrameworkCore.Migrations;

namespace EldredBrown.ProFootball.Net.Data.Migrations
{
    public partial class SeasonsStandingsProcsAndFuncs : Migration
    {
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			CreateSpGetSeasonStandings(migrationBuilder);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE sp_GetSeasonStandings");
        }

        private void CreateSpGetSeasonStandings(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
USE ProFootballDb_Proposed
GO
-- =============================================
-- Author:		Eldred Brown
-- Create date: 2017-01-14
-- Description:	A procedure to return a conference's season standings
-- Revision history:
--	2025-10-02	Eldred Brown
--	*	Changed variable names to snake_case to make more Pythonic
--	2025-10-25	Eldred Brown
--	*	Changed casting of avg_points_for and avg_points_against to decimal.
--	2026-05-07	Eldred Brown
--	*	Changed winning_percentage to derived value after removing column from TeamSeason table.
-- =============================================
CREATE PROCEDURE dbo.sp_GetSeasonStandings
	-- Add the parameters for the stored procedure here
	@season_id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN
		-- Insert statements for procedure here
		SELECT
			(SELECT name FROM dbo.Team WHERE id = team_id) AS team,
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
		WHERE season_id = @season_id
		ORDER BY
			winning_percentage DESC,
			wins DESC,
			losses ASC,
			expected_wins DESC,
			expected_losses ASC,
			team ASC
	END
END
GO");
        }
    }
}
