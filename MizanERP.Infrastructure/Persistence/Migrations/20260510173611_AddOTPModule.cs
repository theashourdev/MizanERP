using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MizanERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOTPModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailOtpCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailOtpExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailOtpCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailOtpExpiry",
                table: "AspNetUsers");
        }
    }
}
