using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ShoppingCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueOrderItemIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderItem_OrderId",
                schema: "dbo",
                table: "OrderItem");

            migrationBuilder.CreateIndex(
                name: "IX_Status_LastModifiedDate",
                schema: "dbo",
                table: "Outbox",
                columns: new[] { "Status", "LastModifiedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId_ItemId",
                schema: "dbo",
                table: "OrderItem",
                columns: new[] { "OrderId", "ItemId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Status_LastModifiedDate",
                schema: "dbo",
                table: "Outbox");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_OrderId_ItemId",
                schema: "dbo",
                table: "OrderItem");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                schema: "dbo",
                table: "OrderItem",
                column: "OrderId");
        }
    }
}
