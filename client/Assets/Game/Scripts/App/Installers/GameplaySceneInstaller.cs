using DuckDoku.Domain;
using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private BoardView _boardView;
        [SerializeField] private GamePlayView _gameplayView;
        [SerializeField] private LivesView _livesView;
        [SerializeField] private CellFeedbackConfig _cellFeedbackConfig;

        public override void InstallBindings()
        {
            Container.BindInstance(_boardView);
            Container.BindInstance(_gameplayView);
            Container.BindInstance(_livesView);
            Container.Bind<CellFeedbackConfig>().FromInstance(_cellFeedbackConfig).AsSingle();

            Container.Bind<ILevelSource>().To<CatalogLevelSource>().AsSingle();
            Container.Bind<IBoardSessionFactory>().To<BoardSessionFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<BoardPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<LivesPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameplayCoordinator>().AsSingle();
        }
    }
}
