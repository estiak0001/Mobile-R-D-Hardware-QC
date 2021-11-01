using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class faultentityUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCheckedQty",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.AddColumn<int>(
                name: "TotalIssueQty",
                table: "MobileRNDFaultsEntry",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalIssueQty",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.AddColumn<int>(
                name: "TotalCheckedQty",
                table: "MobileRNDFaultsEntry",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);
        }
    }
}
