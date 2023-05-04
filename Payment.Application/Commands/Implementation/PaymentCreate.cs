using Payment.Application.Repository;
using Payment.Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Commands.Implementation
{
    public class PaymentCreate : IPaymentCreate
    {
        private readonly IPaymentRepository _repository;

        public PaymentCreate(IPaymentRepository repository)
        {
            _repository = repository;
        }

        void IPaymentCreate.PaymentCreate(PaymentCreateDto dto, string guid)
        {
            var entity = new PaymentEntity(guid, dto.State, dto.Pris);
            _repository.PaymentCreate(entity);
        }
    }
}
