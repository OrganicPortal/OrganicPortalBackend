using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganicPortalBackend.Migrations
{
    /// <inheritdoc />
    public partial class DataAndKeysFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedDate",
                table: "UseCERTTable",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HistoryKey",
                table: "SeedTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerifiedDate",
                table: "UseCERTTable");

            migrationBuilder.DropColumn(
                name: "HistoryKey",
                table: "SeedTable");
        }
    }
}
