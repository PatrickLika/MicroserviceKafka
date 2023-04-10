using Costumer.Application.Repository;
using Customer.Application.Queries;

namespace Costumer.Application.Queries.Implementation
{
    public class ReadCvr : IReadCvr
    {
        private readonly IRepository _repository;
        public ReadCvr(IRepository repository)
        {
            _repository = repository;
        }

        void IReadCvr.ReadCvr(ReadCvrDto dto)
        {
            _repository.ReadCvr(dto);
        }
    }
}