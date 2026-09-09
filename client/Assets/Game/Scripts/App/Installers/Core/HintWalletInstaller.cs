using DuckDoku.Domain;
using Zenject;

namespace DuckDoku.App
{
    public class HintWalletInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IHintWalletClient>().To<HintWalletClient>().AsSingle();
            Container.BindInterfacesAndSelfTo<HintWalletService>().AsSingle();
        }
    }
}
