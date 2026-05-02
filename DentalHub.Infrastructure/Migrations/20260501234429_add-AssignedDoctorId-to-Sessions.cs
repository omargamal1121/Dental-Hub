using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addAssignedDoctorIdtoSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedDoctorId",
                table: "Sessions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AssignedDoctorId",
                table: "Sessions",
                column: "AssignedDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Doctors_AssignedDoctorId",
                table: "Sessions",
                column: "AssignedDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Doctors_AssignedDoctorId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_AssignedDoctorId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "AssignedDoctorId",
                table: "Sessions");
        }
    }
}
