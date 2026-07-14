using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecommendationService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "ServiceRecommendations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "EmployeeRecommendations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "CompanyRecommendations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "BranchRecommendations",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "ServiceRecommendations");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "EmployeeRecommendations");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "CompanyRecommendations");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "BranchRecommendations");
        }
    }
}
