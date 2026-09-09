using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class MetaEnergyInstaller : MonoInstaller
    {
        [SerializeField] private EnergyView _energyView;

        public override void InstallBindings()
        {
            Container.BindInstance(_energyView);
            Container.BindInterfacesAndSelfTo<EnergyPresenter>().AsSingle();
        }
    }
}
