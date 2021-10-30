using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppEs.Migrations
{
    public partial class AddTopFaultAnalysis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MRNDHQC_TopFaultAnalysis",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EmployeeID = table.Column<string>(maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(maxLength: 50, nullable: false),
                    PartsModelID = table.Column<Guid>(maxLength: 50, nullable: false),
                    CategoryID = table.Column<Guid>(maxLength: 150, nullable: false),
                    SubCategoryID = table.Column<Guid>(maxLength: 150, nullable: false),
                    Reason = table.Column<string>(nullable: false),
                    Sample = table.Column<int>(maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(nullable: false),
                    ProblemSolAndRec = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    LUser = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MRNDHQC_TopFaultAnalysis", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MRNDHQC_TopFaultAnalysis");
        }
    }
}
