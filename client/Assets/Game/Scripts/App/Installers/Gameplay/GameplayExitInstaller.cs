using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplayExitInstaller : MonoInstaller
    {
        [SerializeField] private ExitLevelView _exitLevelView;

        public override void InstallBindings()
        {
            Container.BindInstance(_exitLevelView);
        }
    }
}
