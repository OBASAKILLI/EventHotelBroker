using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHotelBroker.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingRejectionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Bookings",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Bookings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Bookings");
        }
    }
}
