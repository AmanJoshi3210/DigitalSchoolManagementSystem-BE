using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalSchoolManagementSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                table: "Documents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByStaffId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ReviewedByStaffId",
                table: "Documents",
                column: "ReviewedByStaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_ReviewedByStaffId",
                table: "Documents",
                column: "ReviewedByStaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_ReviewedByStaffId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ReviewedByStaffId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ReviewedByStaffId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Documents");
        }
    }
}
