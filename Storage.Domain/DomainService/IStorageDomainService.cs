using Storage.Domain.Model;

namespace Storage.Domain.DomainService
{
    public interface IStorageDomainService
    {
        Task<StorageEntity> GetStorageInformation();
    }
}
