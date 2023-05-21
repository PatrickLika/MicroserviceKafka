using Order.Domain.Domain;

namespace Order.Application.Repository
{
    public interface IRepository
    {
        void CreateOrder(OrderEntity orderEntity);
    }
}
