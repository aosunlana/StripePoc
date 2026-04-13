using System;
using System.Linq;
using System.Threading.Tasks;
using StripePoc.Api.Models.Businesses;

namespace StripePoc.Api.Services.Foundations.Businesses
{
    public interface IBusinessService
    {
        ValueTask<Business> AddBusinessAsync(Business business);
        ValueTask<Business> RetrieveBusinessByIdAsync(Guid businessId);
        ValueTask<IQueryable<Business>> RetrieveAllBusinessesAsync();
    }
}
