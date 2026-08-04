using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DentalHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeiprefreshtoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPAdress",
                table: "RefreshTokens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IPAdress",
                table: "RefreshTokens",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
