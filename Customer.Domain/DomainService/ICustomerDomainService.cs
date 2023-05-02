namespace Customer.Domain.DomainService
{
    public interface ICustomerDomainService
    {
        bool CvrIsValid(string cvr);
    }
}
