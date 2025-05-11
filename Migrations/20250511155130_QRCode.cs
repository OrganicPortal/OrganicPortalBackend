using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganicPortalBackend.Migrations
{
    /// <inheritdoc />
    public partial class QRCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolanaQrCodeTable",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QrBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolanaSeedId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolanaQrCodeTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolanaQrCodeTable_SolanaSeedTable_SolanaSeedId",
                        column: x => x.SolanaSeedId,
                        principalTable: "SolanaSeedTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolanaQrCodeTable_SolanaSeedId",
                table: "SolanaQrCodeTable",
                column: "SolanaSeedId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolanaQrCodeTable");
        }
    }
}
