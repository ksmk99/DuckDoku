using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class MetaSceneInstaller: MonoInstaller
    {
        [SerializeField] private LevelFactoryGridData _levelsFactoryData;
        [SerializeField] private EnergyView _energyView;

        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<LevelGridFactory>()
                .AsTransient()
                .WithArguments(_levelsFactoryData);

            Container.BindInstance(_energyView);
            Container.BindInterfacesAndSelfTo<EnergyPresenter>().AsSingle();
        }
    }
}