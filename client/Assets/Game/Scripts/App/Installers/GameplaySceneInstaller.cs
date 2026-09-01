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
        [SerializeField] private GameplaySettings _settings;

        public override void InstallBindings()
        {
            Container.BindInstance(_settings);
            Container.BindInstance(_boardView);
            Container.BindInstance(_gameplayView);

            Container.Bind<ILevelSource>().To<LocalLevelSource>().AsSingle();
            Container.BindInterfacesTo<BoardPresenter>().AsSingle();
        }
    }
}