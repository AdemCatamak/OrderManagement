using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderManagement.Data.Migrations
{
    public partial class CreateTable__OrderState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                                         name: "OrderState",
                                         columns: table => new
                                                           {
                                                               Id = table.Column<int>(nullable: false),
                                                               StateName = table.Column<string>(nullable: false)
                                                           },
                                         constraints: table => { table.PrimaryKey("PK_OrderState", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                                       name: "OrderState");
        }
    }
}