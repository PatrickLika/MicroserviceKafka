using Customer.Application.Queries;

namespace Costumer.Application.Repository
{
    public interface IRepository
    {
        void ReadCvr(ReadCvrDto dto);
    }
}
