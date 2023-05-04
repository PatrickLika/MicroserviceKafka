using Receipt._Domain.Model;
using Receipt.Application.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receipt.Infrastructure
{
    public class ReceiptRepository : IReceiptRepository
    {
        public ReceiptRepository() 
        {
            
        }


        void IReceiptRepository.CreateReceipt(ReceiptEntity receiptEntity)
        {
            Console.WriteLine($"Kvittering er blevet sendt. Kunde CVR: {receiptEntity.Cvr}");
            throw new NotImplementedException();


        }
    }
}
