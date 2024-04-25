using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Acme.ShoppingCart.Data.Migrations {
    public partial class Initial : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Outbox",
                columns: table => new {
                    OutboxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    CorrelationId = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoutingKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                schema: "dbo",
                constraints: table => {
                    table.PrimaryKey("PK_Outbox", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new {
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GivenName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FamilyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserPrincipalName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                schema: "dbo",
                constraints: table => {
                    table.PrimaryKey("PK_Subject", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                schema: "dbo",
                constraints: table => {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Address_Subject_CreateSubjectId",
                        column: x => x.CreateSubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        principalSchema: "dbo");
                    table.ForeignKey(
                        name: "FK_Address_Subject_LastModifiedSubjectId",
                        column: x => x.LastModifiedSubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        principalSchema: "dbo");
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                schema: "dbo",
                constraints: table => {
                    table.PrimaryKey("PK_Customer", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customer_Subject_CreateSubjectId",
                        column: x => x.CreateSubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        principalSchema: "dbo");
                    table.ForeignKey(
                        name: "FK_Customer_Subject_LastModifiedSubjectId",
                        column: x => x.LastModifiedSubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        principalSchema: "dbo");
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    AddressId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                schema: "dbo",
                constraints: table => {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Order_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        principalSchema: "dbo");
                    table.ForeignKey(
                        name: "FK_Order_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "CustomerId",
                        principalSchema: "dbo");
                    table.ForeignKey(
                        name: "FK_Order_Subject_CreateSubjectId",
                        column: x => x.CreateSubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        principalSchema: "dbo");
                    table.ForeignKey(
                        name: "FK_Order_Subject_LastModifiedSubjectId",
                        column: x => x.LastModifiedSubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        principalSchema: "dbo");
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sku = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "money", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedSubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                schema: "dbo",
                constraints: table => {
                    table.PrimaryKey("PK_OrderItem", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        principalSchema: "dbo");
                    table.ForeignKey(
                        name: "FK_OrderItem_Subject_CreateSubjectId",
                        column: x => x.CreateSubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        principalSchema: "dbo");
                    table.ForeignKey(
                        name: "FK_OrderItem_Subject_LastModifiedSubjectId",
                        column: x => x.LastModifiedSubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        principalSchema: "dbo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_CreateSubjectId",
                table: "Address",
                column: "CreateSubjectId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_Address_LastModifiedSubjectId",
                table: "Address",
                column: "LastModifiedSubjectId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CreateSubjectId",
                table: "Customer",
                column: "CreateSubjectId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_LastModifiedSubjectId",
                table: "Customer",
                column: "LastModifiedSubjectId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_Order_AddressId",
                table: "Order",
                column: "AddressId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreateSubjectId",
                table: "Order",
                column: "CreateSubjectId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerId",
                table: "Order",
                column: "CustomerId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_Order_LastModifiedSubjectId",
                table: "Order",
                column: "LastModifiedSubjectId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_CreateSubjectId",
                table: "OrderItem",
                column: "CreateSubjectId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_LastModifiedSubjectId",
                table: "OrderItem",
                column: "LastModifiedSubjectId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId",
                schema: "dbo");

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_MessageId",
                table: "Outbox",
                column: "MessageId",
                schema: "dbo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDate_Status",
                table: "Outbox",
                columns: ["ScheduledDate", "Status"],
                schema: "dbo")
                .Annotation("SqlServer:Include", new[] { "EventType" });

            migrationBuilder.CreateIndex(
                name: "IX_Status_LockId_ScheduleDate",
                table: "Outbox",
                columns: ["Status", "LockId", "ScheduledDate"],
                schema: "dbo");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "OrderItem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Outbox",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Order",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Address",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Customer",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Subject",
                schema: "dbo");
        }
    }
}
