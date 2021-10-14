using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class updateFaultEntryAndDetailsTable3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MobileRNDFaultSubDetails");

            migrationBuilder.AddColumn<int>(
                name: "FaultQty",
                table: "MobileRNDFaultDetails",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SubCategoryID",
                table: "MobileRNDFaultDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FaultDetailsID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FaultQty = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    SubCategoryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
    }
}
