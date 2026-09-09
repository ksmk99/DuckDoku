using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class PopupInstaller : MonoInstaller
    {
        [SerializeField] private PopupRoot _popupRoot;

        public override void InstallBindings()
        {
            Container.BindInstance(_popupRoot);
            Container.BindInterfacesAndSelfTo<PopupService>().AsSingle();
        }
    }
}
