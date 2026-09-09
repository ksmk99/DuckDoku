using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplayLevelTimerInstaller : MonoInstaller
    {
        [SerializeField] private LevelTimerView _levelTimerView;

        public override void InstallBindings()
        {
            Container.BindInstance(_levelTimerView);
            Container.BindInterfacesAndSelfTo<LevelTimerPresenter>().AsSingle();
        }
    }
}
