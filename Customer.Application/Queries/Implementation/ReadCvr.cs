using Costumer.Application.Repository;

namespace Costumer.Application.Queries.Implementation
{
    public class ReadCvr : IReadCvr
    {
        private readonly IRepository _repository;
        public ReadCvr(IRepository repository)
        {
            _repository = repository;
        }

        void IReadCvr.ReadCvr(string cvr)
        {
            _repository.ReadCvr(cvr);
        }
    }
}