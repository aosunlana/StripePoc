using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StripePoc.Api.Services.Foundations.Payments;
using StripePoc.Api.Models.Payments;

namespace StripePoc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : RESTFulSense.Controllers.RESTFulController
    {
        private readonly IPaymentService paymentService;

        public PaymentsController(IPaymentService paymentService) =>
            this.paymentService = paymentService;

        [HttpGet("accounts/{accountId}")]
        public async ValueTask<ActionResult<IQueryable<Payment>>> GetPaymentsByAccountIdAsync(Guid accountId)
        {
            IQueryable<Payment> payments = await this.paymentService.RetrieveAllPaymentsByAccountIdAsync(accountId);
            return Ok(payments);
        }
    }
}
