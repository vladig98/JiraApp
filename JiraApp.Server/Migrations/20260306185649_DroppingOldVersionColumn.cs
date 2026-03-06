using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JiraApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class DroppingOldVersionColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Tasks",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
