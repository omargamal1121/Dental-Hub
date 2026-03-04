using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removepublicidandfixrelation : Migration
    {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Admins_AspNetUsers_UserId",
				table: "Admins");

			migrationBuilder.DropForeignKey(
				name: "FK_CaseRequests_Doctors_DoctorId",
				table: "CaseRequests");

			migrationBuilder.DropForeignKey(
				name: "FK_CaseRequests_Students_StudentId",
				table: "CaseRequests");

			migrationBuilder.DropForeignKey(
				name: "FK_Doctors_AspNetUsers_UserId",
				table: "Doctors");

			migrationBuilder.DropForeignKey(
				name: "FK_Medias_Patients_PatientId",
				table: "Medias");

			migrationBuilder.DropForeignKey(
				name: "FK_PatientCases_Patients_PatientId",
				table: "PatientCases");

			migrationBuilder.DropForeignKey(
				name: "FK_PatientCases_Students_AssignedStudentId",
				table: "PatientCases");

			migrationBuilder.DropForeignKey(
				name: "FK_Patients_AspNetUsers_UserId",
				table: "Patients");

			migrationBuilder.DropForeignKey(
				name: "FK_Sessions_Patients_PatientId",
				table: "Sessions");

			migrationBuilder.DropForeignKey(
				name: "FK_Sessions_Students_StudentId",
				table: "Sessions");

			migrationBuilder.DropForeignKey(
				name: "FK_Students_AspNetUsers_UserId",
				table: "Students");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Students",
				table: "Students");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Patients",
				table: "Patients");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Doctors",
				table: "Doctors");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Admins",
				table: "Admins");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Students",
				table: "Students",
				column: "Id");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Patients",
				table: "Patients",
				column: "Id");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Doctors",
				table: "Doctors",
				column: "Id");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Admins",
				table: "Admins",
				column: "Id");

			migrationBuilder.AddForeignKey(
				name: "FK_Admins_AspNetUsers_Id",
				table: "Admins",
				column: "Id",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_CaseRequests_Doctors_DoctorId",
				table: "CaseRequests",
				column: "DoctorId",
				principalTable: "Doctors",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_CaseRequests_Students_StudentId",
				table: "CaseRequests",
				column: "StudentId",
				principalTable: "Students",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_Doctors_AspNetUsers_Id",
				table: "Doctors",
				column: "Id",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_Medias_Patients_PatientId",
				table: "Medias",
				column: "PatientId",
				principalTable: "Patients",
				principalColumn: "Id");

			migrationBuilder.AddForeignKey(
				name: "FK_PatientCases_Patients_PatientId",
				table: "PatientCases",
				column: "PatientId",
				principalTable: "Patients",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_PatientCases_Students_AssignedStudentId",
				table: "PatientCases",
				column: "AssignedStudentId",
				principalTable: "Students",
				principalColumn: "Id");

			migrationBuilder.AddForeignKey(
				name: "FK_Patients_AspNetUsers_Id",
				table: "Patients",
				column: "Id",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_Sessions_Patients_PatientId",
				table: "Sessions",
				column: "PatientId",
				principalTable: "Patients",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_Sessions_Students_StudentId",
				table: "Sessions",
				column: "StudentId",
				principalTable: "Students",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_Students_AspNetUsers_Id",
				table: "Students",
				column: "Id",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_AspNetUsers_Id",
                table: "Admins");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseRequests_Doctors_DoctorId",
                table: "CaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseRequests_Students_StudentId",
                table: "CaseRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_AspNetUsers_Id",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Medias_Patients_PatientId",
                table: "Medias");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientCases_Patients_PatientId",
                table: "PatientCases");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientCases_Students_AssignedStudentId",
                table: "PatientCases");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_Id",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Patients_PatientId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Students_StudentId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_Id",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Admins",
                table: "Admins");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Universities",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
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
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Sessions",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "SessionNotes",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Patients",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Patients",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "PatientCases",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Medias",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Doctors",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Doctors",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "CaseTypes",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "CaseRequests",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "AspNetUsers",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Admins",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Admins",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors",
                column: "UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Admins",
                table: "Admins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Universities_PublicId",
                table: "Universities",
                column: "PublicId",
                unique: true);

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
                name: "IX_PatientCases_PublicId",
                table: "PatientCases",
                column: "PublicId",
                unique: true);

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
                name: "FK_Admins_AspNetUsers_UserId",
                table: "Admins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseRequests_Doctors_DoctorId",
                table: "CaseRequests",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseRequests_Students_StudentId",
                table: "CaseRequests",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId",
                table: "Doctors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Medias_Patients_PatientId",
                table: "Medias",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCases_Patients_PatientId",
                table: "PatientCases",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCases_Students_AssignedStudentId",
                table: "PatientCases",
                column: "AssignedStudentId",
                principalTable: "Students",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Patients_PatientId",
                table: "Sessions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Students_StudentId",
                table: "Sessions",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
