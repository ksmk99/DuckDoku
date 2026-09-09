using DuckDoku.Presentation;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace DuckDoku.App
{
    public class MetaLevelsInstaller : MonoInstaller
    {
        [SerializeField] private LevelsScrollData _levelsScrollData;
        [Space] 
        [SerializeField] private Button levelsOpenButton;
        [SerializeField] private Button levelsCloseButton;
        [SerializeField] private GameObject levelsObject;

        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<LevelsScrollController>()
                .AsSingle()
                .WithArguments(_levelsScrollData);

            Container
                .BindInterfacesAndSelfTo<FrameToggle<LevelsScrollController>>()
                .AsCached()
                .WithArguments(levelsOpenButton, levelsCloseButton, levelsObject);
        }
    }
}