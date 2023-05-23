namespace Order.Application.Commands
{
    public interface IOrderCreate
    {
        void Create(OrderDto dto);
    }
}
