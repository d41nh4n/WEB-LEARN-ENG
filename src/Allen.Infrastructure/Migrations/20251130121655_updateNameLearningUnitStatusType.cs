using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateNameLearningUnitStatusType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "learningUnitStatusType",
                table: "LearningUnits",
                newName: "LearningUnitStatusType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LearningUnitStatusType",
                table: "LearningUnits",
                newName: "learningUnitStatusType");
        }
    }
}
