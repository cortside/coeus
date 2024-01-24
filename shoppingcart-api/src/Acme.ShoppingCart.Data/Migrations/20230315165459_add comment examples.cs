using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ShoppingCart.Data.Migrations {
    public partial class AddCommentExamples : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AlterTable(
                name: "OrderItem",
                schema: "dbo",
                comment: "Items that belong to an Order");

            migrationBuilder.AlterTable(
                name: "Order",
                schema: "dbo",
                comment: "Orders");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "OrderItem",
                type: "money",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money",
                comment: "Per quantity price");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "OrderItem",
                type: "nvarchar(10)",
                maxLength: 10,
                schema: "dbo",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                comment: "Item Sku");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "OrderItem",
                type: "int",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                comment: "Quantity of Sku");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "OrderItem",
                type: "uniqueidentifier",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                comment: "FK to Item in Catalog service");

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "OrderItem",
                type: "int",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                comment: "Primary Key")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Order",
                type: "nvarchar(20)",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                comment: "Order status (created, paid, shipped, cancelled)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderResourceId",
                table: "Order",
                type: "uniqueidentifier",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                comment: "Public unique identifier");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Order",
                type: "int",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                comment: "Primary Key")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Address",
                type: "nvarchar(50)",
                maxLength: 50,
                schema: "dbo",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderResourceId",
                table: "Order",
                column: "OrderResourceId",
                schema: "dbo",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropIndex(
                name: "IX_Order_OrderResourceId",
                table: "Order",
                schema: "dbo");

            migrationBuilder.AlterTable(
                name: "OrderItem",
                schema: "dbo",
                oldComment: "Items that belong to an Order");

            migrationBuilder.AlterTable(
                name: "Order",
                schema: "dbo",
                oldComment: "Orders");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "OrderItem",
                type: "money",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldComment: "Per quantity price");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "OrderItem",
                type: "nvarchar(10)",
                maxLength: 10,
                schema: "dbo",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldComment: "Item Sku");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "OrderItem",
                type: "int",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Quantity of Sku");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "OrderItem",
                type: "uniqueidentifier",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "FK to Item in Catalog service");

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                table: "OrderItem",
                type: "int",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Primary Key")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Order",
                type: "nvarchar(20)",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldComment: "Order status (created, paid, shipped, cancelled)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderResourceId",
                table: "Order",
                type: "uniqueidentifier",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Public unique identifier");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Order",
                type: "int",
                schema: "dbo",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Primary Key")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Address",
                type: "nvarchar(max)",
                schema: "dbo",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
