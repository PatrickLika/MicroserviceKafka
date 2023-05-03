namespace Storage.Application.Commands
{
    public interface IStorageCommand
    {
        void CheckStorage(StorageDto dto);
    }
}
