using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using StripePoc.Api.Models.PaymentAccounts;
using StripePoc.Api.Services.Orchestrations.AccountSetups;

namespace StripePoc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentAccountsController : RESTFulController
    {
        private readonly IAccountSetupOrchestrationService accountSetupOrchestrationService;

        public PaymentAccountsController(IAccountSetupOrchestrationService accountSetupOrchestrationService) =>
            this.accountSetupOrchestrationService = accountSetupOrchestrationService;

        [HttpPost("{accountId}")]
        public async ValueTask<ActionResult<PaymentAccount>> PostPaymentAccountAsync(Guid accountId)
        {
            PaymentAccount paymentAccount =
                await this.accountSetupOrchestrationService.ProvidePaymentAccountAsync(accountId);

            return Created(paymentAccount);
        }
    }
}
