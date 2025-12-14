using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Allen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePushSubcription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PushSubscriptions_UserId",
                table: "PushSubscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptions_UserId",
                table: "PushSubscriptions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PushSubscriptions_UserId",
                table: "PushSubscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptions_UserId",
                table: "PushSubscriptions",
                column: "UserId",
                unique: true);
        }
    }
}
