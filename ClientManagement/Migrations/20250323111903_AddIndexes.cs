using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Customers_ContactId",
                table: "Customers",
                newName: "IX_Customer_ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_Phone",
                table: "Contacts",
                column: "Phone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contact_Phone",
                table: "Contacts");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_ContactId",
                table: "Customers",
                newName: "IX_Customers_ContactId");
        }
    }
}
