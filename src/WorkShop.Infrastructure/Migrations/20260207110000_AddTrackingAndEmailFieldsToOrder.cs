using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackingAndEmailFieldsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackingStatus",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "Order Placed");

            migrationBuilder.AddColumn<bool>(
                name: "EmailSent",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EmailSent",
                table: "Orders");
        }
    }
}
