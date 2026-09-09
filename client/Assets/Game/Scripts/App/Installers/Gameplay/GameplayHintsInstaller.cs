using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplayHintsInstaller : MonoInstaller
    {
        [SerializeField] private HintsView _hintsView;
        [SerializeField] private HintsPopupView _hintsPopupPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(_hintsView);
            Container.BindInstance(_hintsPopupPrefab);

            Container.BindInterfacesAndSelfTo<HintsPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<HintFlowCoordinator>().AsSingle();
        }
    }
}
