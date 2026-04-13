using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StripePoc.Api.Services.Orchestrations.OneTimePayments;
using StripePoc.Api.Models.PaymentIntents;

namespace StripePoc.Api.Controllers
{
    [ApiController]
    [Route("api/one-time-payments")]
    public class OneTimePaymentsController : RESTFulSense.Controllers.RESTFulController
    {
        private readonly IOneTimePaymentOrchestrationService oneTimePaymentOrchestrationService;

        public OneTimePaymentsController(IOneTimePaymentOrchestrationService oneTimePaymentOrchestrationService) =>
            this.oneTimePaymentOrchestrationService = oneTimePaymentOrchestrationService;

        [HttpPost]
        public async ValueTask<ActionResult<PaymentIntent>> PostOneTimePaymentAsync([FromBody] PaymentIntent paymentIntent)
        {
            PaymentIntent processedPaymentIntent = await this.oneTimePaymentOrchestrationService.ProcessOneTimePaymentAsync(paymentIntent);
            return Created(processedPaymentIntent);
        }

        [HttpPost("quote")]
        public async ValueTask<ActionResult<StripePoc.Api.Models.Events.PaymentLifecycleEvent>> PostOneTimeQuotePaymentAsync([FromBody] PaymentIntent paymentIntent)
        {
            StripePoc.Api.Models.Events.PaymentLifecycleEvent quoteEvent = 
                await this.oneTimePaymentOrchestrationService.ProcessOneTimeQuotePaymentAsync(paymentIntent);
            
            return Created(quoteEvent);
        }

        [HttpPost("{stripePaymentIntentId}/refund")]
        public async ValueTask<ActionResult<string>> PostRefundAsync(string stripePaymentIntentId)
        {
            string refundStatus = await this.oneTimePaymentOrchestrationService.RefundOneTimePaymentAsync(stripePaymentIntentId);
            return Ok(refundStatus);
        }

        [HttpPost("accounts/{accountId}/refund-last")]
        public async ValueTask<ActionResult<string>> PostRefundLastPaymentAsync(System.Guid accountId)
        {
            try
            {
                string refundStatus = await this.oneTimePaymentOrchestrationService.RefundLastPaymentAsync(accountId);
                return Ok(refundStatus);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
