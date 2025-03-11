using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApplication.Migrations
{
    /// <inheritdoc />
    public partial class FixingCAOForReturningApplicants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applicants_CAONumber",
                table: "Applicants");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_CAONumber",
                table: "Applicants",
                column: "CAONumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Applicants_CAONumber",
                table: "Applicants");

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_CAONumber",
                table: "Applicants",
                column: "CAONumber",
                unique: true);
        }
    }
}
