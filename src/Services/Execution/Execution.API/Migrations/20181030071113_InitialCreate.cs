using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoArbitrage.Services.Execution.API.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "execution");

            migrationBuilder.CreateSequence(
                name: "arbitragetransactionsseq",
                schema: "execution",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "orderseq",
                schema: "execution",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "simplearbitrageseq",
                schema: "execution",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "stoplosssettingseq",
                schema: "execution",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "orderstatus",
                schema: "execution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ordertype",
                schema: "execution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordertype", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "requests",
                schema: "execution",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "simplearbitragestatus",
                schema: "execution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_simplearbitragestatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "stoplosssettings",
                schema: "execution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Exchange_ExchangeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stoplosssettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "execution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    OrderId = table.Column<string>(maxLength: 200, nullable: false),
                    ArbitrageId = table.Column<string>(nullable: false),
                    ExchangeId = table.Column<int>(nullable: false),
                    ExchangeOrderId = table.Column<string>(nullable: true),
                    OrderTypeId = table.Column<int>(nullable: false),
                    OrderStatusId = table.Column<int>(nullable: false),
                    BaseCurrency = table.Column<string>(nullable: false),
                    QuoteCurrency = table.Column<string>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    QuantityTotal = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    QuantityFilled = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    CommisionPaid = table.Column<decimal>(type: "decimal(20,8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orders_orderstatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalSchema: "execution",
                        principalTable: "orderstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_ordertype_OrderTypeId",
                        column: x => x.OrderTypeId,
                        principalSchema: "execution",
                        principalTable: "ordertype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "simplearbitrages",
                schema: "execution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ArbitrageId = table.Column<string>(maxLength: 200, nullable: false),
                    BuyOrder_ArbitrageOrderId = table.Column<string>(nullable: true),
                    BuyOrder_ExchangeId = table.Column<int>(nullable: false),
                    BuyOrder_BaseCurrency = table.Column<string>(nullable: true),
                    BuyOrder_QuoteCurrency = table.Column<string>(nullable: true),
                    BuyOrder_Price = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    BuyOrder_Quantity = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    BuyOrder_SlipPrice = table.Column<decimal>(nullable: false),
                    SellOrder_ArbitrageOrderId = table.Column<string>(nullable: true),
                    SellOrder_ExchangeId = table.Column<int>(nullable: false),
                    SellOrder_BaseCurrency = table.Column<string>(nullable: true),
                    SellOrder_QuoteCurrency = table.Column<string>(nullable: true),
                    SellOrder_Price = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    SellOrder_Quantity = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    SellOrder_SlipPrice = table.Column<decimal>(nullable: false),
                    EstimateProfits = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    ActualProfits = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    ArbitrageData_BaseCurrency = table.Column<string>(nullable: true),
                    ArbitrageData_QuoteCurrency = table.Column<string>(nullable: true),
                    ArbitrageData_OriginalBaseCurrencyQuantity = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    ArbitrageData_OriginalQuoteCurrencyQuantity = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    ArbitrageData_FinalBaseCurrencyQuantity = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    ArbitrageData_FinalQuoteCurrencyQuantity = table.Column<decimal>(type: "decimal(20,8)", nullable: false),
                    SimpleArbitrageStatusId = table.Column<int>(nullable: false),
                    IsSuccess = table.Column<bool>(nullable: false),
                    FailureReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_simplearbitrages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_simplearbitrages_simplearbitragestatus_SimpleArbitrageStatusId",
                        column: x => x.SimpleArbitrageStatusId,
                        principalSchema: "execution",
                        principalTable: "simplearbitragestatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlipPrice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BaseCurrency = table.Column<string>(nullable: true),
                    QuoteCurrency = table.Column<string>(nullable: true),
                    SlipQuantity = table.Column<decimal>(nullable: false),
                    SlipPercents = table.Column<decimal>(nullable: false),
                    StopLossSettingId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlipPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlipPrice_stoplosssettings_StopLossSettingId",
                        column: x => x.StopLossSettingId,
                        principalSchema: "execution",
                        principalTable: "stoplosssettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "arbitragetransactions",
                schema: "execution",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    ArbitrageOrderId = table.Column<string>(maxLength: 200, nullable: false),
                    ExchangeId = table.Column<int>(nullable: false),
                    OriginalOrderTypeId = table.Column<int>(nullable: false),
                    BaseCurrency = table.Column<string>(nullable: false),
                    QuoteCurrency = table.Column<string>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Volume = table.Column<decimal>(nullable: false),
                    CommisionPaid = table.Column<decimal>(nullable: false),
                    SimpleArbitrageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_arbitragetransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_arbitragetransactions_ordertype_OriginalOrderTypeId",
                        column: x => x.OriginalOrderTypeId,
                        principalSchema: "execution",
                        principalTable: "ordertype",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_arbitragetransactions_simplearbitrages_SimpleArbitrageId",
                        column: x => x.SimpleArbitrageId,
                        principalSchema: "execution",
                        principalTable: "simplearbitrages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SlipPrice_StopLossSettingId",
                table: "SlipPrice",
                column: "StopLossSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_arbitragetransactions_ArbitrageOrderId",
                schema: "execution",
                table: "arbitragetransactions",
                column: "ArbitrageOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_arbitragetransactions_OriginalOrderTypeId",
                schema: "execution",
                table: "arbitragetransactions",
                column: "OriginalOrderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_arbitragetransactions_SimpleArbitrageId",
                schema: "execution",
                table: "arbitragetransactions",
                column: "SimpleArbitrageId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderId",
                schema: "execution",
                table: "orders",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderStatusId",
                schema: "execution",
                table: "orders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderTypeId",
                schema: "execution",
                table: "orders",
                column: "OrderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_simplearbitrages_ArbitrageId",
                schema: "execution",
                table: "simplearbitrages",
                column: "ArbitrageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_simplearbitrages_SimpleArbitrageStatusId",
                schema: "execution",
                table: "simplearbitrages",
                column: "SimpleArbitrageStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SlipPrice");

            migrationBuilder.DropTable(
                name: "arbitragetransactions",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "requests",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "stoplosssettings",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "simplearbitrages",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "orderstatus",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "ordertype",
                schema: "execution");

            migrationBuilder.DropTable(
                name: "simplearbitragestatus",
                schema: "execution");

            migrationBuilder.DropSequence(
                name: "arbitragetransactionsseq",
                schema: "execution");

            migrationBuilder.DropSequence(
                name: "orderseq",
                schema: "execution");

            migrationBuilder.DropSequence(
                name: "simplearbitrageseq",
                schema: "execution");

            migrationBuilder.DropSequence(
                name: "stoplosssettingseq",
                schema: "execution");
        }
    }
}
