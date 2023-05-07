using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receipt._Domain.Model
{
    public class ReceiptEntity
    {
        public string Id { get; set; }
        public int Screws { get; set; }
        public int Bolts { get; set; }
        public int Nails { get; set; }
        public int Price { get; set; }
        public string Cvr { get; set; }
        public string State { get; set; }
        public string StatePrevious { get; set; }



        public ReceiptEntity(string id, int screws, int bolts, int nails, int price, string cvr, string state, string statePrevious)
        {
            Id = id;
            Screws = screws;
            Bolts = bolts;
            Nails = nails;
            Price = price;
            Cvr = cvr;
            State = state;
            StatePrevious=statePrevious;
        }
    }
}
