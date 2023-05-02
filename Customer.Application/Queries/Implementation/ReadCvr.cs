using Costumer.Application.Repository;
using Customer.Application.Queries;
using Customer.Domain.DomainService;
using Customer.Domain.Model;
using static System.Reflection.Metadata.BlobBuilder;

namespace Costumer.Application.Queries.Implementation
{
    public class ReadCvr : IReadCvr
    {
        private readonly IRepository _repository;
        private readonly ICustomerDomainService  _customerDomainService;
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
                _customerDomainService
            );

            _repository.Produce(model);
        }
    }
}



