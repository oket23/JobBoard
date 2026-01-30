using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobBoard.Recruitment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRecruitment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "UserApplication",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserFirstName",
                table: "UserApplication",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "UserApplication");

            migrationBuilder.DropColumn(
                name: "UserFirstName",
                table: "UserApplication");
        }
    }
}
