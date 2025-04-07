using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPBot.API.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckForTwitterLinksFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CheckForTwitterLinks",
                table: "DiscordGuilds",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckForTwitterLinks",
                table: "DiscordGuilds");
        }
    }
}
