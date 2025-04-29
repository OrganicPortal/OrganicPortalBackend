using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganicPortalBackend.Migrations
{
    /// <inheritdoc />
    public partial class Company : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PhoneTable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PasswordTable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CERTAdditionalModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GerminationRate = table.Column<float>(type: "real", nullable: false),
                    EnergyOfGermination = table.Column<float>(type: "real", nullable: false),
                    PurityPercentage = table.Column<float>(type: "real", nullable: false),
                    MoistureContent = table.Column<float>(type: "real", nullable: false),
                    AbnormalSeedlingsPercentage = table.Column<float>(type: "real", nullable: false),
                    HardSeedsPercentage = table.Column<float>(type: "real", nullable: false),
                    WeedSeedsPercentage = table.Column<float>(type: "real", nullable: false),
                    InertMatterPercentage = table.Column<float>(type: "real", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CERTAdditionalModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CERTModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAddlInfo = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CERTModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyTable",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrustStatus = table.Column<int>(type: "int", nullable: false),
                    LegalType = table.Column<int>(type: "int", nullable: false),
                    EstablishmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isArchivated = table.Column<bool>(type: "bit", nullable: false),
                    ArchivationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyContactTable",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyContactTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyContactTable_CompanyTable_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "CompanyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyTypeOfActivityTable",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyTypeOfActivityTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyTypeOfActivityTable_CompanyTable_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "CompanyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesTable",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeesTable_CompanyTable_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "CompanyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeesTable_UserTable_UserId",
                        column: x => x.UserId,
                        principalTable: "UserTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeedModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScientificName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Variety = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeedType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BatchNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HarvestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TreatmentType = table.Column<int>(type: "int", nullable: false),
                    StorageConditions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AverageWeightThousandSeeds = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeedModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeedModel_CompanyTable_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "CompanyTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UseCERTModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CERTId = table.Column<long>(type: "bigint", nullable: false),
                    CERTAdditionalId = table.Column<long>(type: "bigint", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    SeedModelId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UseCERTModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UseCERTModel_CERTAdditionalModel_CERTAdditionalId",
                        column: x => x.CERTAdditionalId,
                        principalTable: "CERTAdditionalModel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UseCERTModel_CERTModel_CERTId",
                        column: x => x.CERTId,
                        principalTable: "CERTModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UseCERTModel_SeedModel_SeedModelId",
                        column: x => x.SeedModelId,
                        principalTable: "SeedModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CERTFileModel",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CRTId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CERTFileModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CERTFileModel_UseCERTModel_CRTId",
                        column: x => x.CRTId,
                        principalTable: "UseCERTModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CERTFileModel_CRTId",
                table: "CERTFileModel",
                column: "CRTId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyContactTable_CompanyId",
                table: "CompanyContactTable",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyTypeOfActivityTable_CompanyId",
                table: "CompanyTypeOfActivityTable",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesTable_CompanyId",
                table: "EmployeesTable",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesTable_UserId",
                table: "EmployeesTable",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SeedModel_CompanyId",
                table: "SeedModel",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UseCERTModel_CERTAdditionalId",
                table: "UseCERTModel",
                column: "CERTAdditionalId");

            migrationBuilder.CreateIndex(
                name: "IX_UseCERTModel_CERTId",
                table: "UseCERTModel",
                column: "CERTId");

            migrationBuilder.CreateIndex(
                name: "IX_UseCERTModel_SeedModelId",
                table: "UseCERTModel",
                column: "SeedModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CERTFileModel");

            migrationBuilder.DropTable(
                name: "CompanyContactTable");

            migrationBuilder.DropTable(
                name: "CompanyTypeOfActivityTable");

            migrationBuilder.DropTable(
                name: "EmployeesTable");

            migrationBuilder.DropTable(
                name: "UseCERTModel");

            migrationBuilder.DropTable(
                name: "CERTAdditionalModel");

            migrationBuilder.DropTable(
                name: "CERTModel");

            migrationBuilder.DropTable(
                name: "SeedModel");

            migrationBuilder.DropTable(
                name: "CompanyTable");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PhoneTable");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PasswordTable");
        }
    }
}
