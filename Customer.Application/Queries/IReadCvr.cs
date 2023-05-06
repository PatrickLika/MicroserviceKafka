using Customer.Application.Queries;

namespace Customer.Application.Queries
{
    public interface IReadCvr
    {
        void ReadCvr(ReadCvrDto dto);
    }
}