using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updaterelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScheduledAt",
                table: "Sessions",
                newName: "StartAt");

            migrationBuilder.AddColumn<string>(
                name: "DoctorNote",
                table: "Sessions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndAt",
                table: "Sessions",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "EvaluteDoctorId",
                table: "Sessions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedDoctorId",
                table: "PatientCases",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_EvaluteDoctorId",
                table: "Sessions",
                column: "EvaluteDoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCases_AssignedDoctorId",
                table: "PatientCases",
                column: "AssignedDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCases_Doctors_AssignedDoctorId",
                table: "PatientCases",
                column: "AssignedDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Doctors_EvaluteDoctorId",
                table: "Sessions",
                column: "EvaluteDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientCases_Doctors_AssignedDoctorId",
                table: "PatientCases");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Doctors_EvaluteDoctorId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_EvaluteDoctorId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_PatientCases_AssignedDoctorId",
                table: "PatientCases");

            migrationBuilder.DropColumn(
                name: "DoctorNote",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "EndAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "EvaluteDoctorId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "AssignedDoctorId",
                table: "PatientCases");

            migrationBuilder.RenameColumn(
                name: "StartAt",
                table: "Sessions",
                newName: "ScheduledAt");
        }
    }
}
