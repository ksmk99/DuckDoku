using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class CoreInstaller : MonoInstaller
    {
        [SerializeField] private ServerConfig _serverConfig;

        public override void InstallBindings()
        {
            Container.Bind<ServerConfig>().FromInstance(_serverConfig).AsSingle();
            Container.Bind<PlayerSession>().AsSingle();

            Container.Bind<IDeviceIdProvider>().To<DeviceIdProvider>().AsSingle();
            Container.Bind<IServerTimeService>().To<ServerTimeService>().AsSingle();
        }
    }
}
