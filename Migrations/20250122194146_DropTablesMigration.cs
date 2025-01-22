using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMS.Migrations
{
    /// <inheritdoc />
    public partial class DropTablesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                columns: new[] { "DateCreated", "Password" },
                values: new object[] { new DateTime(2025, 1, 23, 3, 41, 45, 658, DateTimeKind.Local).AddTicks(8961), "$2a$11$7Gnf.Os1aQswoCUsRUDar.xG0QLZcelz9CtetHvpSljryWBwcFory" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1,
                columns: new[] { "DateCreated", "Password" },
                values: new object[] { new DateTime(2025, 1, 23, 3, 40, 19, 177, DateTimeKind.Local).AddTicks(5707), "$2a$11$OBzXcG5O5Ztcn0JZ7zjsQeaGAENGTKM6x431gnlJ3oT2Lkd85rpya" });
        }
    }
}
