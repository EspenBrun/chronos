using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Chronos.Migrations
{
    public partial class TimeBlock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeBlocks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    In = table.Column<DateTime>(nullable: false),
                    Out = table.Column<DateTime>(nullable: false),
                    Worked = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeBlocks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeBlocks");
        }
    }
}
