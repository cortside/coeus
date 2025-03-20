using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ShoppingCart.Data.Migrations
{
    /// <inheritdoc />
    public partial class OutboxRemainingAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "LockId",
                schema: "dbo",
                table: "Outbox",
                type: "uniqueidentifier",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingAttempts",
                schema: "dbo",
                table: "Outbox",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingAttempts",
                schema: "dbo",
                table: "Outbox");

            migrationBuilder.AlterColumn<string>(
                name: "LockId",
                schema: "dbo",
                table: "Outbox",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 36,
                oldNullable: true);
        }
    }
}
