using DuckDoku.Domain;
using Zenject;

namespace DuckDoku.App
{
    public class CurrencyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ICurrencyClient>().To<CurrencyClient>().AsSingle();
            Container.BindInterfacesAndSelfTo<CurrencyService>().AsSingle();
        }
    }
}
