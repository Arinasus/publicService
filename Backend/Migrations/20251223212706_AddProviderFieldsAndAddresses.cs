using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderFieldsAndAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BIC",
                schema: "public",
                table: "Providers",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IBAN",
                schema: "public",
                table: "Providers",
                type: "character varying(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UNP",
                schema: "public",
                table: "Providers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Apartment",
                schema: "public",
                table: "Addresses",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "House",
                schema: "public",
                table: "Addresses",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BIC",
                schema: "public",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "IBAN",
                schema: "public",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "UNP",
                schema: "public",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Apartment",
                schema: "public",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "House",
                schema: "public",
                table: "Addresses");
        }
    }
}
