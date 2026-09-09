using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class CurrencyDisplayInstaller : MonoInstaller
    {
        [SerializeField] private CurrencyView _currencyView;

        public override void InstallBindings()
        {
            Container.BindInstance(_currencyView);
            Container.BindInterfacesAndSelfTo<CurrencyPresenter>().AsSingle();
        }
    }
}
