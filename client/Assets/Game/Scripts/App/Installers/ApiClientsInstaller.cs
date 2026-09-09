using Zenject;

namespace DuckDoku.App
{
    public class ApiClientsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IGuestAuthClient>().To<GuestAuthClient>().AsSingle();
            Container.Bind<IProfileClient>().To<ProfileClient>().AsSingle();
            Container.Bind<ITimeClient>().To<TimeClient>().AsSingle();
            Container.Bind<ILevelsClient>().To<LevelsClient>().AsSingle();
        }
    }
}
