using System;

namespace StripePoc.Api.Models.PaymentIntents
{
    public class PaymentIntent
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string StripePaymentIntentId { get; set; }
        public string StripeInvoiceId { get; set; }
        public string StripeCustomerId { get; set; }
        public string StripePaymentMethodId { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public PaymentIntentStatus Status { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
