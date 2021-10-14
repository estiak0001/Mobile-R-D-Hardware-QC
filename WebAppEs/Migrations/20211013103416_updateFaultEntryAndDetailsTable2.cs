using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class updateFaultEntryAndDetailsTable2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MobileRNDFaultDetails_MRNDQC_SubCategory_SubCategoryID",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropIndex(
                name: "IX_MobileRNDFaultDetails_SubCategoryID",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropColumn(
                name: "FaultQty",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropColumn(
                name: "SubCategoryID",
                table: "MobileRNDFaultDetails");

            migrationBuilder.CreateTable(
                name: "MobileRNDFaultSubDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FaultDetailsID = table.Column<Guid>(nullable: false),
                    SubCategoryID = table.Column<Guid>(nullable: false),
                    FaultQty = table.Column<int>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileRNDFaultSubDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MobileRNDFaultSubDetails_MobileRNDFaultDetails_FaultDetailsID",
                        column: x => x.FaultDetailsID,
                        principalTable: "MobileRNDFaultDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MobileRNDFaultSubDetails_MRNDQC_SubCategory_SubCategoryID",
                        column: x => x.SubCategoryID,
                        principalTable: "MRNDQC_SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MobileRNDFaultSubDetails_FaultDetailsID",
                table: "MobileRNDFaultSubDetails",
                column: "FaultDetailsID");

            migrationBuilder.CreateIndex(
                name: "IX_MobileRNDFaultSubDetails_SubCategoryID",
                table: "MobileRNDFaultSubDetails",
                column: "SubCategoryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MobileRNDFaultSubDetails");

            migrationBuilder.AddColumn<int>(
                name: "FaultQty",
                table: "MobileRNDFaultDetails",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SubCategoryID",
                table: "MobileRNDFaultDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MobileRNDFaultDetails_SubCategoryID",
                table: "MobileRNDFaultDetails",
                column: "SubCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_MobileRNDFaultDetails_MRNDQC_SubCategory_SubCategoryID",
                table: "MobileRNDFaultDetails",
                column: "SubCategoryID",
                principalTable: "MRNDQC_SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
