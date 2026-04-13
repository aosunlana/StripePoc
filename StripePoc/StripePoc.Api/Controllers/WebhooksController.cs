using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StripePoc.Api.Services.Orchestrations.Webhooks;

namespace StripePoc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhooksController : RESTFulSense.Controllers.RESTFulController
    {
        private readonly IWebhookOrchestrationService webhookOrchestrationService;

        public WebhooksController(IWebhookOrchestrationService webhookOrchestrationService) =>
            this.webhookOrchestrationService = webhookOrchestrationService;

        [HttpPost("stripe")]
        public async ValueTask<ActionResult> PostStripeWebhookAsync()
        {
            string jsonPayload;

            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                jsonPayload = await reader.ReadToEndAsync();
            }

            string stripeSignatureHeader = Request.Headers["Stripe-Signature"];

            await this.webhookOrchestrationService.HandleStripeEventAsync(jsonPayload, stripeSignatureHeader);

            return Ok();
        }
    }
}
