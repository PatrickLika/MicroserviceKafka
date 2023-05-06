using Customer.Application.Repository;
using Customer.Domain.DomainService;
using Customer.Domain.Model;

namespace Customer.Application.Queries.Implementation
{
    public class ReadCvr : IReadCvr
    {
        private readonly IRepository _repository;
        private readonly ICustomerDomainService _customerDomainService;
        public ReadCvr(IRepository repository, ICustomerDomainService customerDomainService)
        {
            _repository = repository;
            _customerDomainService = customerDomainService;
        }

        void IReadCvr.ReadCvr(ReadCvrDto dto)
        {
            CustomerEntity model = new CustomerEntity(
                dto.Id,
                dto.Screws,
                dto.Bolts,
                dto.Nails,
                dto.Price,
                dto.Cvr,
                _customerDomainService,
                dto.StatePrevious
            );

            _repository.Produce(model);
        }
    }
}



