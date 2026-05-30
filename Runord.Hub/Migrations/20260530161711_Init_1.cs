using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Runord.Hub.Migrations
{
    /// <inheritdoc />
    public partial class Init_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BucketName",
                table: "TaskFiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ObjectName",
                table: "TaskFiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BucketName",
                table: "TaskFiles");

            migrationBuilder.DropColumn(
                name: "ObjectName",
                table: "TaskFiles");
        }
    }
}
