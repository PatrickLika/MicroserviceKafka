namespace Storage.Application.Commands
{
    public interface IRollBackStorage
    {
        void RollBackStorage(StorageDbDto dto);
    }
}
