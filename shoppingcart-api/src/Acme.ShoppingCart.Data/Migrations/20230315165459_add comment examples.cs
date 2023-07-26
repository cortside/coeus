using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ShoppingCart.Data.Migrations {
    public partial class addcommentexamples : Migration {
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
                schema: "dbo",
                table: "OrderItem",
                type: "money",
                nullable: false,
                comment: "Per quantity price",
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                schema: "dbo",
                table: "OrderItem",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                comment: "Item Sku",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                schema: "dbo",
                table: "OrderItem",
                type: "int",
                nullable: false,
                comment: "Quantity of Sku",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                schema: "dbo",
                table: "OrderItem",
                type: "uniqueidentifier",
                nullable: false,
                comment: "FK to Item in Catalog service",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                schema: "dbo",
                table: "OrderItem",
                type: "int",
                nullable: false,
                comment: "Primary Key",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "Order",
                type: "nvarchar(20)",
                nullable: false,
                comment: "Order status (created, paid, shipped, cancelled)",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderResourceId",
                schema: "dbo",
                table: "Order",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Public unique identifier",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                schema: "dbo",
                table: "Order",
                type: "int",
                nullable: false,
                comment: "Primary Key",
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                schema: "dbo",
                table: "Address",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderResourceId",
                schema: "dbo",
                table: "Order",
                column: "OrderResourceId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropIndex(
                name: "IX_Order_OrderResourceId",
                schema: "dbo",
                table: "Order");

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
                schema: "dbo",
                table: "OrderItem",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldComment: "Per quantity price");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                schema: "dbo",
                table: "OrderItem",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldComment: "Item Sku");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                schema: "dbo",
                table: "OrderItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Quantity of Sku");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                schema: "dbo",
                table: "OrderItem",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "FK to Item in Catalog service");

            migrationBuilder.AlterColumn<int>(
                name: "OrderItemId",
                schema: "dbo",
                table: "OrderItem",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Primary Key")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "dbo",
                table: "Order",
                type: "nvarchar(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldComment: "Order status (created, paid, shipped, cancelled)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderResourceId",
                schema: "dbo",
                table: "Order",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Public unique identifier");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                schema: "dbo",
                table: "Order",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Primary Key")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                schema: "dbo",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
