using DuckDoku.Domain;
using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ILevelSource>().To<CatalogLevelSource>().AsSingle();
            Container.Bind<ILevelStartSignal>().To<LevelStartSignal>().AsSingle();

            Container.BindInterfacesAndSelfTo<GameplayCoordinator>().AsSingle();
        }
    }
}
