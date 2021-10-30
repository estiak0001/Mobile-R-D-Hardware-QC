using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class updateTopFaultTable4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnalysisType",
                table: "MRNDHQC_TopFaultAnalysis");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "MRNDHQC_TopFaultAnalysis");

            migrationBuilder.DropColumn(
                name: "EmployeeID",
                table: "MRNDHQC_TopFaultAnalysis");

            migrationBuilder.DropColumn(
                name: "PartsModelID",
                table: "MRNDHQC_TopFaultAnalysis");

            migrationBuilder.AddColumn<Guid>(
                name: "HeadID",
                table: "MRNDHQC_TopFaultAnalysis",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MRNDHQC_TopFaultAnalysis_HeadID",
                table: "MRNDHQC_TopFaultAnalysis",
                column: "HeadID");

            migrationBuilder.AddForeignKey(
                name: "FK_MRNDHQC_TopFaultAnalysis_MRNDHQC_TopFaultHead_HeadID",
                table: "MRNDHQC_TopFaultAnalysis",
                column: "HeadID",
                principalTable: "MRNDHQC_TopFaultHead",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MRNDHQC_TopFaultAnalysis_MRNDHQC_TopFaultHead_HeadID",
                table: "MRNDHQC_TopFaultAnalysis");

            migrationBuilder.DropIndex(
                name: "IX_MRNDHQC_TopFaultAnalysis_HeadID",
                table: "MRNDHQC_TopFaultAnalysis");

            migrationBuilder.DropColumn(
                name: "HeadID",
                table: "MRNDHQC_TopFaultAnalysis");

            migrationBuilder.AddColumn<string>(
                name: "AnalysisType",
                table: "MRNDHQC_TopFaultAnalysis",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "MRNDHQC_TopFaultAnalysis",
                type: "datetime2",
                maxLength: 50,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EmployeeID",
                table: "MRNDHQC_TopFaultAnalysis",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PartsModelID",
                table: "MRNDHQC_TopFaultAnalysis",
                type: "uniqueidentifier",
                maxLength: 50,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
