using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalSchoolManagementSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SwitchFileStorageToCloudinary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StoragePath",
                table: "FileStorages",
                newName: "Url");

            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "FileStorages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "FileStorages");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "FileStorages",
                newName: "StoragePath");
        }
    }
}
