using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMS.Migrations
{
    /// <inheritdoc />
    public partial class updateDbcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserID",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "Address", "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { 1, null, "test@example.com", "Test", "User", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "ConfirmPassword", "DateCreated", "Email", "FirstName", "IsActive", "LastName", "Password", "Role", "TermsAndConditions" },
                values: new object[] { 1, null, new DateTime(2025, 1, 23, 3, 41, 45, 658, DateTimeKind.Local).AddTicks(8961), "test@example.com", "Test", true, "User", "$2a$11$7Gnf.Os1aQswoCUsRUDar.xG0QLZcelz9CtetHvpSljryWBwcFory", "Tenant", null });
        }
    }
}
