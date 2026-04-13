using System;

namespace StripePoc.Blazor.Models.PaymentAccounts
{
    public class PaymentAccount
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string StripeCustomerId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
