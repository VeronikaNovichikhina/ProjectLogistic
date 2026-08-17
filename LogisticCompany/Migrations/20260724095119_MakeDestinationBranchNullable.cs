using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticCompany.Migrations
{
    /// <inheritdoc />
    public partial class MakeDestinationBranchNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "destination_branch_ID",
                table: "orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_orders_destination_branch_ID",
                table: "orders",
                column: "destination_branch_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_destination_branch",
                table: "orders",
                column: "destination_branch_ID",
                principalTable: "branches",
                principalColumn: "branches_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_destination_branch",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_destination_branch_ID",
                table: "orders");

            migrationBuilder.AlterColumn<int>(
                name: "destination_branch_ID",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
