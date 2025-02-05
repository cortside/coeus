using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ShoppingCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class PublishCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Subject_CreateSubjectId",
                schema: "dbo",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Subject_CreateSubjectId",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Subject_CreateSubjectId",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Subject_CreateSubjectId",
                schema: "dbo",
                table: "OrderItem");

            migrationBuilder.RenameColumn(
                name: "CreateSubjectId",
                schema: "dbo",
                table: "OrderItem",
                newName: "CreatedSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_CreateSubjectId",
                schema: "dbo",
                table: "OrderItem",
                newName: "IX_OrderItem_CreatedSubjectId");

            migrationBuilder.RenameColumn(
                name: "CreateSubjectId",
                schema: "dbo",
                table: "Order",
                newName: "CreatedSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CreateSubjectId",
                schema: "dbo",
                table: "Order",
                newName: "IX_Order_CreatedSubjectId");

            migrationBuilder.RenameColumn(
                name: "CreateSubjectId",
                schema: "dbo",
                table: "Customer",
                newName: "CreatedSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_CreateSubjectId",
                schema: "dbo",
                table: "Customer",
                newName: "IX_Customer_CreatedSubjectId");

            migrationBuilder.RenameColumn(
                name: "CreateSubjectId",
                schema: "dbo",
                table: "Address",
                newName: "CreatedSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Address_CreateSubjectId",
                schema: "dbo",
                table: "Address",
                newName: "IX_Address_CreatedSubjectId");

            migrationBuilder.AddColumn<int>(
                name: "PublishCount",
                schema: "dbo",
                table: "Outbox",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Subject_CreatedSubjectId",
                schema: "dbo",
                table: "Address",
                column: "CreatedSubjectId",
                principalSchema: "dbo",
                principalTable: "Subject",
                principalColumn: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Subject_CreatedSubjectId",
                schema: "dbo",
                table: "Customer",
                column: "CreatedSubjectId",
                principalSchema: "dbo",
                principalTable: "Subject",
                principalColumn: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Subject_CreatedSubjectId",
                schema: "dbo",
                table: "Order",
                column: "CreatedSubjectId",
                principalSchema: "dbo",
                principalTable: "Subject",
                principalColumn: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Subject_CreatedSubjectId",
                schema: "dbo",
                table: "OrderItem",
                column: "CreatedSubjectId",
                principalSchema: "dbo",
                principalTable: "Subject",
                principalColumn: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Subject_CreatedSubjectId",
                schema: "dbo",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Subject_CreatedSubjectId",
                schema: "dbo",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Subject_CreatedSubjectId",
                schema: "dbo",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Subject_CreatedSubjectId",
                schema: "dbo",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "PublishCount",
                schema: "dbo",
                table: "Outbox");

            migrationBuilder.RenameColumn(
                name: "CreatedSubjectId",
                schema: "dbo",
                table: "OrderItem",
                newName: "CreateSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_CreatedSubjectId",
                schema: "dbo",
                table: "OrderItem",
                newName: "IX_OrderItem_CreateSubjectId");

            migrationBuilder.RenameColumn(
                name: "CreatedSubjectId",
                schema: "dbo",
                table: "Order",
                newName: "CreateSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CreatedSubjectId",
                schema: "dbo",
                table: "Order",
                newName: "IX_Order_CreateSubjectId");

            migrationBuilder.RenameColumn(
                name: "CreatedSubjectId",
                schema: "dbo",
                table: "Customer",
                newName: "CreateSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_CreatedSubjectId",
                schema: "dbo",
                table: "Customer",
                newName: "IX_Customer_CreateSubjectId");

            migrationBuilder.RenameColumn(
                name: "CreatedSubjectId",
                schema: "dbo",
                table: "Address",
                newName: "CreateSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Address_CreatedSubjectId",
                schema: "dbo",
                table: "Address",
                newName: "IX_Address_CreateSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Subject_CreateSubjectId",
                schema: "dbo",
                table: "Address",
                column: "CreateSubjectId",
                principalSchema: "dbo",
                principalTable: "Subject",
                principalColumn: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Subject_CreateSubjectId",
                schema: "dbo",
                table: "Customer",
                column: "CreateSubjectId",
                principalSchema: "dbo",
                principalTable: "Subject",
                principalColumn: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Subject_CreateSubjectId",
                schema: "dbo",
                table: "Order",
                column: "CreateSubjectId",
                principalSchema: "dbo",
                principalTable: "Subject",
                principalColumn: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Subject_CreateSubjectId",
                schema: "dbo",
                table: "OrderItem",
                column: "CreateSubjectId",
                principalSchema: "dbo",
                principalTable: "Subject",
                principalColumn: "SubjectId");
        }
    }
}
