using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRowInQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndTextIndex",
                table: "SubQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTranscriptIndex",
                table: "SubQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "SubQuestions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTextIndex",
                table: "SubQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTranscriptIndex",
                table: "SubQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTextIndex",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndTranscriptIndex",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Explanation",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTextIndex",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartTranscriptIndex",
                table: "Questions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTextIndex",
                table: "SubQuestions");

            migrationBuilder.DropColumn(
                name: "EndTranscriptIndex",
                table: "SubQuestions");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "SubQuestions");

            migrationBuilder.DropColumn(
                name: "StartTextIndex",
                table: "SubQuestions");

            migrationBuilder.DropColumn(
                name: "StartTranscriptIndex",
                table: "SubQuestions");

            migrationBuilder.DropColumn(
                name: "EndTextIndex",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "EndTranscriptIndex",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Explanation",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "StartTextIndex",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "StartTranscriptIndex",
                table: "Questions");
        }
    }
}
