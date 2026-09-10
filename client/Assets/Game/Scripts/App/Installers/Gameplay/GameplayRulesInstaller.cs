using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class GameplayRulesInstaller : MonoInstaller
    {
        [SerializeField] private RulesStripView _rulesStripView;

        public override void InstallBindings()
        {
            Container.BindInstance(_rulesStripView);
        }
    }
}
