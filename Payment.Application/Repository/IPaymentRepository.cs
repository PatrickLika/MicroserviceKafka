using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Payment.Domain.Domain;

namespace Payment.Application.Repository
{
    public interface IPaymentRepository
    {
        void PaymentCreate(PaymentEntity paymentEntity);
    }
}
