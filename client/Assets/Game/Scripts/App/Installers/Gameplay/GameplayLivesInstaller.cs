using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplayLivesInstaller : MonoInstaller
    {
        [SerializeField] private LivesView _livesView;

        public override void InstallBindings()
        {
            Container.BindInstance(_livesView);
            Container.BindInterfacesAndSelfTo<LivesPresenter>().AsSingle();
        }
    }
}
