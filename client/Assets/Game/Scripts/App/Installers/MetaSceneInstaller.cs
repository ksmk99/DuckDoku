using DuckDoku.Presentation;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class MetaSceneInstaller: MonoInstaller
    {
        [SerializeField] private LevelFactoryGridData _levelsFactoryData;
        
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<LevelGridFactory>()
                .AsTransient()
                .WithArguments(_levelsFactoryData);
        }
    }
}