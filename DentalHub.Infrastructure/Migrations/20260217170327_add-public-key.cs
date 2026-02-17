using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addpublickey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medias_CaseTypes_Id",
                table: "Medias");

          


            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Students",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Students",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Sessions",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "SessionNotes",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Patients",
                type: "char(36)",
                nullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Patients",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedStudentId",
                table: "PatientCases",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "PatientCases",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Medias",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Doctors",
                type: "char(36)",
                nullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Doctors",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "CaseTypes",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "CaseRequests",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "AspNetUsers",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Admins",
                type: "char(36)",
                nullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Admins",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

          

            migrationBuilder.CreateIndex(
                name: "IX_Students_PublicId",
                table: "Students",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_PublicId",
                table: "Sessions",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionNotes_PublicId",
                table: "SessionNotes",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PublicId",
                table: "Patients",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientCases_AssignedStudentId",
                table: "PatientCases",
                column: "AssignedStudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCases_PublicId",
                table: "PatientCases",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medias_CaseTypeId",
                table: "Medias",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Medias_PublicId",
                table: "Medias",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_PublicId",
                table: "Doctors",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseTypes_PublicId",
                table: "CaseTypes",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseRequests_PublicId",
                table: "CaseRequests",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PublicId",
                table: "AspNetUsers",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_PublicId",
                table: "Admins",
                column: "PublicId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_CaseTypes_CaseTypeId",
                table: "Medias",
                column: "CaseTypeId",
                principalTable: "CaseTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCases_Students_AssignedStudentId",
                table: "PatientCases",
                column: "AssignedStudentId",
                principalTable: "Students",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medias_CaseTypes_CaseTypeId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientCases_Students_AssignedStudentId",
                table: "PatientCases");

          

            migrationBuilder.DropIndex(
                name: "IX_Students_PublicId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_PublicId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_SessionNotes_PublicId",
                table: "SessionNotes");

            migrationBuilder.DropIndex(
                name: "IX_Patients_PublicId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_PatientCases_AssignedStudentId",
                table: "PatientCases");

            migrationBuilder.DropIndex(
                name: "IX_PatientCases_PublicId",
                table: "PatientCases");

            migrationBuilder.DropIndex(
                name: "IX_Medias_CaseTypeId",
                table: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_Medias_PublicId",
                table: "Medias");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_PublicId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_CaseTypes_PublicId",
                table: "CaseTypes");

            migrationBuilder.DropIndex(
                name: "IX_CaseRequests_PublicId",
                table: "CaseRequests");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PublicId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Admins_PublicId",
                table: "Admins");

           
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "SessionNotes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "AssignedStudentId",
                table: "PatientCases");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "PatientCases");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Medias");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "CaseTypes");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "CaseRequests");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Admins");

           


            migrationBuilder.AddForeignKey(
                name: "FK_Medias_CaseTypes_Id",
                table: "Medias",
                column: "Id",
                principalTable: "CaseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
