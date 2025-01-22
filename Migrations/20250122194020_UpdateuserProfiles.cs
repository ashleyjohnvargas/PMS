using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateuserProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "Address", "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { 1, null, "test@example.com", "Test", "User", null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "ConfirmPassword", "DateCreated", "Email", "FirstName", "IsActive", "LastName", "Password", "Role", "TermsAndConditions" },
                values: new object[] { 1, null, new DateTime(2025, 1, 23, 3, 40, 19, 177, DateTimeKind.Local).AddTicks(5707), "test@example.com", "Test", true, "User", "$2a$11$OBzXcG5O5Ztcn0JZ7zjsQeaGAENGTKM6x431gnlJ3oT2Lkd85rpya", "Tenant", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
