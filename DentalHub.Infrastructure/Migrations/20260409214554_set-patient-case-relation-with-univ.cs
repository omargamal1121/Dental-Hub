using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setpatientcaserelationwithuniv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "PatientCases",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UniversityId",
                table: "PatientCases",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "TeethNumbers",
                table: "Diagnoses",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCases_UniversityId",
                table: "PatientCases",
                column: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCases_Universities_UniversityId",
                table: "PatientCases",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientCases_Universities_UniversityId",
                table: "PatientCases");

            migrationBuilder.DropIndex(
                name: "IX_PatientCases_UniversityId",
                table: "PatientCases");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "PatientCases");

            migrationBuilder.DropColumn(
                name: "UniversityId",
                table: "PatientCases");

            migrationBuilder.DropColumn(
                name: "TeethNumbers",
                table: "Diagnoses");
        }
    }
}
