using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalSchoolManagementSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfilePhotoDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DocumentId",
                table: "Users",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Documents_DocumentId",
                table: "Users",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Documents_DocumentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DocumentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Users");
        }
    }
}
