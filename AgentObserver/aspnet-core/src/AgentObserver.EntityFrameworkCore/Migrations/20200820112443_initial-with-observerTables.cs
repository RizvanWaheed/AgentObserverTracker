using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AgentObserver.Migrations
{
    public partial class initialwithobserverTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    DepartmentName = table.Column<string>(nullable: true),
                    TeamName = table.Column<string>(nullable: true),
                    ManagerOrSupervisorName = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(nullable: true),
                    ActivityTime = table.Column<DateTime>(nullable: false),
                    PressedKey = table.Column<string>(nullable: true),
                    ActivityType = table.Column<string>(nullable: true),
                    NetWorkActivity = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAvailablitiy",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckinTime = table.Column<DateTime>(nullable: false),
                    CheckoutTime = table.Column<DateTime>(nullable: true),
                    BreakStartTime = table.Column<DateTime>(nullable: true),
                    BreakEndTime = table.Column<DateTime>(nullable: true),
                    IsBreak = table.Column<bool>(nullable: true),
                    UserNTLogin = table.Column<string>(nullable: true),
                    IsCheckedIn = table.Column<bool>(nullable: false),
                    IsCheckedOut = table.Column<bool>(nullable: false),
                    AvailabilityTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAvailablitiy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserBreaks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BreakStartTime = table.Column<DateTime>(nullable: false),
                    BreakEndTime = table.Column<DateTime>(nullable: true),
                    IsBreak = table.Column<bool>(nullable: true),
                    UserNTLogin = table.Column<string>(nullable: true),
                    EmpId = table.Column<string>(nullable: true),
                    CreatedDateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBreaks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityUser");

            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.DropTable(
                name: "UserAvailablitiy");

            migrationBuilder.DropTable(
                name: "UserBreaks");
        }
    }
}
