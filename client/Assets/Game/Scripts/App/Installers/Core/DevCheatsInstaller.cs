using Zenject;

namespace DuckDoku.App
{
    public class DevCheatsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
#if UNITY_EDITOR
            Container.Bind<IDevClient>().To<DevClient>().AsSingle();
            Container.BindInterfacesAndSelfTo<DevMaxEnergyHotkey>().AsSingle();
            Container.BindInterfacesAndSelfTo<DevGrantCurrencyHotkey>().AsSingle();
#endif
        }
    }
}
