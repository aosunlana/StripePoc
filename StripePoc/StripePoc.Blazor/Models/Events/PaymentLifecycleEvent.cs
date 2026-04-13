using System;

namespace StripePoc.Blazor.Models.Events
{
    public class PaymentLifecycleEvent
    {
        public bool IsValid { get; set; }
        public string ExternalReferenceId { get; set; }
        public string EventType { get; set; }
        public string SubscriptionId { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string CheckoutUrl { get; set; }
        public DateTimeOffset OccurredAt { get; set; }
    }
}
