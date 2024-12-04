using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApplication.Migrations.Primary
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "ScholarshipTypes",
                columns: table => new
                {
                    ScholarshipTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScholarshipLevelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipTypes", x => x.ScholarshipTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Scholarships",
                columns: table => new
                {
                    ScholarshipID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OtherDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScholarshipTypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scholarships", x => x.ScholarshipID);
                    table.ForeignKey(
                        name: "FK_Scholarships_ScholarshipTypes_ScholarshipTypeID",
                        column: x => x.ScholarshipTypeID,
                        principalTable: "ScholarshipTypes",
                        principalColumn: "ScholarshipTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applicants",
                columns: table => new
                {
                    ApplicantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CAONumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredLeisurewearSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeclarationConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecondarySchoolAttended = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriorThirdLevelAttendance = table.Column<bool>(type: "bit", nullable: false),
                    CourseSelectionReasons = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SportPositionOrCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentClub = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PastClubs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HighestCompetitionLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SportingAchievements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SportingGoals = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CampusID = table.Column<int>(type: "int", nullable: true),
                    ScholarshipID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.ApplicantID);
                    table.ForeignKey(
                        name: "FK_Applicants_Campuses_CampusID",
                        column: x => x.CampusID,
                        principalTable: "Campuses",
                        principalColumn: "CampusID");
                    table.ForeignKey(
                        name: "FK_Applicants_Scholarships_ScholarshipID",
                        column: x => x.ScholarshipID,
                        principalTable: "Scholarships",
                        principalColumn: "ScholarshipID");
                });

            migrationBuilder.CreateTable(
                name: "ApplicantSports",
                columns: table => new
                {
                    ApplicantID = table.Column<int>(type: "int", nullable: false),
                    SportID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantSports", x => new { x.ApplicantID, x.SportID });
                    table.ForeignKey(
                        name: "FK_ApplicantSports_Applicants_ApplicantID",
                        column: x => x.ApplicantID,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantSports_Sports_SportID",
                        column: x => x.SportID,
                        principalTable: "Sports",
                        principalColumn: "SportID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactDetails",
                columns: table => new
                {
                    ContactID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantID = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentsPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentsEmail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactDetails", x => x.ContactID);
                    table.ForeignKey(
                        name: "FK_ContactDetails_Applicants_ApplicantID",
                        column: x => x.ApplicantID,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseCodes",
                columns: table => new
                {
                    CourseCodeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantID = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCodes", x => x.CourseCodeID);
                    table.ForeignKey(
                        name: "FK_CourseCodes_Applicants_ApplicantID",
                        column: x => x.ApplicantID,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeDetails",
                columns: table => new
                {
                    HomeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantID = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeDetails", x => x.HomeID);
                    table.ForeignKey(
                        name: "FK_HomeDetails_Applicants_ApplicantID",
                        column: x => x.ApplicantID,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Referees",
                columns: table => new
                {
                    RefereeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TitleOrRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referees", x => x.RefereeID);
                    table.ForeignKey(
                        name: "FK_Referees_Applicants_ApplicantID",
                        column: x => x.ApplicantID,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScholarshipApplications",
                columns: table => new
                {
                    ApplicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantID = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ApplicationType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipApplications", x => x.ApplicationID);
                    table.ForeignKey(
                        name: "FK_ScholarshipApplications_Applicants_ApplicantID",
                        column: x => x.ApplicantID,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScholarshipOfferHistories",
                columns: table => new
                {
                    OfferID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantID = table.Column<int>(type: "int", nullable: false),
                    SportID = table.Column<int>(type: "int", nullable: false),
                    CampusID = table.Column<int>(type: "int", nullable: true),
                    ScholarshipID = table.Column<int>(type: "int", nullable: false),
                    OfferDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponseStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScholarshipOfferHistories", x => x.OfferID);
                    table.ForeignKey(
                        name: "FK_ScholarshipOfferHistories_Applicants_ApplicantID",
                        column: x => x.ApplicantID,
                        principalTable: "Applicants",
                        principalColumn: "ApplicantID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScholarshipOfferHistories_Scholarships_ScholarshipID",
                        column: x => x.ScholarshipID,
                        principalTable: "Scholarships",
                        principalColumn: "ScholarshipID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScholarshipOfferHistories_Sports_SportID",
                        column: x => x.SportID,
                        principalTable: "Sports",
                        principalColumn: "SportID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_CampusID",
                table: "Applicants",
                column: "CampusID");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_CAONumber",
                table: "Applicants",
                column: "CAONumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_ScholarshipID",
                table: "Applicants",
                column: "ScholarshipID");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantSports_SportID",
                table: "ApplicantSports",
                column: "SportID");

            migrationBuilder.CreateIndex(
                name: "IX_ContactDetails_ApplicantID",
                table: "ContactDetails",
                column: "ApplicantID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCodes_ApplicantID",
                table: "CourseCodes",
                column: "ApplicantID");

            migrationBuilder.CreateIndex(
                name: "IX_HomeDetails_ApplicantID",
                table: "HomeDetails",
                column: "ApplicantID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Referees_ApplicantID",
                table: "Referees",
                column: "ApplicantID");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipApplications_ApplicantID",
                table: "ScholarshipApplications",
                column: "ApplicantID");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipOfferHistories_ApplicantID_ScholarshipID",
                table: "ScholarshipOfferHistories",
                columns: new[] { "ApplicantID", "ScholarshipID" });

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipOfferHistories_ScholarshipID",
                table: "ScholarshipOfferHistories",
                column: "ScholarshipID");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipOfferHistories_SportID",
                table: "ScholarshipOfferHistories",
                column: "SportID");

            migrationBuilder.CreateIndex(
                name: "IX_Scholarships_ScholarshipTypeID",
                table: "Scholarships",
                column: "ScholarshipTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantSports");

            migrationBuilder.DropTable(
                name: "ContactDetails");

            migrationBuilder.DropTable(
                name: "CourseCodes");

            migrationBuilder.DropTable(
                name: "HomeDetails");

            migrationBuilder.DropTable(
                name: "Referees");

            migrationBuilder.DropTable(
                name: "ScholarshipApplications");

            migrationBuilder.DropTable(
                name: "ScholarshipOfferHistories");

            migrationBuilder.DropTable(
                name: "Applicants");

            migrationBuilder.DropTable(
                name: "Campuses");

            migrationBuilder.DropTable(
                name: "Scholarships");

            migrationBuilder.DropTable(
                name: "ScholarshipTypes");
        }
    }
}
