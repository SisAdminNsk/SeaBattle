using Microsoft.Extensions.DependencyInjection;

namespace SeaBattleGame.DI
{
    public class DependencyInjector
    {
        private static readonly object _lock = new();

        private IServiceCollection _services = new ServiceCollection();
        private IServiceProvider _serviceProvider;
        public ServiceType GetRequiredService<ServiceType>()
        {
            lock (_lock)
            {
                return _serviceProvider.GetRequiredService<ServiceType>()
                    ?? throw new ArgumentNullException($"Service: {nameof(ServiceType)} was not finded. " +
                    $"The reason for this may be that the type was not registered in DI");
            }
        }
        public DependencyInjector()
        {
            _serviceProvider = _services.BuildServiceProvider();
        }
    }
}
