using Microsoft.EntityFrameworkCore.Migrations;

namespace EldredBrown.ProFootball.Net.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Season",
                columns: table => new
                {
                    year = table.Column<int>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Season", x => x.year);
                    table.CheckConstraint("CK_Season_Year_Min", "[Year] >= 1920");
                });

            migrationBuilder.CreateTable(
                name: "Association",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parent_id = table.Column<int>(nullable: true),
                    long_name = table.Column<string>(maxLength: 100, nullable: false),
                    short_name = table.Column<string>(maxLength: 5, nullable: false),
                    first_season_year = table.Column<int>(nullable: false),
                    last_season_year = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Association", x => x.id);
                    table.UniqueConstraint("UQ_Association_LongName", x => x.long_name);
                    table.UniqueConstraint("UQ_Association_ShortName", x => x.short_name);
                    table.ForeignKey(
                        name: "FK_Association_ParentId",
                        column: x => x.parent_id,
                        principalTable: "Association",
                        principalColumn: "id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Association_Season_FirstSeasonYear",
                        column: x => x.first_season_year,
                        principalTable: "Season",
                        principalColumn: "year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Association_Season_LastSeasonYear",
                        column: x => x.last_season_year,
                        principalTable: "Season",
                        principalColumn: "year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.id);
                    table.UniqueConstraint("UQ_Team_Name", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    season_year = table.Column<int>(nullable: false),
                    league_id = table.Column<int>(nullable: true),
                    week = table.Column<int>(nullable: false),
                    guest_name = table.Column<string>(maxLength: 100, nullable: false),
                    guest_score = table.Column<int>(nullable: false, defaultValue: 0),
                    host_name = table.Column<string>(maxLength: 100, nullable: false),
                    host_score = table.Column<int>(nullable: false, defaultValue: 0),
                    is_playoff = table.Column<bool>(nullable: false, defaultValue: false),
                    notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.id);
                    table.UniqueConstraint("UQ_Game_Season_League_Week_Teams", x => new {
                        x.season_year, x.league_id, x.week, x.guest_name, x.host_name
                    });
                    table.ForeignKey(
                        name: "FK_Game_Season_SeasonYear",
                        column: x => x.season_year,
                        principalTable: "Season",
                        principalColumn: "year",
                        onUpdate: ReferentialAction.Restrict,
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Game_Association_LeagueId",
                        column: x => x.league_id,
                        principalTable: "Association",
                        principalColumn: "id",
                        onUpdate: ReferentialAction.Restrict,
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeagueSeason",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    league_id = table.Column<int>(nullable: false),
                    season_year = table.Column<int>(nullable: false),
                    num_of_weeks_scheduled = table.Column<int>(nullable: false, defaultValue: 0),
                    num_of_weeks_completed = table.Column<int>(nullable: false, defaultValue: 0),
                    total_games = table.Column<int>(nullable: false, defaultValue: 0),
                    total_points = table.Column<int>(nullable: false, defaultValue: 0),
                    average_points = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueSeason", x => x.id);
                    table.UniqueConstraint("UQ_LeagueSeason_League_Season", x => new { x.league_id, x.season_year });
                    table.ForeignKey(
                        name: "FK_LeagueSeason_Association_LeagueId",
                        column: x => x.league_id,
                        principalTable: "Association",
                        principalColumn: "id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueSeason_Season_SeasonYear",
                        column: x => x.season_year,
                        principalTable: "Season",
                        principalColumn: "year",
                        onUpdate: ReferentialAction.Restrict,
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamSeason",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    team_id = table.Column<int>(nullable: false),
                    season_year = table.Column<int>(nullable: false),
                    league_id = table.Column<int>(nullable: false),
                    conference_id = table.Column<int>(nullable: true),
                    division_id = table.Column<int>(nullable: true),
                    games = table.Column<int>(nullable: false, defaultValue: 0),
                    wins = table.Column<int>(nullable: false, defaultValue: 0),
                    losses = table.Column<int>(nullable: false, defaultValue: 0),
                    ties = table.Column<int>(nullable: false, defaultValue: 0),
                    points_for = table.Column<int>(nullable: false, defaultValue: 0),
                    points_against = table.Column<int>(nullable: false, defaultValue: 0),
                    expected_wins = table.Column<decimal>(nullable: false, defaultValue: 0m),
                    expected_losses = table.Column<decimal>(nullable: false, defaultValue: 0m),
                    offensive_average = table.Column<decimal>(nullable: true),
                    offensive_factor = table.Column<decimal>(nullable: true),
                    offensive_index = table.Column<decimal>(nullable: true),
                    defensive_average = table.Column<decimal>(nullable: true),
                    defensive_factor = table.Column<decimal>(nullable: true),
                    defensive_index = table.Column<decimal>(nullable: true),
                    final_expected_winning_percentage = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamSeason", x => x.id);
                    table.UniqueConstraint("UQ_TeamSeason_Team_Season", x => new { x.team_id, x.season_year });
                    table.ForeignKey(
                        name: "FK_TeamSeason_Team_TeamId",
                        column: x => x.team_id,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamSeason_Season_SeasonYear",
                        column: x => x.season_year,
                        principalTable: "Season",
                        principalColumn: "year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TeamSeason_Association_LeagueId",
                        column: x => x.league_id,
                        principalTable: "Association",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TeamSeason_Association_ConferenceId",
                        column: x => x.conference_id,
                        principalTable: "Association",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TeamSeason_Association_DivisionId",
                        column: x => x.division_id,
                        principalTable: "Association",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FK_Association_ParentId",
                table: "Association",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Association_Season_FirstSeasonYear",
                table: "Association",
                column: "first_season_year");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Association_Season_LastSeasonYear",
                table: "Association",
                column: "last_season_year");

            migrationBuilder.CreateIndex(
                name: "FK_Game_Season_SeasonYear",
                table: "Game",
                column: "season_year");

            migrationBuilder.CreateIndex(
                name: "FK_Game_Association_LeagueId",
                table: "Game",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "IX_FK_LeagueSeason_Association_LeagueId",
                table: "LeagueSeason",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "IX_FK_LeagueSeason_Season_SeasonYear",
                table: "LeagueSeason",
                column: "season_year");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Team_TeamId",
                table: "TeamSeason",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Season_SeasonYear",
                table: "TeamSeason",
                column: "season_year");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Association_LeagueId",
                table: "TeamSeason",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Association_ConferenceId",
                table: "TeamSeason",
                column: "conference_id");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Association_DivisionId",
                table: "TeamSeason",
                column: "division_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamSeasonScheduleAverages");

            migrationBuilder.DropTable(
                name: "TeamSeasonScheduleTotals");

            migrationBuilder.DropTable(
                name: "TeamSeasonScheduleProfile");

            migrationBuilder.DropTable(
                name: "SeasonStandings");

            migrationBuilder.DropTable(
                name: "TeamSeason");

            migrationBuilder.DropTable(
                name: "LeagueSeason");

            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "Team");

            migrationBuilder.DropTable(
                name: "Association");

            migrationBuilder.DropTable(
                name: "Season");
        }
    }
}
