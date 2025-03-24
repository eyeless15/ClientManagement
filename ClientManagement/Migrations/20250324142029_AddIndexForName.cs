using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customer_Name",
                table: "Customers",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customer_Name",
                table: "Customers");
        }
    }
}
