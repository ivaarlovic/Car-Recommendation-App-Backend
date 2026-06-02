using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRecommendationApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCompletedToSurveyUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "SurveyUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "SurveyUsers");
        }
    }
}
