using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class urlsupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShortenedUrls_Users_CreatorId",
                table: "ShortenedUrls");

            migrationBuilder.DropForeignKey(
                name: "FK_ShortenedUrls_Users_CreatorUserId",
                table: "ShortenedUrls");

            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_CreatorUserId",
                table: "ShortenedUrls");

            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_ShortUrl",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "ShortUrl",
                table: "ShortenedUrls");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "ShortenedUrls",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalUrl",
                table: "ShortenedUrls",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatorId",
                table: "ShortenedUrls",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "ShortCode",
                table: "ShortenedUrls",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_CreatedAt",
                table: "ShortenedUrls",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_ShortCode",
                table: "ShortenedUrls",
                column: "ShortCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShortenedUrls_Users_CreatorId",
                table: "ShortenedUrls",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShortenedUrls_Users_CreatorId",
                table: "ShortenedUrls");

            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_CreatedAt",
                table: "ShortenedUrls");

            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_ShortCode",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "ShortCode",
                table: "ShortenedUrls");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ShortenedUrls",
                newName: "CreatedDate");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalUrl",
                table: "ShortenedUrls",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatorId",
                table: "ShortenedUrls",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorUserId",
                table: "ShortenedUrls",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ShortUrl",
                table: "ShortenedUrls",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_CreatorUserId",
                table: "ShortenedUrls",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_ShortUrl",
                table: "ShortenedUrls",
                column: "ShortUrl",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShortenedUrls_Users_CreatorId",
                table: "ShortenedUrls",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShortenedUrls_Users_CreatorUserId",
                table: "ShortenedUrls",
                column: "CreatorUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
