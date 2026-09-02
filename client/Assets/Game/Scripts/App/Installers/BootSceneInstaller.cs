using TMPro;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class BootSceneInstaller : MonoInstaller
    {
        [SerializeField] private TMP_Text _authOutput;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BootAuthPresenter>().AsSingle().WithArguments(_authOutput);
        }
    }
}