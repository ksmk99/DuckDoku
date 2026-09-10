using Zenject;

namespace DuckDoku.App
{
    public class GameplayDevCheatsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
#if UNITY_EDITOR
            Container.BindInterfacesAndSelfTo<DevAutoSolveHotkey>().AsSingle();
#endif
        }
    }
}
