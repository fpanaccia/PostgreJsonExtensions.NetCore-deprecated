using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestStudio.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Test");

            migrationBuilder.CreateTable(
                name: "Test",
                schema: "Test",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    json = table.Column<string>(type: "jsonb", nullable: false),
                    json2 = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Test", x => x.id);
                });

            migrationBuilder.InsertData(
                schema: "Test",
                table: "Test",
                columns: new[] { "id", "json", "json2" },
                values: new object[,]
                {
                    { new Guid("086b558f-bb80-8645-bc05-27ec1b17b005"), "{\"Str\":\"1234\",\"Fecha\":{\"Date\":\"0001-01-01T00:00:00\"},\"Num\":456.0,\"Logico\":true}", "{\"Str\":\"1234\",\"Fecha\":{\"Date\":\"0001-01-01T00:00:00\"},\"Num\":456.0,\"Logico\":true}" },
                    { new Guid("6a250b45-f183-844a-a5dd-feb33be7f250"), "{\"Str\":\"0456\",\"Fecha\":{\"Date\":\"9999-12-31T23:59:59.9999999\"},\"Num\":789.0,\"Logico\":false}", "{\"Str\":\"0456\",\"Fecha\":{\"Date\":\"9999-12-31T23:59:59.9999999\"},\"Num\":789.0,\"Logico\":false}" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Test",
                schema: "Test");
        }
    }
}
