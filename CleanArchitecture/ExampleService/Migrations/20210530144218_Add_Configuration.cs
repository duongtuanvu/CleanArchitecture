using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExampleService.Migrations
{
    public partial class Add_Configuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "AspNetUserClaims",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUserClaims",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUserClaims",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUserClaims",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "AspNetUserClaims",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "AspNetUserClaims",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "AspNetUserClaims",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId1",
                table: "AspNetUserClaims",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId1",
                table: "AspNetUserClaims",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId1",
                table: "AspNetUserClaims");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserClaims_UserId1",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "AspNetUserClaims");
        }
    }
}
