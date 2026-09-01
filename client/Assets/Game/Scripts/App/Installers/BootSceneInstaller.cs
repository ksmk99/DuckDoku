  using TMPro;                                                                                                                                                                                           
  using UnityEngine;                                                                                                                                                                                     
  using Zenject;                                                                                                                                                                                         
                                                                                                                                                                                                         
  namespace DuckDoku.App                                                                                                                                                                                 
  {                                                                                                                                                                                                      
      public class BootSceneInstaller : MonoInstaller                                                                                                                                                    
      {                                                                                                                                                                                                  
          [SerializeField] private TMP_Text _markerOutput;                                                                                                                                               
                                                                                                                                                                                                         
          public override void InstallBindings()                                                                                                                                                         
          {                                                                                                                                                                                              
              Container.Bind<TMP_Text>().FromInstance(_markerOutput).AsSingle();    
              Container.Bind<StartupMarker>().AsSingle();
              Container.BindInterfacesTo<BootMarkerPresenter>().AsSingle();                                                                                                                              
          }                                                                                                                                                                                              
      }                                                                                                                                                                                                  
  }  