using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Receipt._Domain.Model;
using Receipt.Application.Repository;

namespace Receipt.Application.Commands.Implementation
{
    public class ReceiptCreate: IReceiptCreate
    {
        private readonly IReceiptRepository _repository;

        public ReceiptCreate(IReceiptRepository repository)
        {
            _repository = repository;
        }

        void IReceiptCreate.ReceiptCreate(ReceiptCreateDto dto, string guid)
        {
            var model = new ReceiptEntity(guid, dto.Screws, dto.Bolts, dto.Nails, dto.Price, dto.Cvr, dto.State);
            _repository.CreateReceipt(model);
        }
    }
}
