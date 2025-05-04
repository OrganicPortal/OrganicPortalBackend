using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganicPortalBackend.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UseCERTTable_SeedTable_SeedModelId",
                table: "UseCERTTable");

            migrationBuilder.DropIndex(
                name: "IX_UseCERTTable_SeedModelId",
                table: "UseCERTTable");

            migrationBuilder.DropColumn(
                name: "SeedModelId",
                table: "UseCERTTable");

            migrationBuilder.AddColumn<long>(
                name: "SeedId",
                table: "UseCERTTable",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_UseCERTTable_SeedId",
                table: "UseCERTTable",
                column: "SeedId");

            migrationBuilder.AddForeignKey(
                name: "FK_UseCERTTable_SeedTable_SeedId",
                table: "UseCERTTable",
                column: "SeedId",
                principalTable: "SeedTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UseCERTTable_SeedTable_SeedId",
                table: "UseCERTTable");

            migrationBuilder.DropIndex(
                name: "IX_UseCERTTable_SeedId",
                table: "UseCERTTable");

            migrationBuilder.DropColumn(
                name: "SeedId",
                table: "UseCERTTable");

            migrationBuilder.AddColumn<long>(
                name: "SeedModelId",
                table: "UseCERTTable",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UseCERTTable_SeedModelId",
                table: "UseCERTTable",
                column: "SeedModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_UseCERTTable_SeedTable_SeedModelId",
                table: "UseCERTTable",
                column: "SeedModelId",
                principalTable: "SeedTable",
                principalColumn: "Id");
        }
    }
}
