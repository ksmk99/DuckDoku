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
            Container.BindInterfacesTo<BoardPresenter>().AsSingle();
        }
    }
}