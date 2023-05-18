using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receipt.Application.Commands
{
    public interface IReceiptCreate
    { 
        Task<bool> ReceiptCreate(ReceiptCreateDto dto, string guid); 
    }
}
