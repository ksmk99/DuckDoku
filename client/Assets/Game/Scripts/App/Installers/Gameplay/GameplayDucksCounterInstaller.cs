using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplayDucksCounterInstaller : MonoInstaller
    {
        [SerializeField] private DucksCounterView _ducksCounterView;

        public override void InstallBindings()
        {
            Container.BindInstance(_ducksCounterView);
            Container.BindInterfacesAndSelfTo<DucksCounterPresenter>().AsSingle();
        }
    }
}
