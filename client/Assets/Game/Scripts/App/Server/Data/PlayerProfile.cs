using System;                                                                         
                                                                                        
namespace DuckDoku.App                                                                
{                                                                                     
    [Serializable]                                                                    
    public class PlayerProfile                                                        
    {                                                                                 
        public string playerId;                                                       
        public string displayName;                                                    
        public string createdAt;                                                      
        public string lastSeenAt;                                                     
                                                                                        
        public bool HasName => !string.IsNullOrEmpty(displayName);                    
    }                                                                                 
}