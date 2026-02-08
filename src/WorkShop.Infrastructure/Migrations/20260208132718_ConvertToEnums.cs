using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add temporary columns for the enum values
            migrationBuilder.AddColumn<int>(
                name: "RoleTemp",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusTemp",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrackingStatusTemp",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Convert existing data for Users.Role
            migrationBuilder.Sql(@"
                UPDATE ""Users""
                SET ""RoleTemp"" = CASE 
                    WHEN ""Role"" = 'Customer' THEN 0
                    WHEN ""Role"" = 'Admin' THEN 1
                    ELSE 0
                END");

            // Convert existing data for Orders.Status
            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""StatusTemp"" = CASE 
                    WHEN ""Status"" = 'Pending' THEN 0
                    WHEN ""Status"" = 'Completed' THEN 1
                    WHEN ""Status"" = 'Cancelled' THEN 2
                    ELSE 0
                END");

            // Convert existing data for Orders.TrackingStatus
            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""TrackingStatusTemp"" = CASE 
                    WHEN ""TrackingStatus"" = 'Order Placed' THEN 0
                    WHEN ""TrackingStatus"" = 'Processing' THEN 1
                    WHEN ""TrackingStatus"" = 'InWarehouse' THEN 2
                    WHEN ""TrackingStatus"" = 'On The Way' THEN 3
                    WHEN ""TrackingStatus"" = 'Out For Delivery' THEN 4
                    WHEN ""TrackingStatus"" = 'Delivered' THEN 5
                    ELSE 0
                END");

            // Drop old columns
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TrackingStatus",
                table: "Orders");

            // Rename temp columns to original names
            migrationBuilder.RenameColumn(
                name: "RoleTemp",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "Orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TrackingStatusTemp",
                table: "Orders",
                newName: "TrackingStatus");

            // Add default value constraints
            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TrackingStatus",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add temporary text columns
            migrationBuilder.AddColumn<string>(
                name: "RoleTemp",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "Customer");

            migrationBuilder.AddColumn<string>(
                name: "StatusTemp",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<string>(
                name: "TrackingStatusTemp",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "Order Placed");

            // Convert back to text for Users.Role
            migrationBuilder.Sql(@"
                UPDATE ""Users""
                SET ""RoleTemp"" = CASE 
                    WHEN ""Role"" = 0 THEN 'Customer'
                    WHEN ""Role"" = 1 THEN 'Admin'
                    ELSE 'Customer'
                END");

            // Convert back to text for Orders.Status
            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""StatusTemp"" = CASE 
                    WHEN ""Status"" = 0 THEN 'Pending'
                    WHEN ""Status"" = 1 THEN 'Completed'
                    WHEN ""Status"" = 2 THEN 'Cancelled'
                    ELSE 'Pending'
                END");

            // Convert back to text for Orders.TrackingStatus
            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""TrackingStatusTemp"" = CASE 
                    WHEN ""TrackingStatus"" = 0 THEN 'Order Placed'
                    WHEN ""TrackingStatus"" = 1 THEN 'Processing'
                    WHEN ""TrackingStatus"" = 2 THEN 'InWarehouse'
                    WHEN ""TrackingStatus"" = 3 THEN 'On The Way'
                    WHEN ""TrackingStatus"" = 4 THEN 'Out For Delivery'
                    WHEN ""TrackingStatus"" = 5 THEN 'Delivered'
                    ELSE 'Order Placed'
                END");

            // Drop integer columns
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TrackingStatus",
                table: "Orders");

            // Rename temp columns back
            migrationBuilder.RenameColumn(
                name: "RoleTemp",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "Orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TrackingStatusTemp",
                table: "Orders",
                newName: "TrackingStatus");

            // Set default values
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "Customer");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "TrackingStatus",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "Order Placed");
        }
    }
}
