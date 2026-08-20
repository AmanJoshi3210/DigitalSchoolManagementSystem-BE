using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalSchoolManagementSystem.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaffPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffUserId = table.Column<int>(type: "int", nullable: false),
                    PermissionKey = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffPermissions_StaffUsers_StaffUserId",
                        column: x => x.StaffUserId,
                        principalTable: "StaffUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffPermissions_StaffUserId_PermissionKey",
                table: "StaffPermissions",
                columns: new[] { "StaffUserId", "PermissionKey" },
                unique: true);

            // Backfill: every staff user who already existed before this feature shipped keeps
            // full access to the pages they could already reach (StaffRole 4 = Admin is excluded
            // - Admins never get explicit rows, they bypass via the staffRole claim instead).
            // Staff registered after this migration start with zero permissions until an Admin
            // grants them.
            migrationBuilder.Sql(@"
                INSERT INTO StaffPermissions (StaffUserId, PermissionKey, CreatedAt, IsActive)
                SELECT su.Id, k.PermissionKey, GETUTCDATE(), 1
                FROM StaffUsers su
                INNER JOIN Users u ON u.Id = su.UserId
                CROSS JOIN (VALUES ('ActionHub'), ('Students'), ('Programs'), ('Messages')) AS k(PermissionKey)
                WHERE su.Role <> 4 AND su.IsActive = 1 AND u.IsActive = 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // DropTable removes the backfilled rows along with the table - no separate DELETE needed.
            migrationBuilder.DropTable(
                name: "StaffPermissions");
        }
    }
}
