using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setuserdiagnosisrelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Diagnoses_CreatedById",
                table: "Diagnoses",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_AspNetUsers_CreatedById",
                table: "Diagnoses",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_AspNetUsers_CreatedById",
                table: "Diagnoses");

            migrationBuilder.DropIndex(
                name: "IX_Diagnoses_CreatedById",
                table: "Diagnoses");
        }
    }
}
