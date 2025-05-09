using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganicPortalBackend.Migrations
{
    /// <inheritdoc />
    public partial class SolanaSeedModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolanaSeedTable",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HistoryKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountPrivateKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountPublicKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variety = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeedType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TreatmentType = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolanaSeedTable", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolanaSeedTable");
        }
    }
}
