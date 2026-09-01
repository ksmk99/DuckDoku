using TMPro;                                                                                                                                                                                           
  using Zenject;                                                                                                                                                                                         
                                                                                                                                                                                                         
  namespace DuckDoku.App                                                                                                                                                                                 
  {                                                                                                                                                                                                      
      public class BootMarkerPresenter : IInitializable                                                                                                                                                  
      {                                                                                                                                                                                                  
          private readonly StartupMarker _marker;                                                                                                                                                        
          private readonly TMP_Text _output;                                                                                                                                                             
                                                                                                                                                                                                         
          [Inject]                                                                                                                                                                                       
          public BootMarkerPresenter(StartupMarker marker, TMP_Text output)                                                                                                                              
          {                                                                                                                                                                                              
              _marker = marker;                                                                                                                                                                          
              _output = output;                                                                                                                                                                          
          }                                                                                                                                                                                              
                                                                                                                                                                                                         
          public void Initialize()                                                                                                                                                                       
          {                                                                                                                                                                                              
              _output.text = _marker.GetMarker();    
                                                                                                                                                         
          }                                                                                                                                                                                              
      }                                                                                                                                                                                                  
  }         