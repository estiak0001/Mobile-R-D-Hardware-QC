using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class updatefaultentry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotaCheckedQty",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.AddColumn<string>(
                name: "Shift",
                table: "MobileRNDFaultsEntry",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalCheckedQty",
                table: "MobileRNDFaultsEntry",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shift",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "TotalCheckedQty",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.AddColumn<int>(
                name: "TotaCheckedQty",
                table: "MobileRNDFaultsEntry",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);
        }
    }
}
