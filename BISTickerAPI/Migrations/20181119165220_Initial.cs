using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BISTickerAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coins",
                columns: table => new
                {
                    Symbol = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coins", x => x.Symbol);
                });

            migrationBuilder.CreateTable(
                name: "Exchange",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exchange", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TickerEntries",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PairCoin1Symbol = table.Column<string>(nullable: true),
                    PairCoin2Symbol = table.Column<string>(nullable: true),
                    ExchangeId = table.Column<int>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    AskPrice = table.Column<double>(nullable: false),
                    BidPrice = table.Column<double>(nullable: false),
                    LastPrice = table.Column<double>(nullable: false),
                    Low = table.Column<double>(nullable: true),
                    High = table.Column<double>(nullable: true),
                    Volume = table.Column<double>(nullable: true),
                    SellVolume = table.Column<double>(nullable: true),
                    BuyVolume = table.Column<double>(nullable: true),
                    Change = table.Column<double>(nullable: true),
                    Open = table.Column<double>(nullable: true),
                    Close = table.Column<double>(nullable: true),
                    BaseVolume = table.Column<double>(nullable: true),
                    BuyBaseVolume = table.Column<double>(nullable: true),
                    SellBaseVolume = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TickerEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TickerEntries_Exchange_ExchangeId",
                        column: x => x.ExchangeId,
                        principalTable: "Exchange",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TickerEntries_Coins_PairCoin1Symbol",
                        column: x => x.PairCoin1Symbol,
                        principalTable: "Coins",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TickerEntries_Coins_PairCoin2Symbol",
                        column: x => x.PairCoin2Symbol,
                        principalTable: "Coins",
                        principalColumn: "Symbol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TickerEntries_ExchangeId",
                table: "TickerEntries",
                column: "ExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_TickerEntries_PairCoin1Symbol",
                table: "TickerEntries",
                column: "PairCoin1Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_TickerEntries_PairCoin2Symbol",
                table: "TickerEntries",
                column: "PairCoin2Symbol");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TickerEntries");

            migrationBuilder.DropTable(
                name: "Exchange");

            migrationBuilder.DropTable(
                name: "Coins");
        }
    }
}
