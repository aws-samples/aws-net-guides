using System.Collections.Generic;
using System.Linq;
using AutoFixture;

namespace AspNetCoreWebApiRds.Data
{
    public static class Seeder
    {
        
        // This is purely for a demo, don't anything like this in a real application!
        public static void Seed(this BillingContext billingContext)
        {
            if (!billingContext.Invoices.Any())
            {
                Fixture fixture = new Fixture();
                fixture.Customize<Invoice>(invoice => invoice.Without(i => i.InvoiceId));
                List<Invoice> invoices = fixture.CreateMany<Invoice>(200).ToList();
                billingContext.AddRange(invoices);
                billingContext.SaveChanges();
           }
        }
    }
}