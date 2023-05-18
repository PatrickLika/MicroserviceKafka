using Receipt._Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receipt.Application.Repository
{
    public interface IReceiptRepository
    {
        Task<bool> CreateReceipt(ReceiptEntity receiptEntity);
    }
}
