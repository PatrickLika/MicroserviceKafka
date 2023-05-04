using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Commands
{
    public class PaymentCreateDto
    {
        public string State { get; set; }
        public int Pris { get; set; }
    }
}
