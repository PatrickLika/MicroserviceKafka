using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Commands
{
    public interface IPaymentCreate
    {
        public void PaymentCreate(PaymentCreateDto dto, string guid);
    }
}
