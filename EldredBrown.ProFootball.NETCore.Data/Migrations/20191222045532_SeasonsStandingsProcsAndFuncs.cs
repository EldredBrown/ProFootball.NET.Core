using Microsoft.EntityFrameworkCore.Migrations;

namespace EldredBrown.ProFootball.NETCore.Data.Migrations
{
    public partial class SeasonsStandingsProcsAndFuncs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE sp_GetSeasonStandings 
	@seasonYear int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		TeamName AS Team,
        ConferenceName AS Conference,
        DivisionName AS Division,
        Wins,
        Losses,
        Ties,
        WinningPercentage,
        PointsFor,
        PointsAgainst,
		CASE
			WHEN Games = 0
				THEN NULL
			ELSE CAST(PointsFor AS float) / CAST(Games AS float)
		END AS AvgPointsFor,
		CASE
			WHEN Games = 0
				THEN NULL
			ELSE CAST(PointsAgainst AS float) / CAST(Games AS float)
		END AS AvgPointsAgainst
	FROM dbo.TeamSeasons
	WHERE SeasonYear = @seasonYear
	ORDER BY WinningPercentage DESC, Wins DESC, Team ASC
END
GO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE sp_GetSeasonStandings");
        }
    }
}
