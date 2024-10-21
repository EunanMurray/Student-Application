using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApplicationPages.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommitteeMembers",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommitteeMembers", x => x.MemberID);
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
                name: "Sports",
                columns: table => new
                {
                    SportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SportName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sports", x => x.SportID);
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
                name: "MemberSports",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    SportID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberSports", x => new { x.MemberID, x.SportID });
                    table.ForeignKey(
                        name: "FK_MemberSports_CommitteeMembers_MemberID",
                        column: x => x.MemberID,
                        principalTable: "CommitteeMembers",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberSports_Sports_SportID",
                        column: x => x.SportID,
                        principalTable: "Sports",
                        principalColumn: "SportID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applicants",
                columns: table => new
                {
                    ApplicantID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CAONumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CampusID = table.Column<int>(type: "int", nullable: true),
                    ScholarshipID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applicants", x => x.ApplicantID);
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
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    RefereeDetails = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                name: "IX_HomeDetails_ApplicantID",
                table: "HomeDetails",
                column: "ApplicantID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberSports_SportID",
                table: "MemberSports",
                column: "SportID");

            migrationBuilder.CreateIndex(
                name: "IX_Referees_ApplicantID",
                table: "Referees",
                column: "ApplicantID");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipApplications_ApplicantID",
                table: "ScholarshipApplications",
                column: "ApplicantID");

            migrationBuilder.CreateIndex(
                name: "IX_ScholarshipOfferHistories_ApplicantID",
                table: "ScholarshipOfferHistories",
                column: "ApplicantID");

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
                name: "HomeDetails");

            migrationBuilder.DropTable(
                name: "MemberSports");

            migrationBuilder.DropTable(
                name: "Referees");

            migrationBuilder.DropTable(
                name: "ScholarshipApplications");

            migrationBuilder.DropTable(
                name: "ScholarshipOfferHistories");

            migrationBuilder.DropTable(
                name: "CommitteeMembers");

            migrationBuilder.DropTable(
                name: "Applicants");

            migrationBuilder.DropTable(
                name: "Sports");

            migrationBuilder.DropTable(
                name: "Scholarships");

            migrationBuilder.DropTable(
                name: "ScholarshipTypes");
        }
    }
}
