using Microsoft.AspNetCore.Mvc;
using StripePoc.Api.Models.PaymentMethods;
using StripePoc.Api.Services.Orchestrations.AccountSetups;

namespace StripePoc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodsController : RESTFulSense.Controllers.RESTFulController
    {
        private readonly IAccountSetupOrchestrationService accountSetupOrchestrationService;

        public PaymentMethodsController(IAccountSetupOrchestrationService accountSetupOrchestrationService) =>
            this.accountSetupOrchestrationService = accountSetupOrchestrationService;

        [HttpPost("accounts/{accountId}/setup/initiate")]
        public async ValueTask<ActionResult<string>> PostInitiateCardSetupAsync(Guid accountId)
        {
            string clientSecret =
                await this.accountSetupOrchestrationService.InitiatePaymentMethodSetupAsync(
                    accountId);

            return Ok(clientSecret);
        }

        [HttpPost("accounts/{accountId}")]
        public async ValueTask<ActionResult<PaymentMethod>> PostPaymentMethodAsync(
            Guid accountId,
            [FromBody] PaymentMethod paymentMethod)
        {
            PaymentMethod savedPaymentMethod =
                await this.accountSetupOrchestrationService.FinalisePaymentMethodSetupAsync(
                accountId, paymentMethod.StripePaymentMethodId);

            return Created(savedPaymentMethod);
        }

        [HttpGet("accounts/{accountId}")]
        public async ValueTask<ActionResult<IQueryable<PaymentMethod>>> GetPaymentMethodsByAccountIdAsync(
            Guid accountId)
        {
            IQueryable<PaymentMethod> paymentMethods =
                await this.accountSetupOrchestrationService
                    .RetrieveAllPaymentMethodsByAccountIdAsync(accountId);

            return Ok(paymentMethods);
        }

        [HttpPatch("accounts/{accountId}/{paymentMethodId}/default")]
        public async ValueTask<ActionResult<PaymentMethod>> PatchDefaultPaymentMethodAsync(Guid accountId, Guid paymentMethodId)
        {
            PaymentMethod updatedPaymentMethod = await this.accountSetupOrchestrationService.SetDefaultPaymentMethodAsync(accountId, paymentMethodId);
            return Ok(updatedPaymentMethod);
        }

        [HttpDelete("accounts/{accountId}/{paymentMethodId}")]
        public async ValueTask<ActionResult<PaymentMethod>> DeletePaymentMethodAsync(Guid accountId, Guid paymentMethodId)
        {
            PaymentMethod removedPaymentMethod = await this.accountSetupOrchestrationService.RemovePaymentMethodAsync(accountId, paymentMethodId);
            return Ok(removedPaymentMethod);
        }
    }
}
