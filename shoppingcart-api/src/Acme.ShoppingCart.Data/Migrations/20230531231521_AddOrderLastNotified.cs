using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ShoppingCart.Data.Migrations {
    public partial class AddOrderLastNotified : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AlterColumn<string>(
                name: "UserPrincipalName",
                schema: "dbo",
                table: "Subject",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Username (upn claim)",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "Subject",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Subject primary key",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GivenName",
                schema: "dbo",
                table: "Subject",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Subject primary key",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FamilyName",
                schema: "dbo",
                table: "Subject",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Subject Surname ()",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Subject",
                type: "datetime2",
                nullable: false,
                comment: "Date and time entity was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                schema: "dbo",
                table: "Subject",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Subject primary key",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                schema: "dbo",
                table: "OrderItem",
                type: "datetime2",
                nullable: false,
                comment: "Date and time entity was last modified",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "OrderItem",
                type: "datetime2",
                nullable: false,
                comment: "Date and time entity was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                schema: "dbo",
                table: "Order",
                type: "datetime2",
                nullable: false,
                comment: "Date and time entity was last modified",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Order",
                type: "datetime2",
                nullable: false,
                comment: "Date and time entity was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastNotified",
                schema: "dbo",
                table: "Order",
                type: "datetime2",
                nullable: true,
                comment: "Date customer was last notified for order");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                schema: "dbo",
                table: "Customer",
                type: "datetime2",
                nullable: false,
                comment: "Date and time entity was last modified",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Customer",
                type: "datetime2",
                nullable: false,
                comment: "Date and time entity was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                schema: "dbo",
                table: "Address",
                type: "datetime2",
                nullable: false,
                comment: "Date and time entity was last modified",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Address",
                type: "datetime2",
                nullable: false,
                comment: "Date and time entity was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "LastNotified",
                schema: "dbo",
                table: "Order");

            migrationBuilder.AlterColumn<string>(
                name: "UserPrincipalName",
                schema: "dbo",
                table: "Subject",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Username (upn claim)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "Subject",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Subject primary key");

            migrationBuilder.AlterColumn<string>(
                name: "GivenName",
                schema: "dbo",
                table: "Subject",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Subject primary key");

            migrationBuilder.AlterColumn<string>(
                name: "FamilyName",
                schema: "dbo",
                table: "Subject",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Subject Surname ()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Subject",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time entity was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "SubjectId",
                schema: "dbo",
                table: "Subject",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Subject primary key");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                schema: "dbo",
                table: "OrderItem",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time entity was last modified");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "OrderItem",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time entity was created");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                schema: "dbo",
                table: "Order",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time entity was last modified");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Order",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time entity was created");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                schema: "dbo",
                table: "Customer",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time entity was last modified");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Customer",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time entity was created");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                schema: "dbo",
                table: "Address",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time entity was last modified");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                schema: "dbo",
                table: "Address",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time entity was created");
        }
    }
}
