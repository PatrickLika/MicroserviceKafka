﻿using Storage.Domain.DomainService;

namespace Storage.Domain.Model
{
    public class StorageEntity
    {
        public string Id { get; set; }
        public int Screws { get; set; }
        public int Bolts { get; set; }
        public int Nails { get; set; }
        public int Price { get; set; }
        public string Cvr { get; set; }
        public string State { get; set; }

        public bool IsInStorage { get; set; }

        private readonly IStorageDomainService _domainService;

        public StorageEntity(string id, int screws, int bolts, int nails, int price, string cvr, string state, IStorageDomainService domainService)
        {
            Id = id;
            Screws = screws;
            Bolts = bolts;
            Nails = nails;
            Price = price;
            Cvr = cvr;
            State = state;
            _domainService = domainService;
            IsInStorage = GetIsInStorage();

        }

        bool GetIsInStorage()
        {
            return _domainService.IsInStorage(Screws, Bolts, Nails);
        }



    }
}
