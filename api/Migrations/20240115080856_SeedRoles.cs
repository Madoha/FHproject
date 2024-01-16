using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5c14a25e-a017-4bdf-ba2b-f44b9ab7490a", "49fec42f-447a-41d6-bf32-718495cf7b7b", "User", "USER" },
                    { "ca4d1e77-9fdc-49de-896b-87dc1b349b8a", "5b268d38-4007-4b26-bda9-d8b1b453a346", "Admin", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5c14a25e-a017-4bdf-ba2b-f44b9ab7490a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ca4d1e77-9fdc-49de-896b-87dc1b349b8a");
        }
    }
}
