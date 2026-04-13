using Microsoft.EntityFrameworkCore;
using StripePoc.Api.Models.PaymentAccounts;
using StripePoc.Api.Models.PaymentMethods;
using StripePoc.Api.Models.Subscriptions;
using StripePoc.Api.Models.PaymentIntents;
using StripePoc.Api.Models.Payments;
using StripePoc.Api.Models.Businesses;
using StripePoc.Api.Models.Accounts;

namespace StripePoc.Api.Brokers.Storages
{
    public partial class StorageBroker : DbContext, IStorageBroker
    {
        public StorageBroker(DbContextOptions<StorageBroker> options)
            : base(options)
        {
            this.Database.Migrate();
        }

        public DbSet<PaymentAccount> PaymentAccounts { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<PaymentIntent> PaymentIntents { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure composite keys or indexes if needed
            modelBuilder.Entity<PaymentAccount>()
                .HasIndex(b => b.AccountId)
                .IsUnique();

            modelBuilder.Entity<Subscription>()
                .HasIndex(s => s.StripeSubscriptionId)
                .IsUnique();

            modelBuilder.Entity<PaymentIntent>()
                .HasIndex(p => p.StripePaymentIntentId)
                .IsUnique();

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.StripeReferenceId)
                .IsUnique();
        }
    }
}
