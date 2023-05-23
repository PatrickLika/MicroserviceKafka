using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Order.Domain.Domain
{
    public class OrderEntity
    {
        public string Id { get; set; }
        public int Screws { get; set; }
        public int Bolts { get; set; }
        public int Nails { get; set; }
        public int Price { get; set; }
        public string Cvr { get; set; }
        public string State { get; set; }
        


        public OrderEntity(int screws, int bolts, int nails, int price, string cvr)
        {
            Id = Guid.NewGuid().ToString();
            Screws = screws;
            Bolts = bolts;
            Nails = nails;
            Price = price;
            Cvr = cvr;
            State = States.OrderPending;
            if (!ValidOrder()) throw new InvalidOperationException("Fejl");
        }

        public bool ValidOrder()
        {
            if (Screws > 0 || Bolts > 0 || Nails > 0)
            {
                return true;
            }

            return false;
        }

    }

}
