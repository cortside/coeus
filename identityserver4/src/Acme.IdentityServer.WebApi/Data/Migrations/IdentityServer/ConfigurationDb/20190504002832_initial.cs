using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Acme.IdentityServer.WebApi.Data.Migrations.IdentityServer.ConfigurationDb {
    public partial class initial : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "LogoutUri",
                table: "Clients");

            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.RenameTable(
                name: "IdentityResources",
                newName: "IdentityResources",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "IdentityClaims",
                newName: "IdentityClaims",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientSecrets",
                newName: "ClientSecrets",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientScopes",
                newName: "ClientScopes",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "Clients",
                newName: "Clients",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientRedirectUris",
                newName: "ClientRedirectUris",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientPostLogoutRedirectUris",
                newName: "ClientPostLogoutRedirectUris",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientIdPRestrictions",
                newName: "ClientIdPRestrictions",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientGrantTypes",
                newName: "ClientGrantTypes",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientCorsOrigins",
                newName: "ClientCorsOrigins",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ClientClaims",
                newName: "ClientClaims",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ApiSecrets",
                newName: "ApiSecrets",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ApiScopes",
                newName: "ApiScopes",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ApiScopeClaims",
                newName: "ApiScopeClaims",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ApiResources",
                newName: "ApiResources",
                newSchema: "auth");

            migrationBuilder.RenameTable(
                name: "ApiClaims",
                newName: "ApiClaims",
                newSchema: "auth");

            migrationBuilder.RenameColumn(
                name: "PrefixClientClaims",
                schema: "auth",
                table: "Clients",
                newName: "NonEditable");

            migrationBuilder.RenameColumn(
                name: "LogoutSessionRequired",
                schema: "auth",
                table: "Clients",
                newName: "FrontChannelLogoutSessionRequired");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "auth",
                table: "IdentityResources",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                schema: "auth",
                table: "IdentityResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                schema: "auth",
                table: "IdentityResources",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "auth",
                table: "ClientSecrets",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "auth",
                table: "ClientSecrets",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "auth",
                table: "ClientSecrets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "LogoUri",
                schema: "auth",
                table: "Clients",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BackChannelLogoutSessionRequired",
                schema: "auth",
                table: "Clients",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BackChannelLogoutUri",
                schema: "auth",
                table: "Clients",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientClaimsPrefix",
                schema: "auth",
                table: "Clients",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ConsentLifetime",
                schema: "auth",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "auth",
                table: "Clients",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "auth",
                table: "Clients",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeviceCodeLifetime",
                schema: "auth",
                table: "Clients",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FrontChannelLogoutUri",
                schema: "auth",
                table: "Clients",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                schema: "auth",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PairWiseSubjectSalt",
                schema: "auth",
                table: "Clients",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                schema: "auth",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCodeType",
                schema: "auth",
                table: "Clients",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserSsoLifetime",
                schema: "auth",
                table: "Clients",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "auth",
                table: "ApiSecrets",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "auth",
                table: "ApiSecrets",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "auth",
                table: "ApiSecrets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "auth",
                table: "ApiResources",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                schema: "auth",
                table: "ApiResources",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                schema: "auth",
                table: "ApiResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                schema: "auth",
                table: "ApiResources",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiProperties",
                schema: "auth",
                columns: table => new {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ApiProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiProperties_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalSchema: "auth",
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientProperties",
                schema: "auth",
                columns: table => new {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ClientId = table.Column<int>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ClientProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientProperties_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "auth",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProperties",
                schema: "auth",
                columns: table => new {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    IdentityResourceId = table.Column<int>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_IdentityProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProperties_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalSchema: "auth",
                        principalTable: "IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiProperties_ApiResourceId",
                schema: "auth",
                table: "ApiProperties",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientProperties_ClientId",
                schema: "auth",
                table: "ClientProperties",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProperties_IdentityResourceId",
                schema: "auth",
                table: "IdentityProperties",
                column: "IdentityResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "ApiProperties",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "ClientProperties",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "IdentityProperties",
                schema: "auth");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "auth",
                table: "IdentityResources");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                schema: "auth",
                table: "IdentityResources");

            migrationBuilder.DropColumn(
                name: "Updated",
                schema: "auth",
                table: "IdentityResources");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "auth",
                table: "ClientSecrets");

            migrationBuilder.DropColumn(
                name: "BackChannelLogoutSessionRequired",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "BackChannelLogoutUri",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ClientClaimsPrefix",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ConsentLifetime",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DeviceCodeLifetime",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "FrontChannelLogoutUri",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LastAccessed",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PairWiseSubjectSalt",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Updated",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserCodeType",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserSsoLifetime",
                schema: "auth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "auth",
                table: "ApiSecrets");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "auth",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "LastAccessed",
                schema: "auth",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                schema: "auth",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "Updated",
                schema: "auth",
                table: "ApiResources");

            migrationBuilder.RenameTable(
                name: "IdentityResources",
                schema: "auth",
                newName: "IdentityResources");

            migrationBuilder.RenameTable(
                name: "IdentityClaims",
                schema: "auth",
                newName: "IdentityClaims");

            migrationBuilder.RenameTable(
                name: "ClientSecrets",
                schema: "auth",
                newName: "ClientSecrets");

            migrationBuilder.RenameTable(
                name: "ClientScopes",
                schema: "auth",
                newName: "ClientScopes");

            migrationBuilder.RenameTable(
                name: "Clients",
                schema: "auth",
                newName: "Clients");

            migrationBuilder.RenameTable(
                name: "ClientRedirectUris",
                schema: "auth",
                newName: "ClientRedirectUris");

            migrationBuilder.RenameTable(
                name: "ClientPostLogoutRedirectUris",
                schema: "auth",
                newName: "ClientPostLogoutRedirectUris");

            migrationBuilder.RenameTable(
                name: "ClientIdPRestrictions",
                schema: "auth",
                newName: "ClientIdPRestrictions");

            migrationBuilder.RenameTable(
                name: "ClientGrantTypes",
                schema: "auth",
                newName: "ClientGrantTypes");

            migrationBuilder.RenameTable(
                name: "ClientCorsOrigins",
                schema: "auth",
                newName: "ClientCorsOrigins");

            migrationBuilder.RenameTable(
                name: "ClientClaims",
                schema: "auth",
                newName: "ClientClaims");

            migrationBuilder.RenameTable(
                name: "ApiSecrets",
                schema: "auth",
                newName: "ApiSecrets");

            migrationBuilder.RenameTable(
                name: "ApiScopes",
                schema: "auth",
                newName: "ApiScopes");

            migrationBuilder.RenameTable(
                name: "ApiScopeClaims",
                schema: "auth",
                newName: "ApiScopeClaims");

            migrationBuilder.RenameTable(
                name: "ApiResources",
                schema: "auth",
                newName: "ApiResources");

            migrationBuilder.RenameTable(
                name: "ApiClaims",
                schema: "auth",
                newName: "ApiClaims");

            migrationBuilder.RenameColumn(
                name: "NonEditable",
                table: "Clients",
                newName: "PrefixClientClaims");

            migrationBuilder.RenameColumn(
                name: "FrontChannelLogoutSessionRequired",
                table: "Clients",
                newName: "LogoutSessionRequired");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientSecrets",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ClientSecrets",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "LogoUri",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoutUri",
                table: "Clients",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ApiSecrets",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ApiSecrets",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250);
        }
    }
}
