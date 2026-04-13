using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StripePoc.Api.Brokers.Storages;
using StripePoc.Api.Models.Accounts;
using StripePoc.Api.Models.Businesses;
using StripePoc.Api.Services.Orchestrations.Accounts;

namespace StripePoc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : RESTFulSense.Controllers.RESTFulController
    {
        private readonly IAccountOrchestrationService accountOrchestrationService;

        public AccountController(IAccountOrchestrationService accountOrchestrationService) =>
            this.accountOrchestrationService = accountOrchestrationService;

        [HttpGet("businesses")]
        public async ValueTask<ActionResult<IEnumerable<Business>>> GetBusinesses()
        {
            var businesses = await this.accountOrchestrationService.RetrieveAllBusinessesAsync();
            return Ok(businesses.ToList());
        }

        [HttpGet("all")]
        public async ValueTask<ActionResult<IEnumerable<Account>>> GetAllAccounts()
        {
            var accounts = await this.accountOrchestrationService.RetrieveAllAccountsAsync();
            return Ok(accounts.ToList());
        }

        [HttpGet("business/{businessId}")]
        public async ValueTask<ActionResult<IEnumerable<Account>>> GetAccountsByBusiness(Guid businessId)
        {
            var accounts = await this.accountOrchestrationService.RetrieveAccountsByBusinessIdAsync(businessId);
            return Ok(accounts.ToList());
        }
    }
}
