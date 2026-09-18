using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHotelBroker.Migrations
{
    public partial class AddHotelCategoryTableAndColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HotelCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rating = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ServiceFee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "HotelCategories",
                columns: new[] { "Id", "Name", "Rating", "ServiceFee", "Description", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    { 1, "Standard Hotel", "3-Star", 500m, "Standard comfortable accommodation", true, DateTime.UtcNow },
                    { 2, "Premium Hotel", "4-Star", 750m, "Premium luxury hotel with enhanced amenities", true, DateTime.UtcNow },
                    { 3, "Luxury Resort", "5-Star", 1000m, "Five-star world-class luxury resort and safari lodge", true, DateTime.UtcNow },
                    { 4, "Boutique & Villa", "Boutique", 600m, "Intimate boutique hotel or private villa", true, DateTime.UtcNow },
                    { 5, "Budget Lodge", "Budget", 300m, "Affordable safari lodge or guest house", true, DateTime.UtcNow }
                });

            migrationBuilder.AddColumn<int>(
                name: "HotelCategoryId",
                table: "Hotels",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_HotelCategoryId",
                table: "Hotels",
                column: "HotelCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_HotelCategories_HotelCategoryId",
                table: "Hotels",
                column: "HotelCategoryId",
                principalTable: "HotelCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_HotelCategories_HotelCategoryId",
                table: "Hotels");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_HotelCategoryId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "HotelCategoryId",
                table: "Hotels");

            migrationBuilder.DropTable(
                name: "HotelCategories");
        }
    }
}