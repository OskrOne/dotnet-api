namespace PortfolioBuffett.Model
{
    public class Order
    {
        public string ContractId { get; set; }
        public int InstrumentId { get; set; }
        public string InstrumentName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Side { get; set; }
        public string Status { get; set; }
    }
}
