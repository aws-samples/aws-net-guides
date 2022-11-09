using Microsoft.AspNetCore.Mvc;
using AspNetCoreWebApiRds.Data;

namespace AspNetCoreWebApiRds.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly BillingContext _billingContext;

        public InvoicesController (BillingContext billingContext)
        {
            _billingContext = billingContext; 
        }

        [HttpGet]
        public ActionResult Get(int take = 10, int skip = 0)
        {
            return Ok(_billingContext.Invoices.OrderBy(i => i.InvoiceId).Skip(skip).Take(take));
        }
    }
}
