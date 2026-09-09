using Zenject;

namespace DuckDoku.App
{
    public class StateMachineInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IStateMachine>()
                .To<SceneStateMachine>()
                .FromMethod(_ => new SceneStateMachine(new BootSceneState()))
                .AsSingle();
        }
    }
}
