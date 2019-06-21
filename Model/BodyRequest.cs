namespace PortfolioBuffett.Model
{
    class BodyRequest
    {
        public string ContractId { get; set; }
        public BodyRequest(string contractId)
        {
            ContractId = contractId;
        }

        public override string ToString()
        {
            return "ContractId: " + ContractId;
        }
    }
}
