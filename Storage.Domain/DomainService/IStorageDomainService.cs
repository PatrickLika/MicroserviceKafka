namespace Storage.Domain.DomainService
{
    public interface IStorageDomainService
    {
        bool IsInStorage(int screws, int bolts, int nails);
    }
}
