using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddContractIdToService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Contracts_ContractID",
                schema: "public",
                table: "Services");

            migrationBuilder.AlterColumn<int>(
                name: "ContractID",
                schema: "public",
                table: "Services",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Contracts_ContractID",
                schema: "public",
                table: "Services",
                column: "ContractID",
                principalSchema: "public",
                principalTable: "Contracts",
                principalColumn: "ContractID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Contracts_ContractID",
                schema: "public",
                table: "Services");

            migrationBuilder.AlterColumn<int>(
                name: "ContractID",
                schema: "public",
                table: "Services",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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
    }
}
