using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StripePoc.Api.Services.Orchestrations.Subscriptions;
using StripePoc.Api.Models.Subscriptions;

namespace StripePoc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionsController : RESTFulSense.Controllers.RESTFulController
    {
        private readonly ISubscriptionOrchestrationService subscriptionOrchestrationService;

        public SubscriptionsController(ISubscriptionOrchestrationService subscriptionOrchestrationService) =>
            this.subscriptionOrchestrationService = subscriptionOrchestrationService;

        [HttpPost]
        public async ValueTask<ActionResult<Subscription>> PostSubscriptionAsync([FromBody] Subscription subscription)
        {
            Subscription addedSubscription = await this.subscriptionOrchestrationService.AddSubscriptionAsync(subscription);
            return Created(addedSubscription);
        }

        [HttpPost("quote")]
        public async ValueTask<ActionResult<StripePoc.Api.Models.Events.PaymentLifecycleEvent>> PostSubscriptionQuoteAsync([FromBody] Subscription subscription)
        {
            StripePoc.Api.Models.Events.PaymentLifecycleEvent quoteEvent = 
                await this.subscriptionOrchestrationService.ProcessSubscriptionQuoteAsync(subscription);
            
            return Created(quoteEvent);
        }

        [HttpGet("accounts/{accountId}")]
        public async ValueTask<ActionResult<IQueryable<Subscription>>> GetSubscriptionsByAccountIdAsync(Guid accountId)
        {
            IQueryable<Subscription> subscriptions = await this.subscriptionOrchestrationService.RetrieveAllSubscriptionsByAccountIdAsync(accountId);
            return Ok(subscriptions);
        }

        [HttpGet("{subscriptionId}")]
        public async ValueTask<ActionResult<Subscription>> GetSubscriptionByIdAsync(Guid subscriptionId)
        {
            Subscription subscription = await this.subscriptionOrchestrationService.RetrieveSubscriptionByIdAsync(subscriptionId);
            return Ok(subscription);
        }

        [HttpDelete("{subscriptionId}")]
        public async ValueTask<ActionResult<Subscription>> DeleteSubscriptionAsync(Guid subscriptionId)
        {
            Subscription cancelledSubscription = await this.subscriptionOrchestrationService.CancelSubscriptionAsync(subscriptionId);
            return Ok(cancelledSubscription);
        }
    }
}
