using Storage.Domain.DomainService;

namespace Storage.Domain.Model
{
    public class StorageEntity
    {
        public string Id { get; set; }
        public int Screws { get; set; }
        public int Bolts { get; set; }
        public int Nails { get; set; }
        public string State { get; set; }
        private readonly IStorageDomainService _domainService;

        public StorageEntity(string id, int screws, int bolts, int nails, string state, IStorageDomainService domainService)
        {
            Id = id;
            Screws = screws;
            Bolts = bolts;
            Nails = nails;
            State = state;
            _domainService = domainService;


        }
        private async Task IsInStorageAsync()
        {
            if (await _domainService.IsInStorage(Screws, Bolts, Nails))
            {
                State = "StorageApproved";
            }
            else
            {
                State = "StorageDenied";
            }
        }



    }
}
