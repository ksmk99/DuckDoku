using DuckDoku.Domain;
using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class EnergyInstaller : MonoInstaller
    {
        [SerializeField] private EnergyPopupView _energyPopupPrefab;

        public override void InstallBindings()
        {
            Container.Bind<IEnergyClient>().To<EnergyClient>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnergyService>().AsSingle();
            Container.BindInstance(_energyPopupPrefab);
        }
    }
}
