using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalSchoolManagementSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffRoleEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Designation",
                table: "StaffUsers");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "StaffUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "StaffUsers");

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "StaffUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
