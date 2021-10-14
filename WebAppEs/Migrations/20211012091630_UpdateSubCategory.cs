using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class UpdateSubCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryID",
                table: "MRNDQC_SubCategory",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MRNDQC_SubCategory_CategoryID",
                table: "MRNDQC_SubCategory",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_MRNDQC_SubCategory_MRNDQC_Category_CategoryID",
                table: "MRNDQC_SubCategory",
                column: "CategoryID",
                principalTable: "MRNDQC_Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MRNDQC_SubCategory_MRNDQC_Category_CategoryID",
                table: "MRNDQC_SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_MRNDQC_SubCategory_CategoryID",
                table: "MRNDQC_SubCategory");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "MRNDQC_SubCategory");
        }
    }
}
