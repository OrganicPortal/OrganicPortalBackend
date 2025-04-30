using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganicPortalBackend.Migrations
{
    /// <inheritdoc />
    public partial class SeedCERTInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CERTFileModel_UseCERTModel_CRTId",
                table: "CERTFileModel");

            migrationBuilder.DropForeignKey(
                name: "FK_SeedModel_CompanyTable_CompanyId",
                table: "SeedModel");

            migrationBuilder.DropForeignKey(
                name: "FK_UseCERTModel_CERTAdditionalModel_CERTAdditionalId",
                table: "UseCERTModel");

            migrationBuilder.DropForeignKey(
                name: "FK_UseCERTModel_CERTModel_CERTId",
                table: "UseCERTModel");

            migrationBuilder.DropForeignKey(
                name: "FK_UseCERTModel_SeedModel_SeedModelId",
                table: "UseCERTModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UseCERTModel",
                table: "UseCERTModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeedModel",
                table: "SeedModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CERTModel",
                table: "CERTModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CERTFileModel",
                table: "CERTFileModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CERTAdditionalModel",
                table: "CERTAdditionalModel");

            migrationBuilder.RenameTable(
                name: "UseCERTModel",
                newName: "UseCERTTable");

            migrationBuilder.RenameTable(
                name: "SeedModel",
                newName: "SeedTable");

            migrationBuilder.RenameTable(
                name: "CERTModel",
                newName: "CERTTable");

            migrationBuilder.RenameTable(
                name: "CERTFileModel",
                newName: "CERTFileTable");

            migrationBuilder.RenameTable(
                name: "CERTAdditionalModel",
                newName: "CERTAdditionalTable");

            migrationBuilder.RenameIndex(
                name: "IX_UseCERTModel_SeedModelId",
                table: "UseCERTTable",
                newName: "IX_UseCERTTable_SeedModelId");

            migrationBuilder.RenameIndex(
                name: "IX_UseCERTModel_CERTId",
                table: "UseCERTTable",
                newName: "IX_UseCERTTable_CERTId");

            migrationBuilder.RenameIndex(
                name: "IX_UseCERTModel_CERTAdditionalId",
                table: "UseCERTTable",
                newName: "IX_UseCERTTable_CERTAdditionalId");

            migrationBuilder.RenameIndex(
                name: "IX_SeedModel_CompanyId",
                table: "SeedTable",
                newName: "IX_SeedTable_CompanyId");

            migrationBuilder.RenameColumn(
                name: "CRTId",
                table: "CERTFileTable",
                newName: "UseCERTId");

            migrationBuilder.RenameIndex(
                name: "IX_CERTFileModel_CRTId",
                table: "CERTFileTable",
                newName: "IX_CERTFileTable_UseCERTId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CERTAdditionalTable",
                newName: "Note");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "UseCERTTable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CERTFileTable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "CERTAdditionalTable",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "CERTAdditionalTable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UseCERTTable",
                table: "UseCERTTable",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeedTable",
                table: "SeedTable",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CERTTable",
                table: "CERTTable",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CERTFileTable",
                table: "CERTFileTable",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CERTAdditionalTable",
                table: "CERTAdditionalTable",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CERTFileTable_UseCERTTable_UseCERTId",
                table: "CERTFileTable",
                column: "UseCERTId",
                principalTable: "UseCERTTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SeedTable_CompanyTable_CompanyId",
                table: "SeedTable",
                column: "CompanyId",
                principalTable: "CompanyTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UseCERTTable_CERTAdditionalTable_CERTAdditionalId",
                table: "UseCERTTable",
                column: "CERTAdditionalId",
                principalTable: "CERTAdditionalTable",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UseCERTTable_CERTTable_CERTId",
                table: "UseCERTTable",
                column: "CERTId",
                principalTable: "CERTTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UseCERTTable_SeedTable_SeedModelId",
                table: "UseCERTTable",
                column: "SeedModelId",
                principalTable: "SeedTable",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CERTFileTable_UseCERTTable_UseCERTId",
                table: "CERTFileTable");

            migrationBuilder.DropForeignKey(
                name: "FK_SeedTable_CompanyTable_CompanyId",
                table: "SeedTable");

            migrationBuilder.DropForeignKey(
                name: "FK_UseCERTTable_CERTAdditionalTable_CERTAdditionalId",
                table: "UseCERTTable");

            migrationBuilder.DropForeignKey(
                name: "FK_UseCERTTable_CERTTable_CERTId",
                table: "UseCERTTable");

            migrationBuilder.DropForeignKey(
                name: "FK_UseCERTTable_SeedTable_SeedModelId",
                table: "UseCERTTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UseCERTTable",
                table: "UseCERTTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeedTable",
                table: "SeedTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CERTTable",
                table: "CERTTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CERTFileTable",
                table: "CERTFileTable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CERTAdditionalTable",
                table: "CERTAdditionalTable");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "UseCERTTable");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CERTFileTable");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "CERTAdditionalTable");

            migrationBuilder.RenameTable(
                name: "UseCERTTable",
                newName: "UseCERTModel");

            migrationBuilder.RenameTable(
                name: "SeedTable",
                newName: "SeedModel");

            migrationBuilder.RenameTable(
                name: "CERTTable",
                newName: "CERTModel");

            migrationBuilder.RenameTable(
                name: "CERTFileTable",
                newName: "CERTFileModel");

            migrationBuilder.RenameTable(
                name: "CERTAdditionalTable",
                newName: "CERTAdditionalModel");

            migrationBuilder.RenameIndex(
                name: "IX_UseCERTTable_SeedModelId",
                table: "UseCERTModel",
                newName: "IX_UseCERTModel_SeedModelId");

            migrationBuilder.RenameIndex(
                name: "IX_UseCERTTable_CERTId",
                table: "UseCERTModel",
                newName: "IX_UseCERTModel_CERTId");

            migrationBuilder.RenameIndex(
                name: "IX_UseCERTTable_CERTAdditionalId",
                table: "UseCERTModel",
                newName: "IX_UseCERTModel_CERTAdditionalId");

            migrationBuilder.RenameIndex(
                name: "IX_SeedTable_CompanyId",
                table: "SeedModel",
                newName: "IX_SeedModel_CompanyId");

            migrationBuilder.RenameColumn(
                name: "UseCERTId",
                table: "CERTFileModel",
                newName: "CRTId");

            migrationBuilder.RenameIndex(
                name: "IX_CERTFileTable_UseCERTId",
                table: "CERTFileModel",
                newName: "IX_CERTFileModel_CRTId");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "CERTAdditionalModel",
                newName: "Description");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "CERTAdditionalModel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UseCERTModel",
                table: "UseCERTModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeedModel",
                table: "SeedModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CERTModel",
                table: "CERTModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CERTFileModel",
                table: "CERTFileModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CERTAdditionalModel",
                table: "CERTAdditionalModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CERTFileModel_UseCERTModel_CRTId",
                table: "CERTFileModel",
                column: "CRTId",
                principalTable: "UseCERTModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SeedModel_CompanyTable_CompanyId",
                table: "SeedModel",
                column: "CompanyId",
                principalTable: "CompanyTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UseCERTModel_CERTAdditionalModel_CERTAdditionalId",
                table: "UseCERTModel",
                column: "CERTAdditionalId",
                principalTable: "CERTAdditionalModel",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UseCERTModel_CERTModel_CERTId",
                table: "UseCERTModel",
                column: "CERTId",
                principalTable: "CERTModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UseCERTModel_SeedModel_SeedModelId",
                table: "UseCERTModel",
                column: "SeedModelId",
                principalTable: "SeedModel",
                principalColumn: "Id");
        }
    }
}
