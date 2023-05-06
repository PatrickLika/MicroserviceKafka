using Storage.Application.Repository;
using Storage.Domain.DomainService;
using Storage.Domain.Model;

namespace Storage.Application.Commands.Implementation
{
    public class StorageCommand : IStorageCommand
    {
        private readonly IRepository _repository;
        private readonly IStorageDomainService _storageDomainService;
        public StorageCommand(IRepository repository, IStorageDomainService storageDomainService)
        {
            _repository = repository;
            _storageDomainService = storageDomainService;
        }

        void IStorageCommand.CheckStorage(StorageDto dto)
        {

            var entity = new StorageEntity
            (
                dto.Id,
                dto.Screws,
                dto.Bolts,
                dto.Nails,
                dto.Price,
                dto.Cvr,
                dto.State,
                _storageDomainService
            );

        
            _repository.Produce(entity);
        }

    }
}
