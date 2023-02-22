using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SimpleDLock.Core.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FieldName = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    BookedTime = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booking", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Field",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => x.Name);
                });

            migrationBuilder.InsertData(
                table: "Field",
                column: "Name",
                values: new object[]
                {
                    "Field 01",
                    "Field 16",
                    "Field 15",
                    "Field 14",
                    "Field 13",
                    "Field 12",
                    "Field 11",
                    "Field 10",
                    "Field 09",
                    "Field 08",
                    "Field 07",
                    "Field 06",
                    "Field 05",
                    "Field 04",
                    "Field 03",
                    "Field 02",
                    "Field 17",
                    "Field 18"
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Booking");

            migrationBuilder.DropTable(
                name: "Field");
        }
    }
}
