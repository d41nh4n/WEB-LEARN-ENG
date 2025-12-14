using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeResidualEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Decks_Tags_TagEntityId",
                table: "Decks");

            migrationBuilder.DropTable(
                name: "VocabulariesRelation");

            migrationBuilder.DropTable(
                name: "VocabularyProgress");

            migrationBuilder.DropTable(
                name: "VocabularyTags");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Decks_TagEntityId",
                table: "Decks");

            migrationBuilder.DropColumn(
                name: "TagEntityId",
                table: "Decks");

            migrationBuilder.AddColumn<int>(
                name: "Point",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Point",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "TagEntityId",
                table: "Decks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NameTag = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VocabulariesRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelateVocabId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceVocabId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VocabRelationType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabulariesRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabulariesRelation_Vocabularies_RelateVocabId",
                        column: x => x.RelateVocabId,
                        principalTable: "Vocabularies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VocabulariesRelation_Vocabularies_SourceVocabId",
                        column: x => x.SourceVocabId,
                        principalTable: "Vocabularies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VocabularyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityType = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VocabularyProgress_Vocabularies_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "Vocabularies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VocabularyTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VocabularyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VocabularyTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VocabularyTags_Vocabularies_VocabularyId",
                        column: x => x.VocabularyId,
                        principalTable: "Vocabularies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Decks_TagEntityId",
                table: "Decks",
                column: "TagEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabulariesRelation_RelateVocabId",
                table: "VocabulariesRelation",
                column: "RelateVocabId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabulariesRelation_SourceVocabId",
                table: "VocabulariesRelation",
                column: "SourceVocabId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyProgress_UserId",
                table: "VocabularyProgress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyProgress_VocabularyId",
                table: "VocabularyProgress",
                column: "VocabularyId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyTags_TagId",
                table: "VocabularyTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_VocabularyTags_VocabularyId",
                table: "VocabularyTags",
                column: "VocabularyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Decks_Tags_TagEntityId",
                table: "Decks",
                column: "TagEntityId",
                principalTable: "Tags",
                principalColumn: "Id");
        }
    }
}
