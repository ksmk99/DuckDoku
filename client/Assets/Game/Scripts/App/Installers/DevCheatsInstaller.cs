#if UNITY_EDITOR
using Zenject;

namespace DuckDoku.App
{
    public class DevCheatsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IDevClient>().To<DevClient>().AsSingle();
            Container.BindInterfacesAndSelfTo<DevMaxEnergyHotkey>().AsSingle();
        }
    }
}
#endif
