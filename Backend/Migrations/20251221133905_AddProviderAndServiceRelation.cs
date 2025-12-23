using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderAndServiceRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProviderID",
                schema: "public",
                table: "Services",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ProviderID",
                schema: "public",
                table: "Services",
                column: "ProviderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Providers_ProviderID",
                schema: "public",
                table: "Services",
                column: "ProviderID",
                principalSchema: "public",
                principalTable: "Providers",
                principalColumn: "ProviderID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Providers_ProviderID",
                schema: "public",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ProviderID",
                schema: "public",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ProviderID",
                schema: "public",
                table: "Services");
        }
    }
}
