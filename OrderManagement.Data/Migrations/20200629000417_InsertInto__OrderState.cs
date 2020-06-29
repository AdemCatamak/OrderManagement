using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderManagement.Data.Migrations
{
    public partial class InsertInto__OrderState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Insert(migrationBuilder, 0, "Initial");
            Insert(migrationBuilder, 10, "Submitted");
            Insert(migrationBuilder, 20, "PaymentProcessStarted");
            Insert(migrationBuilder, 21, "PaymentCompleted");
            Insert(migrationBuilder, 22, "PaymentFailed");
            Insert(migrationBuilder, 30, "OrderShipped");
            Insert(migrationBuilder, 31, "ShipmentDelivered");
            Insert(migrationBuilder, 32, "ShipmentReturned");
            Insert(migrationBuilder, 40, "RefundStarted");
            Insert(migrationBuilder, 41, "RefundCompleted");
        }

        private void Insert(MigrationBuilder migrationBuilder, int id, string stateName)
        {
            migrationBuilder.InsertData("OrderState", new[] {"Id", "StateName"},
                                        new object[] {id, stateName});
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            Delete(migrationBuilder, 0);
            Delete(migrationBuilder, 10);
            Delete(migrationBuilder, 20);
            Delete(migrationBuilder, 21);
            Delete(migrationBuilder, 22);
            Delete(migrationBuilder, 30);
            Delete(migrationBuilder, 31);
            Delete(migrationBuilder, 32);
            Delete(migrationBuilder, 40);
            Delete(migrationBuilder, 41);
        }

        private void Delete(MigrationBuilder migrationBuilder, int id)
        {
            migrationBuilder.DeleteData("OrderState", "Id", id);
        }
    }
}