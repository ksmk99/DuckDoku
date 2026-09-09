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
            Container.Bind<IEnergyService>().To<EnergyService>().AsSingle();
            Container.BindInstance(_energyPopupPrefab);
        }
    }
}
