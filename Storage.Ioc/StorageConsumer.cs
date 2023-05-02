namespace Storage.Ioc
{
    public class StorageConsumer : IHostedService
    {
        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
