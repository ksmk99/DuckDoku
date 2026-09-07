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

        public override void InstallBindings()
        {
            Container.BindInstance(_boardView);
            Container.BindInstance(_gameplayView);

            Container.Bind<ILevelSource>().To<CatalogLevelSource>().AsSingle();
            Container.Bind<IBoardSessionFactory>().To<BoardSessionFactory>().AsSingle();

            Container.BindInterfacesAndSelfTo<BoardPresenter>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameplayCoordinator>().AsSingle();

#if UNITY_EDITOR
            Container.BindInterfacesAndSelfTo<DevAutoSolveHotkey>().AsSingle();
#endif
        }
    }
}
