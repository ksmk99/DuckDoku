using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private MainMenuView mainMenuView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MainMenuPresenter>().AsSingle().WithArguments(mainMenuView);
        }
    }
}