using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplayLevelIndicatorInstaller : MonoInstaller
    {
        [SerializeField] private LevelIndicatorView _levelIndicatorView;

        public override void InstallBindings()
        {
            Container.BindInstance(_levelIndicatorView);
            Container.BindInterfacesAndSelfTo<LevelIndicatorPresenter>().AsSingle();
        }
    }
}
