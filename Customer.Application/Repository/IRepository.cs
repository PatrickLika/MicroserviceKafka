using Customer.Domain.Model;

namespace Customer.Application.Repository
{
    public interface IRepository
    {
        void Produce(CustomerEntity entity);
    }
}
