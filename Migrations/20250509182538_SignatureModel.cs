using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganicPortalBackend.Migrations
{
    /// <inheritdoc />
    public partial class SignatureModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Signature",
                table: "SolanaSeedTable");

            migrationBuilder.CreateTable(
                name: "SignatureTablse",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolanaSeedId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignatureTablse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignatureTablse_SolanaSeedTable_SolanaSeedId",
                        column: x => x.SolanaSeedId,
                        principalTable: "SolanaSeedTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SignatureTablse_SolanaSeedId",
                table: "SignatureTablse",
                column: "SolanaSeedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignatureTablse");

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                table: "SolanaSeedTable",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
