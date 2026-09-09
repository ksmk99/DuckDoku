#if UNITY_EDITOR
using Zenject;

namespace DuckDoku.App
{
    public class GameplayDevCheatsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<DevAutoSolveHotkey>().AsSingle();
        }
    }
}
#endif
