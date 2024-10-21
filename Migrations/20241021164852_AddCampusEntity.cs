using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApplicationPages.Migrations
{
    /// <inheritdoc />
    public partial class AddCampusEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HomeDetails_ApplicantID",
                table: "HomeDetails");

            migrationBuilder.DropIndex(
                name: "IX_ContactDetails_ApplicantID",
                table: "ContactDetails");

            migrationBuilder.RenameColumn(
                name: "RefereeDetails",
                table: "Referees",
                newName: "TitleOrRole");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Referees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Referees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Referees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CourseSelectionReasons",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrentClub",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Applicants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HighestCompetitionLevel",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeclarationConfirmed",
                table: "Applicants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MobilePhoneNumber",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PastClubs",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreferredLeisurewearSize",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "PriorThirdLevelAttendance",
                table: "Applicants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecondarySchoolAttended",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SportPositionOrCategory",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SportingAchievements",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SportingGoals",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Campuses",
                columns: table => new
                {
                    CampusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campuses", x => x.CampusID);
                });

            migrationBuilder.CreateTable(
                name: "CourseCode",
                columns: table => new
                {
                    CourseCodeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantID = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCode", x => x.CourseCodeID);
                    table.ForeignKey(
                        name: "FK_CourseCode_Applicants_ApplicantID",
                        column: x => x.ApplicantID,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeDetails_ApplicantID",
                table: "HomeDetails",
                column: "ApplicantID");

            migrationBuilder.CreateIndex(
                name: "IX_ContactDetails_ApplicantID",
                table: "ContactDetails",
                column: "ApplicantID");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_CampusID",
                table: "Applicants",
                column: "CampusID");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCode_ApplicantID",
                table: "CourseCode",
                column: "ApplicantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Campuses_CampusID",
                table: "Applicants",
                column: "CampusID",
                principalTable: "Campuses",
                principalColumn: "CampusID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Campuses_CampusID",
                table: "Applicants");

            migrationBuilder.DropTable(
                name: "Campuses");

            migrationBuilder.DropTable(
                name: "CourseCode");

            migrationBuilder.DropIndex(
                name: "IX_HomeDetails_ApplicantID",
                table: "HomeDetails");

            migrationBuilder.DropIndex(
                name: "IX_ContactDetails_ApplicantID",
                table: "ContactDetails");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_CampusID",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Referees");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Referees");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Referees");

            migrationBuilder.DropColumn(
                name: "CourseSelectionReasons",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "CurrentClub",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "HighestCompetitionLevel",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "IsDeclarationConfirmed",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "MobilePhoneNumber",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "PastClubs",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "PreferredLeisurewearSize",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "PriorThirdLevelAttendance",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "SecondarySchoolAttended",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "SportPositionOrCategory",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "SportingAchievements",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "SportingGoals",
                table: "Applicants");

            migrationBuilder.RenameColumn(
                name: "TitleOrRole",
                table: "Referees",
                newName: "RefereeDetails");

            migrationBuilder.CreateIndex(
                name: "IX_HomeDetails_ApplicantID",
                table: "HomeDetails",
                column: "ApplicantID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactDetails_ApplicantID",
                table: "ContactDetails",
                column: "ApplicantID",
                unique: true);
        }
    }
}
