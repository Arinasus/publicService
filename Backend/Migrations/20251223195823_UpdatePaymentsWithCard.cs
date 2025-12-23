using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentsWithCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardID",
                schema: "public",
                table: "Payments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CardID",
                schema: "public",
                table: "Payments",
                column: "CardID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Cards_CardID",
                schema: "public",
                table: "Payments",
                column: "CardID",
                principalSchema: "public",
                principalTable: "Cards",
                principalColumn: "CardID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Cards_CardID",
                schema: "public",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CardID",
                schema: "public",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CardID",
                schema: "public",
                table: "Payments");
        }
    }
}
