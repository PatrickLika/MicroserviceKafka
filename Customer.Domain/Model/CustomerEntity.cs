using Customer.Domain.DomainService;

namespace Customer.Domain.Model
{
    public class CustomerEntity
    {
        public string Id { get; set; }
        public int Screws { get; set; }
        public int Bolts { get; set; }
        public int Nails { get; set; }
        public int Price { get; set; }
        public string Cvr { get; set; }
        public bool IsValid { get; set; }
        public string State { get; set; }
        public string StatePrevious { get; set; }

        private readonly ICustomerDomainService _domainService;

        public CustomerEntity(string id, int screws, int bolts, int nails, int price, string cvr, ICustomerDomainService domainService, string statePrevious)
        {
            Id = id;
            Screws = screws;
            Bolts = bolts;
            Nails = nails;
            Price = price;
            Cvr = cvr;
            _domainService = domainService;
            IsValid = ValidID();
            StatePrevious=statePrevious;
        }
        
        private bool ValidID()
        {
            return _domainService.CvrIsValid(Cvr);
        }

    }
}
