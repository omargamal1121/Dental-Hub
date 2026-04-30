using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addSessionNoteIdtoMedias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medias_CaseTypes_CaseTypeId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_PatientCases_PatientCaseId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Patients_PatientId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Sessions_SessionId",
                table: "Medias");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "SessionNotes",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "CaseTypeId1",
                table: "Medias",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId1",
                table: "Medias",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "SessionNoteId",
                table: "Medias",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_CaseTypeId1",
                table: "Medias",
                column: "CaseTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_PatientId1",
                table: "Medias",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_SessionNoteId",
                table: "Medias",
                column: "SessionNoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_CaseTypes_CaseTypeId",
                table: "Medias",
                column: "CaseTypeId",
                principalTable: "CaseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_CaseTypes_CaseTypeId1",
                table: "Medias",
                column: "CaseTypeId1",
                principalTable: "CaseTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_PatientCases_PatientCaseId",
                table: "Medias",
                column: "PatientCaseId",
                principalTable: "PatientCases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Patients_PatientId",
                table: "Medias",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Patients_PatientId1",
                table: "Medias",
                column: "PatientId1",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_SessionNotes_SessionNoteId",
                table: "Medias",
                column: "SessionNoteId",
                principalTable: "SessionNotes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Sessions_SessionId",
                table: "Medias",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medias_CaseTypes_CaseTypeId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_CaseTypes_CaseTypeId1",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_PatientCases_PatientCaseId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Patients_PatientId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Patients_PatientId1",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_SessionNotes_SessionNoteId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Sessions_SessionId",
                table: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_Medias_CaseTypeId1",
                table: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_Medias_PatientId1",
                table: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_Medias_SessionNoteId",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "CaseTypeId1",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "SessionNoteId",
                table: "Medias");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "SessionNotes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(2000)",
                oldMaxLength: 2000)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_CaseTypes_CaseTypeId",
                table: "Medias",
                column: "CaseTypeId",
                principalTable: "CaseTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_PatientCases_PatientCaseId",
                table: "Medias",
                column: "PatientCaseId",
                principalTable: "PatientCases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Patients_PatientId",
                table: "Medias",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Sessions_SessionId",
                table: "Medias",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");
        }
    }
}
