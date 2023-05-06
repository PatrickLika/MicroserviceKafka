using Storage.Application.Repository;

namespace Storage.Application.Commands
{
    public class RollbackStorage : IRollBackStorage
    {
        private readonly IRepository _repository;
        public RollbackStorage(IRepository repository)
        {
            _repository = repository;
        }

        void IRollBackStorage.RollBackStorage(StorageDbDto dto)
        {
            _repository.Rollback(dto);
        }
    }
}
