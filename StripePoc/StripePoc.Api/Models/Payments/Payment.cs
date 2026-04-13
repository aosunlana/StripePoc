using System;

namespace StripePoc.Api.Models.Payments
{
    public enum PaymentType
    {
        OneTime,
        Recurring
    }

    public enum PaymentStatus
    {
        Succeeded,
        Failed,
        Refunded
    }

    public class Payment
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string StripeReferenceId { get; set; }
        public PaymentType Type { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTimeOffset PaidAt { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
