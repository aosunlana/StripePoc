using System;

namespace StripePoc.Blazor.Models.Subscriptions
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string StripeSubscriptionId { get; set; }
        public string ServiceName { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public int IntervalCount { get; set; }
        public SubscriptionStatus Status { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
        public DateTimeOffset CurrentPeriodStart { get; set; }
        public DateTimeOffset CurrentPeriodEnd { get; set; }
        public string StripeClientSecret { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
