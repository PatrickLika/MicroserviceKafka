using Storage.Application.Repository;

namespace Storage.Application.Commands.Implementation
{
    public class RollbackStorage : IRollBackStorage
    {
        private readonly IRepository _repository;
        public RollbackStorage(IRepository repository)
        {
            _repository = repository;
        }

        void IRollBackStorage.RollBackStorage(StorageDbDto dto, StorageDto storageDto)
        {
            _repository.Rollback(dto, storageDto);
        }
    }
}
