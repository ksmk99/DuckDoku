using DuckDoku.Domain;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DuckDoku.App
{
    public class LevelsInstaller : MonoInstaller
    {
        [SerializeField] private LevelCatalogAsset _levelCatalog;
        
        public override void InstallBindings()
        {
            Container.Bind<LevelCatalogAsset>().FromInstance(_levelCatalog).AsSingle();

            Container.Bind<ILevelLauncher>().To<LevelLauncher>().AsSingle();
            Container.Bind<ILevelEntryGate>().To<LevelEntryGate>().AsSingle();
            Container.Bind<ILevelMapSource>().To<LevelMapSource>().AsSingle();
            Container.Bind<ILevelSessionService>().To<LevelSessionService>().AsSingle();
            Container.Bind<ILevelFinishContext>().To<LevelFinishContext>().AsSingle();
        }
    }
}
