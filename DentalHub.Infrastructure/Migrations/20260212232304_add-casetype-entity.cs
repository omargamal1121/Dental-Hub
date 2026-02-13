using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcasetypeentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TreatmentType",
                table: "PatientCases",
                newName: "Description");

            migrationBuilder.AddColumn<Guid>(
                name: "CaseTypeId",
                table: "PatientCases",
                type: "char(36)",
                nullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "CaseTypeId",
                table: "Medias",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "CaseTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeleteAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCases_CaseTypeId",
                table: "PatientCases",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseTypes_Name",
                table: "CaseTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_CaseTypes_Id",
                table: "Medias",
                column: "Id",
                principalTable: "CaseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCases_CaseTypes_CaseTypeId",
                table: "PatientCases",
                column: "CaseTypeId",
                principalTable: "CaseTypes",
                principalColumn: "CaseTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medias_CaseTypes_Id",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientCases_CaseTypes_CaseTypeId",
                table: "PatientCases");

            migrationBuilder.DropTable(
                name: "CaseTypes");

            migrationBuilder.DropIndex(
                name: "IX_PatientCases_CaseTypeId",
                table: "PatientCases");

            migrationBuilder.DropColumn(
                name: "CaseTypeId",
                table: "PatientCases");

            migrationBuilder.DropColumn(
                name: "CaseTypeId",
                table: "Medias");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "PatientCases",
                newName: "TreatmentType");
        }
    }
}
