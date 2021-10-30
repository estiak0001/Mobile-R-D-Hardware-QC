using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class updateTopFaultTable5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "MRNDHQC_TopFaultHead",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LUser",
                table: "MRNDHQC_TopFaultHead",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "MRNDHQC_TopFaultHead",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "MRNDHQC_TopFaultHead");

            migrationBuilder.DropColumn(
                name: "LUser",
                table: "MRNDHQC_TopFaultHead");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "MRNDHQC_TopFaultHead");
        }
    }
}
