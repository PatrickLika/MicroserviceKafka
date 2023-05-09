using Storage.Application.Commands;
using Storage.Domain.Model;

namespace Storage.Application.Repository
{
    public interface IRepository
    {
        void Produce(StorageEntity entity);

        void Rollback(StorageDbDto dto, StorageDto storageDto);

    }
}
