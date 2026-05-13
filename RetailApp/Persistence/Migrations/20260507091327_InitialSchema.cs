using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Smartphones", "Smartphone" },
                    { 2, "Tablets", "Tablet" },
                    { 3, "Laptops", "Laptop" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PhoneNumber" },
                values: new object[] { 1, new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), "johnretail.admin@retail.com", "John", "Retail", "0987654321" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedDate", "Description", "ImageUrl", "Name", "Price", "StockQuantity", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), "{\"OS\":\"iOS 26\",\"CPU\":\"A19\",\"GPU\":\"Apple GPU 5 Cores\",\"RAM\":\"8 GB\",\"Memory\":\"256 GB\",\"Cameras\":\"Main: 48.0 MP + 48.0 MP, Front: 18.0 MP\",\"Screen\":\"6.3 inch OLED, 120Hz, Super Retina XDR (1206 x 2622 Pixels), MAX 3000 nits\",\"Battery Duration\":\"30h\",\"SIM Profile\":\"1 eSIM, 1 nano SIM\"}", "iphone-17-2-400x400.jpg", "iPhone 17 256GB", 913.52m, 10, null },
                    { 2, 3, new DateTime(2026, 5, 6, 0, 0, 0, 0, DateTimeKind.Utc), "{\"OS\":\"Windows 11\",\"CPU\":\"Intel Core i7 Raptor Lake - 14700HX, 20 Cores 28 Threads, 2.10GHz\",\"GPU\":\"NVIDIA GeForce RTX 5050, 8 GB, 115W\",\"RAM\":\"16 GB DDR5 5600MHz, MAX 32 GB\",\"Hard Drive\":\"1 TB SSD M.2 NVMe PCIe Gen 4 x 4 2242, Support 1 slot for SSD M.2 PCIe (MAX 1TB 2280)\", \"Screen\":\"15.1 inch OLED 165Hz, 100% DCI-P3\"}", "lenovo-gaming-legion-5-15irx10-i7-14700hx-83ly00hyvn-1-638909844112150811-750x500.jpg", "Laptop Lenovo Gaming Legion 5 15IRX10 - 83LY00HYVN (i7 14700HX, 16GB, 1TB, RTX 5050 8GB, WQXGA 165Hz, OfficeH24, Win11)", 1636.79m, 5, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
