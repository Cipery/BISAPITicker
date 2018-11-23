using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BISTickerAPI.Migrations
{
    public partial class ExchangesDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TickerEntries_Exchange_ExchangeId",
                table: "TickerEntries");

            migrationBuilder.DropIndex(
                name: "IX_TickerEntries_ExchangeId",
                table: "TickerEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exchange",
                table: "Exchange");

            migrationBuilder.DropColumn(
                name: "ExchangeId",
                table: "TickerEntries");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Exchange");

            migrationBuilder.RenameTable(
                name: "Exchange",
                newName: "Exchanges");

            migrationBuilder.AddColumn<string>(
                name: "ExchangeName",
                table: "TickerEntries",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Exchanges",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exchanges",
                table: "Exchanges",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TickerEntries_ExchangeName",
                table: "TickerEntries",
                column: "ExchangeName");

            migrationBuilder.AddForeignKey(
                name: "FK_TickerEntries_Exchanges_ExchangeName",
                table: "TickerEntries",
                column: "ExchangeName",
                principalTable: "Exchanges",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TickerEntries_Exchanges_ExchangeName",
                table: "TickerEntries");

            migrationBuilder.DropIndex(
                name: "IX_TickerEntries_ExchangeName",
                table: "TickerEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exchanges",
                table: "Exchanges");

            migrationBuilder.DropColumn(
                name: "ExchangeName",
                table: "TickerEntries");

            migrationBuilder.RenameTable(
                name: "Exchanges",
                newName: "Exchange");

            migrationBuilder.AddColumn<int>(
                name: "ExchangeId",
                table: "TickerEntries",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Exchange",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Exchange",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exchange",
                table: "Exchange",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TickerEntries_ExchangeId",
                table: "TickerEntries",
                column: "ExchangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TickerEntries_Exchange_ExchangeId",
                table: "TickerEntries",
                column: "ExchangeId",
                principalTable: "Exchange",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
