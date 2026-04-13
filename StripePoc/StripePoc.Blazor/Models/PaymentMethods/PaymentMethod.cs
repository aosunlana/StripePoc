using System;

namespace StripePoc.Blazor.Models.PaymentMethods
{
    public class PaymentMethod
    {
        public Guid Id { get; set; }
        public Guid PaymentAccountId { get; set; }
        public string StripePaymentMethodId { get; set; }
        public string Brand { get; set; }
        public string Last4 { get; set; }
        public long ExpMonth { get; set; }
        public long ExpYear { get; set; }
        public bool IsDefault { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
