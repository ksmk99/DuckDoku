using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class BootSceneInstaller : MonoInstaller
    {
        [SerializeField] private BootAuthView _bootAuthView;
        [SerializeField] private BootRetryConfig _bootRetryConfig;

        public override void InstallBindings()
        {
            Container.Bind<RemoteLoadCoordinator>().AsSingle();

            Container.BindInterfacesTo<BootAuthPresenter>().AsSingle()
                .WithArguments(_bootAuthView, _bootRetryConfig);
        }
    }
}
