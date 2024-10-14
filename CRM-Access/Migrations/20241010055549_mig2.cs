using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM_Access.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyDomain",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CompanyDomain", "CompanyName", "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { null, null, "2ec4c041-75f9-4859-a105-212ef88b6540", new DateTime(2024, 10, 10, 9, 55, 47, 915, DateTimeKind.Local).AddTicks(5983), "AQAAAAIAAYagAAAAEK0kmpu3zSp7EXOMfMJY1X66EHYrFALaJ286wv3/avVaHFqDfg9PPaqulC23XnvK4g==", "ba64ae7f-b786-49b1-9797-9e4e3ae35641" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyDomain",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a4397ac4-ecf2-4932-b07d-fa676a30a855", new DateTime(2024, 9, 29, 12, 26, 21, 438, DateTimeKind.Local).AddTicks(7026), "AQAAAAIAAYagAAAAEACydWUBkCprMSf0fprHGM/EDNbg1VuhsUjTp81nefgIvbJkEEb7WFEPh8biqlJ7HA==", "20474885-19fa-4202-9127-0c8de3f1ea81" });
        }
    }
}
