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
                    Year = table.Column<int>(nullable: false),
                    NumOfWeeksScheduled = table.Column<int>(nullable: false, defaultValue: 0),
                    NumOfWeeksCompleted = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Season", x => x.Year);
                });

            migrationBuilder.CreateTable(
                name: "League",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortName = table.Column<string>(maxLength: 5, nullable: false),
                    LongName = table.Column<string>(maxLength: 50, nullable: false),
                    FirstSeasonYear = table.Column<int>(nullable: false),
                    LastSeasonYear = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_League", x => x.Id);
                    table.UniqueConstraint("UQ_League_ShortName", x => x.ShortName);
                    table.UniqueConstraint("UQ_League_LongName", x => x.LongName);
                    table.ForeignKey(
                        name: "FK_League_Season_FirstSeasonYear",
                        column: x => x.FirstSeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_League_Season_LastSeasonYear",
                        column: x => x.LastSeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Conference",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortName = table.Column<string>(maxLength: 5, nullable: false),
                    LongName = table.Column<string>(maxLength: 50, nullable: false),
                    LeagueId = table.Column<int>(nullable: false),
                    FirstSeasonYear = table.Column<int>(nullable: false),
                    LastSeasonYear = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conference", x => x.Id);
                    table.UniqueConstraint("UQ_Conference_ShortName", x => x.ShortName);
                    table.UniqueConstraint("UQ_Conference_LongName", x => x.LongName);
                    table.ForeignKey(
                        name: "FK_Conference_League_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "League",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Conference_Season_FirstSeasonYear",
                        column: x => x.FirstSeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Conference_Season_LastSeasonYear",
                        column: x => x.LastSeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.Restrict,
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    LeagueId = table.Column<int>(nullable: false),
                    ConferenceId = table.Column<int>(nullable: true),
                    FirstSeasonYear = table.Column<int>(nullable: false),
                    LastSeasonYear = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.Id);
                    table.UniqueConstraint("UQ_Division_Name", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Division_League_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "League",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Division_Conference_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conference",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Division_Season_FirstSeasonYear",
                        column: x => x.FirstSeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Division_Season_LastSeasonYear",
                        column: x => x.LastSeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                    table.UniqueConstraint("UQ_Team_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeasonYear = table.Column<int>(nullable: false),
                    Week = table.Column<int>(nullable: false),
                    GuestName = table.Column<string>(maxLength: 50, nullable: false),
                    GuestScore = table.Column<int>(nullable: false, defaultValue: 0),
                    HostName = table.Column<string>(maxLength: 50, nullable: false),
                    HostScore = table.Column<int>(nullable: false, defaultValue: 0),
                    IsPlayoff = table.Column<bool>(nullable: false, defaultValue: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                    table.UniqueConstraint("UQ_Game_Season_Week_Teams", x => new { x.SeasonYear, x.Week, x.GuestName, x.HostName });
                    table.ForeignKey(
                        name: "FK_Game_Season_SeasonYear",
                        column: x => x.SeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.Restrict,
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeagueSeason",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeagueId = table.Column<int>(nullable: false),
                    SeasonYear = table.Column<int>(nullable: false),
                    TotalGames = table.Column<int>(nullable: false, defaultValue: 0),
                    TotalPoints = table.Column<int>(nullable: false, defaultValue: 0),
                    AveragePoints = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueSeason", x => x.Id);
                    table.UniqueConstraint("UQ_LeagueSeason_League_Season", x => new { x.LeagueId, x.SeasonYear });
                    table.ForeignKey(
                        name: "FK_LeagueSeason_League_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "League",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueSeason_Season_SeasonYear",
                        column: x => x.SeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.Restrict,
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamSeason",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamId = table.Column<int>(nullable: false),
                    SeasonYear = table.Column<int>(nullable: false),
                    LeagueId = table.Column<int>(nullable: false),
                    ConferenceId = table.Column<int>(nullable: true),
                    DivisionId = table.Column<int>(nullable: true),
                    Games = table.Column<int>(nullable: false, defaultValue: 0),
                    Wins = table.Column<int>(nullable: false, defaultValue: 0),
                    Losses = table.Column<int>(nullable: false, defaultValue: 0),
                    Ties = table.Column<int>(nullable: false, defaultValue: 0),
                    PointsFor = table.Column<int>(nullable: false, defaultValue: 0),
                    PointsAgainst = table.Column<int>(nullable: false, defaultValue: 0),
                    ExpectedWins = table.Column<decimal>(nullable: false, defaultValue: 0m),
                    ExpectedLosses = table.Column<decimal>(nullable: false, defaultValue: 0m),
                    OffensiveAverage = table.Column<decimal>(nullable: true),
                    OffensiveFactor = table.Column<decimal>(nullable: true),
                    OffensiveIndex = table.Column<decimal>(nullable: true),
                    DefensiveAverage = table.Column<decimal>(nullable: true),
                    DefensiveFactor = table.Column<decimal>(nullable: true),
                    DefensiveIndex = table.Column<decimal>(nullable: true),
                    FinalExpectedWinningPercentage = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamSeason", x => x.Id);
                    table.UniqueConstraint("UQ_TeamSeason_Team_Season", x => new { x.TeamId, x.SeasonYear });
                    table.ForeignKey(
                        name: "FK_TeamSeason_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamSeason_Season_SeasonYear",
                        column: x => x.SeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TeamSeason_League_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "League",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamSeason_Conference_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conference",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TeamSeason_Division_DivisionId",
                        column: x => x.DivisionId,
                        principalTable: "Division",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FK_League_Season_FirstSeasonYear",
                table: "League",
                column: "FirstSeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_League_Season_LastSeasonYear",
                table: "League",
                column: "LastSeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Conference_League_LeagueId",
                table: "Conference",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Conference_Season_FirstSeasonYear",
                table: "Conference",
                column: "FirstSeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Conference_Season_LastSeasonYear",
                table: "Conference",
                column: "LastSeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Division_League_LeagueId",
                table: "Division",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Division_Conference_ConferenceId",
                table: "Division",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Division_Season_FirstSeasonYear",
                table: "Division",
                column: "FirstSeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Division_Season_LastSeasonYear",
                table: "Division",
                column: "LastSeasonYear");

            migrationBuilder.CreateIndex(
                name: "FK_Game_Season_SeasonYear",
                table: "Game",
                column: "SeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_LeagueSeason_League_LeagueId",
                table: "LeagueSeason",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_LeagueSeason_Season_SeasonYear",
                table: "LeagueSeason",
                column: "SeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Team_TeamId",
                table: "TeamSeason",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Season_SeasonYear",
                table: "TeamSeason",
                column: "SeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_League_LeagueId",
                table: "TeamSeason",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Conference_ConferenceId",
                table: "TeamSeason",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Division_DivisionId",
                table: "TeamSeason",
                column: "DivisionId");
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
                name: "Division");

            migrationBuilder.DropTable(
                name: "Conference");

            migrationBuilder.DropTable(
                name: "League");

            migrationBuilder.DropTable(
                name: "Season");
        }
    }
}
