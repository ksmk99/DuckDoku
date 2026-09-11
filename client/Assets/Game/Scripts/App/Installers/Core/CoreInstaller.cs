using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class CoreInstaller : MonoInstaller
    {
        [SerializeField] private ServerConfig _serverConfig;
        [SerializeField] private SfxConfig _sfxConfig;

        public override void InstallBindings()
        {
            Container.Bind<ServerConfig>().FromInstance(_serverConfig).AsSingle();
            Container.Bind<PlayerSession>().AsSingle();

            Container.Bind<IDeviceIdProvider>().To<DeviceIdProvider>().AsSingle();
            Container.Bind<IServerTimeService>().To<ServerTimeService>().AsSingle();

            Container.Bind<IAudioSettings>().To<AudioSettings>().AsSingle();
            Container.Bind<SfxConfig>().FromInstance(_sfxConfig).AsSingle();
            Container.BindInterfacesTo<SfxPlayer>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("SfxPlayer")
                .AsSingle()
                .NonLazy();
        }
    }
}
