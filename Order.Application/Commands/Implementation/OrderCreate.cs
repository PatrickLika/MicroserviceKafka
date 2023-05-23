using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order.Application.Repository;
using Order.Domain.Domain;

namespace Order.Application.Commands.Implementation
{
    public class OrderCreate : IOrderCreate
    {
        private readonly IRepository _repository;
        public OrderCreate(IRepository repository)
        {
            _repository = repository;
        }

        void IOrderCreate.Create(OrderDto dto)
        {
            var order = new OrderEntity(dto.Screws, dto.Bolts, dto.Nails, dto.Price, dto.Cvr);
            _repository.CreateOrder(order);
        }
    }
}
