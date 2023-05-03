using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Domain.Domain
{
    public class PaymentEntity
    {
        public string Guid { get; set; }
        public string State { get; set; }
        public int Pris { get; set; }


        public PaymentEntity(string guid, string state, int pris)
        {
            Guid = guid;
            State = state;
            Pris = pris;
            PaymentOk();
        }


        private bool PaymentMax()
        {
            if (Pris > 5000) { return false; }
            return true;
        }

        private void PaymentOk()
        {
            if (PaymentMax()) State = "PaymentApproved";
            
            else State = "PaymentDenied";
        }

    }
}
