using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EldredBrown.ProFootball.Net.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Season",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(nullable: false),
                    NumOfWeeksScheduled = table.Column<int>(nullable: false, defaultValue: 0),
                    NumOfWeeksCompleted = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Season", x => x.Id);
                    table.UniqueConstraint("UQ_Season_Year", x => x.Year);
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
                    LeagueName = table.Column<string>(nullable: false),
                    FirstSeasonYear = table.Column<int>(nullable: false),
                    LastSeasonYear = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conference", x => x.Id);
                    table.UniqueConstraint("UQ_Conference_ShortName", x => x.ShortName);
                    table.UniqueConstraint("UQ_Conference_LongName", x => x.LongName);
                    table.ForeignKey(
                        name: "FK_Conference_League_LeagueName",
                        column: x => x.LeagueName,
                        principalTable: "League",
                        principalColumn: "ShortName",
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
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Division",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    LeagueName = table.Column<string>(nullable: false),
                    ConferenceName = table.Column<string>(nullable: false),
                    FirstSeasonYear = table.Column<int>(nullable: false),
                    LastSeasonYear = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Division", x => x.Id);
                    table.UniqueConstraint("UQ_Division_Name", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Division_Conference_ConferenceName",
                        column: x => x.ConferenceName,
                        principalTable: "Conference",
                        principalColumn: "ShortName",
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
                    GuestName = table.Column<string>(nullable: false),
                    GuestScore = table.Column<int>(nullable: false, defaultValue: 0),
                    HostName = table.Column<string>(nullable: false),
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
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "LeagueSeason",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeagueName = table.Column<string>(maxLength: 5, nullable: false),
                    SeasonYear = table.Column<int>(nullable: false),
                    TotalGames = table.Column<int>(nullable: false, defaultValue: 0),
                    TotalPoints = table.Column<int>(nullable: false, defaultValue: 0),
                    AveragePoints = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueSeason", x => x.Id);
                    table.UniqueConstraint("UQ_LeagueSeason_League_Season", x => new { x.LeagueName, x.SeasonYear });
                    table.ForeignKey(
                        name: "FK_LeagueSeason_League_LeagueName",
                        column: x => x.LeagueName,
                        principalTable: "League",
                        principalColumn: "Name",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueSeason_Season_SeasonYear",
                        column: x => x.SeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamSeason",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamName = table.Column<string>(maxLength: 50, nullable: false),
                    SeasonYear = table.Column<int>(nullable: false),
                    LeagueName = table.Column<string>(maxLength: 5, nullable: false),
                    ConferenceName = table.Column<string>(maxLength: 5, nullable: true),
                    DivisionName = table.Column<string>(maxLength: 50, nullable: true),
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
                    table.UniqueConstraint("UQ_TeamSeason_Team_Season", x => new { x.TeamName, x.SeasonYear });
                    table.ForeignKey(
                        name: "FK_TeamSeason_Team_TeamName",
                        column: x => x.TeamName,
                        principalTable: "Team",
                        principalColumn: "Name",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamSeason_Season_SeasonYear",
                        column: x => x.SeasonYear,
                        principalTable: "Season",
                        principalColumn: "Year",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamSeason_League_LeagueName",
                        column: x => x.LeagueName,
                        principalTable: "League",
                        principalColumn: "ShortName",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TeamSeason_Conference_ConferenceName",
                        column: x => x.ConferenceName,
                        principalTable: "Conference",
                        principalColumn: "ShortName",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TeamSeason_Division_DivisionName",
                        column: x => x.ConferenceName,
                        principalTable: "Division",
                        principalColumn: "Name",
                        onUpdate: ReferentialAction.NoAction,
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "SeasonStandings",
                columns: table => new
                {
                    Team = table.Column<string>(nullable: true),
                    Conference = table.Column<string>(nullable: true),
                    Division = table.Column<string>(nullable: true),
                    Wins = table.Column<int>(nullable: false),
                    Losses = table.Column<int>(nullable: false),
                    Ties = table.Column<int>(nullable: false),
                    WinningPercentage = table.Column<decimal>(nullable: true),
                    PointsFor = table.Column<int>(nullable: false),
                    PointsAgainst = table.Column<int>(nullable: false),
                    AvgPointsFor = table.Column<decimal>(nullable: true),
                    AvgPointsAgainst = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TeamSeasonScheduleProfile",
                columns: table => new
                {
                    Opponent = table.Column<string>(nullable: true),
                    GamePointsFor = table.Column<int>(nullable: true),
                    GamePointsAgainst = table.Column<int>(nullable: true),
                    OpponentWins = table.Column<int>(nullable: true),
                    OpponentLosses = table.Column<int>(nullable: true),
                    OpponentTies = table.Column<int>(nullable: true),
                    OpponentWinningPercentage = table.Column<decimal>(nullable: true),
                    OpponentWeightedGames = table.Column<decimal>(nullable: true),
                    OpponentWeightedPointsFor = table.Column<decimal>(nullable: true),
                    OpponentWeightedPointsAgainst = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TeamSeasonScheduleTotals",
                columns: table => new
                {
                    Games = table.Column<int>(nullable: true),
                    PointsFor = table.Column<int>(nullable: true),
                    PointsAgainst = table.Column<int>(nullable: true),
                    ScheduleWins = table.Column<int>(nullable: true),
                    ScheduleLosses = table.Column<int>(nullable: true),
                    ScheduleTies = table.Column<int>(nullable: true),
                    ScheduleWinningPercentage = table.Column<decimal>(nullable: true),
                    ScheduleGames = table.Column<decimal>(nullable: true),
                    SchedulePointsFor = table.Column<decimal>(nullable: true),
                    SchedulePointsAgainst = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TeamSeasonScheduleAverages",
                columns: table => new
                {
                    PointsFor = table.Column<decimal>(nullable: true),
                    PointsAgainst = table.Column<decimal>(nullable: true),
                    SchedulePointsFor = table.Column<decimal>(nullable: true),
                    SchedulePointsAgainst = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FK_League_Season_FirstSeasonYear",
                table: "League",
                column: "FirstSeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_League_Season_LastSeasonYear",
                table: "League",
                column: "LastSeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Conference_League_LeagueName",
                table: "Conference",
                column: "LeagueName");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Conference_Season_FirstSeasonYear",
                table: "Conference",
                column: "FirstSeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Conference_Season_LastSeasonYear",
                table: "Conference",
                column: "LastSeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Division_League_LeagueName",
                table: "Division",
                column: "LeagueName");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Division_Conference_ConferenceName",
                table: "Division",
                column: "ConferenceName");

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
                name: "IX_FK_LeagueSeason_League_LeagueName",
                table: "LeagueSeason",
                column: "LeagueName");

            migrationBuilder.CreateIndex(
                name: "IX_FK_LeagueSeason_Season_SeasonYear",
                table: "LeagueSeason",
                column: "SeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Team_TeamName",
                table: "TeamSeason",
                column: "TeamName");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Season_SeasonId",
                table: "TeamSeason",
                column: "SeasonYear");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_League_LeagueName",
                table: "TeamSeason",
                column: "LeagueName");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Conference_ConferenceName",
                table: "TeamSeason",
                column: "ConferenceName");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TeamSeason_Division_DivisionName",
                table: "TeamSeason",
                column: "DivisionName");
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
                name: "SeasonStanding");

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

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AspNetRoles");
        }
    }
}
