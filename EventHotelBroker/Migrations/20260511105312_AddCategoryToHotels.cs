using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHotelBroker.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToHotels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Hotels",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Hotel")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Hotels");
        }
    }
}
