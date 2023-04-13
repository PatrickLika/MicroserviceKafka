using Storage.Domain.Model;

namespace Storage.Domain.DomainService
{
    public interface IStorageDomainService
    {
        Task<bool> IsInStorage(int screws, int bolts, int nails);
    }
}
