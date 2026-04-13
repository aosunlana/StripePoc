using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StripePoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeInvoiceIdToPaymentIntent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeInvoiceId",
                table: "PaymentIntents",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeInvoiceId",
                table: "PaymentIntents");
        }
    }
}
