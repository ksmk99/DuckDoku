using DuckDoku.Domain;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private ServerConfig _serverConfig;
        [SerializeField] private LevelCatalogAsset _levelCatalog;

        public override void InstallBindings()
        {
            Container.Bind<ServerConfig>().FromInstance(_serverConfig).AsSingle();
            Container.Bind<LevelCatalogAsset>().FromInstance(_levelCatalog).AsSingle();
            Container.Bind<PlayerSession>().AsSingle();

            Container.Bind<IStateMachine>()
                .To<SceneStateMachine>()
                .FromMethod(_ => new SceneStateMachine(new BootSceneState()))
                .AsSingle();

            Container.Bind<IDeviceIdProvider>().To<DeviceIdProvider>().AsSingle();
            Container.Bind<IServerTimeService>().To<ServerTimeService>().AsSingle();

            Container.Bind<IGuestAuthClient>().To<GuestAuthClient>().AsSingle();
            Container.Bind<IProfileClient>().To<ProfileClient>().AsSingle();
            Container.Bind<ITimeClient>().To<TimeClient>().AsSingle();
            Container.Bind<ILevelsClient>().To<LevelsClient>().AsSingle();

            Container.Bind<ILevelLauncher>().To<LevelLauncher>().AsSingle();
            Container.Bind<ILevelMapSource>().To<LevelMapSource>().AsSingle();
            Container.Bind<ILevelSessionService>().To<LevelSessionService>().AsSingle();
        }
    }
}
