using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHotelBroker.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCustomizableToEventPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustomizable",
                table: "EventPackages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCustomizable",
                table: "EventPackages");
        }
    }
}
