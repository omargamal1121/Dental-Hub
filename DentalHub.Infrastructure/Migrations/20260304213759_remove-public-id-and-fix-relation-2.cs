using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removepublicidandfixrelation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropColumn(name: "PublicId", table: "Universities"); migrationBuilder.DropColumn(name: "UserId", table: "Students"); migrationBuilder.DropColumn(name: "PublicId", table: "Students"); migrationBuilder.DropColumn(name: "PublicId", table: "Sessions"); migrationBuilder.DropColumn(name: "PublicId", table: "SessionNotes"); migrationBuilder.DropColumn(name: "UserId", table: "Patients"); migrationBuilder.DropColumn(name: "PublicId", table: "Patients"); migrationBuilder.DropColumn(name: "PublicId", table: "PatientCases"); migrationBuilder.DropColumn(name: "PublicId", table: "Medias"); migrationBuilder.DropColumn(name: "UserId", table: "Doctors"); migrationBuilder.DropColumn(name: "PublicId", table: "Doctors"); migrationBuilder.DropColumn(name: "PublicId", table: "CaseTypes"); migrationBuilder.DropColumn(name: "PublicId", table: "CaseRequests"); migrationBuilder.DropColumn(name: "PublicId", table: "AspNetUsers"); migrationBuilder.DropColumn(name: "UserId", table: "Admins"); migrationBuilder.DropColumn(name: "PublicId", table: "Admins");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
