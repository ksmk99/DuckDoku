using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class LevelFinishSceneInstaller : MonoInstaller
    {
        [SerializeField] private LevelFinishView _levelFinishView;

        public override void InstallBindings()
        {
            Container.BindInstance(_levelFinishView);
            Container.BindInterfacesTo<LevelFinishPresenter>().AsSingle();
        }
    }
}
