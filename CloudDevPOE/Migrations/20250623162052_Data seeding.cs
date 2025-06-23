using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CloudDevPOE.Migrations
{
    /// <inheritdoc />
    public partial class Dataseeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "EventType",
                newName: "Name");

            migrationBuilder.InsertData(
                table: "EventType",
                columns: new[] { "EventTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "Comedy" },
                    { 2, "Concert" },
                    { 3, "Conference" },
                    { 4, "eSports" },
                    { 5, "Sports" },
                    { 6, "Theater" },
                    { 7, "Workshop" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "EventType",
                keyColumn: "EventTypeId",
                keyValue: 7);

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "EventType",
                newName: "Description");
        }
    }
}
