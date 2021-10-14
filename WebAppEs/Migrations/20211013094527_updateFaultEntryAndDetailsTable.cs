using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class updateFaultEntryAndDetailsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AesthMaterialFault",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "AesthProductionFault",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "FuncMaterialFault",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "FuncProductionFault",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "FuncSoftwareFault",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "TotalAestheticFault",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "TotalFunctionalFault",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "ApplicationUserID",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropColumn(
                name: "FaultType",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropColumn(
                name: "RootCause",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropColumn(
                name: "Solution",
                table: "MobileRNDFaultDetails");

            migrationBuilder.AddColumn<Guid>(
                name: "LUser",
                table: "MobileRNDFaultsEntry",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "QCPass",
                table: "MobileRNDFaultsEntry",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Shipment",
                table: "MobileRNDFaultsEntry",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeOfProduction",
                table: "MobileRNDFaultsEntry",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryID",
                table: "MobileRNDFaultDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LUser",
                table: "MobileRNDFaultDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SubCategoryID",
                table: "MobileRNDFaultDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MobileRNDFaultDetails_CategoryID",
                table: "MobileRNDFaultDetails",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_MobileRNDFaultDetails_SubCategoryID",
                table: "MobileRNDFaultDetails",
                column: "SubCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_MobileRNDFaultDetails_MRNDQC_Category_CategoryID",
                table: "MobileRNDFaultDetails",
                column: "CategoryID",
                principalTable: "MRNDQC_Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MobileRNDFaultDetails_MRNDQC_SubCategory_SubCategoryID",
                table: "MobileRNDFaultDetails",
                column: "SubCategoryID",
                principalTable: "MRNDQC_SubCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MobileRNDFaultDetails_MRNDQC_Category_CategoryID",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MobileRNDFaultDetails_MRNDQC_SubCategory_SubCategoryID",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropIndex(
                name: "IX_MobileRNDFaultDetails_CategoryID",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropIndex(
                name: "IX_MobileRNDFaultDetails_SubCategoryID",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropColumn(
                name: "LUser",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "QCPass",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "Shipment",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "TypeOfProduction",
                table: "MobileRNDFaultsEntry");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropColumn(
                name: "LUser",
                table: "MobileRNDFaultDetails");

            migrationBuilder.DropColumn(
                name: "SubCategoryID",
                table: "MobileRNDFaultDetails");

            migrationBuilder.AddColumn<int>(
                name: "AesthMaterialFault",
                table: "MobileRNDFaultsEntry",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AesthProductionFault",
                table: "MobileRNDFaultsEntry",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FuncMaterialFault",
                table: "MobileRNDFaultsEntry",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FuncProductionFault",
                table: "MobileRNDFaultsEntry",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FuncSoftwareFault",
                table: "MobileRNDFaultsEntry",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalAestheticFault",
                table: "MobileRNDFaultsEntry",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalFunctionalFault",
                table: "MobileRNDFaultsEntry",
                type: "int",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                table: "MobileRNDFaultsEntry",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserID",
                table: "MobileRNDFaultDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FaultType",
                table: "MobileRNDFaultDetails",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "MobileRNDFaultDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RootCause",
                table: "MobileRNDFaultDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Solution",
                table: "MobileRNDFaultDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
