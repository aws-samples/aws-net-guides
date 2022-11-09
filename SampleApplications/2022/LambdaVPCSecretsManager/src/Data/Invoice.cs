namespace AspNetCoreWebApiRds.Data
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public string? Name { get; set; }
        public InvoiceCategory InvoiceCategory { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
    }
}