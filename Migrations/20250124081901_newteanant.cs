using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMS.Migrations
{
    /// <inheritdoc />
    public partial class newteanant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "LeaseID",
                table: "Tenants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnitID",
                table: "Tenants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Tenants",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_LeaseID",
                table: "Tenants",
                column: "LeaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_UnitID",
                table: "Tenants",
                column: "UnitID");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_UserID",
                table: "Tenants",
                column: "UserID",
                unique: true,
                filter: "[UserID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Leases_LeaseID",
                table: "Tenants",
                column: "LeaseID",
                principalTable: "Leases",
                principalColumn: "LeaseID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Units_UnitID",
                table: "Tenants",
                column: "UnitID",
                principalTable: "Units",
                principalColumn: "UnitID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Users_UserID",
                table: "Tenants",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Leases_LeaseID",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Units_UnitID",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Users_UserID",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_LeaseID",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_UnitID",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_UserID",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "LeaseID",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "UnitID",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Tenants");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
