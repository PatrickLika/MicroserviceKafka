using Customer.Application.Queries;
using Customer.Domain.Model;

namespace Costumer.Application.Repository
{
    public interface IRepository
    {
        void Produce(CustomerEntity entity);
    }
}
