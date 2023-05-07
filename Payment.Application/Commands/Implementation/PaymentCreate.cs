using Payment.Application.Repository;
using Payment.Domain.Domain;

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
            var entity = new PaymentEntity(dto.Id, dto.Screws, dto.Bolts, dto.Nails, dto.Price, dto.Cvr, dto.State, dto.StatePrevious);
            _repository.PaymentCreate(entity);
        }
    }
}
