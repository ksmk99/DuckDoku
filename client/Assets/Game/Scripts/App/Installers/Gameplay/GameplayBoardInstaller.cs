using DuckDoku.Domain;
using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplayBoardInstaller : MonoInstaller
    {
        [SerializeField] private BoardView _boardView;
        [SerializeField] private CellFeedbackConfig _cellFeedbackConfig;
        [SerializeField] private BoardInputConfig _boardInputConfig;

        public override void InstallBindings()
        {
            Container.BindInstance(_boardView);
            Container.Bind<CellFeedbackConfig>().FromInstance(_cellFeedbackConfig).AsSingle();
            Container.Bind<BoardInputConfig>().FromInstance(_boardInputConfig).AsSingle();

            Container.Bind<IBoardSessionFactory>().To<BoardSessionFactory>().AsSingle();
            Container.Bind<HintService>().AsSingle();

            Container.BindInterfacesAndSelfTo<BoardPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<BoardHoverPresenter>().AsSingle();
        }
    }
}
