using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceToMeterReading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "ContractID",
                schema: "public",
                table: "Services",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServiceID",
                schema: "public",
                table: "MeterReadings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Services_ContractID",
                schema: "public",
                table: "Services",
                column: "ContractID");

            migrationBuilder.CreateIndex(
                name: "IX_MeterReadings_ServiceID",
                schema: "public",
                table: "MeterReadings",
                column: "ServiceID");

            migrationBuilder.AddForeignKey(
                name: "FK_MeterReadings_Services_ServiceID",
                schema: "public",
                table: "MeterReadings",
                column: "ServiceID",
                principalSchema: "public",
                principalTable: "Services",
                principalColumn: "ServiceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Contracts_ContractID",
                schema: "public",
                table: "Services",
                column: "ContractID",
                principalSchema: "public",
                principalTable: "Contracts",
                principalColumn: "ContractID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MeterReadings_Services_ServiceID",
                schema: "public",
                table: "MeterReadings");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Contracts_ContractID",
                schema: "public",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_ContractID",
                schema: "public",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_MeterReadings_ServiceID",
                schema: "public",
                table: "MeterReadings");

            migrationBuilder.DropColumn(
                name: "ContractID",
                schema: "public",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServiceID",
                schema: "public",
                table: "MeterReadings");
        }
    }
}
