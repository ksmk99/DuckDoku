namespace DuckDoku.Contracts
{
    public enum ErrorCode                                                    
    {                                                                        
        Unknown = 0,                                                         
                                                                               
        InvalidRequest = 1,                                                  
        UnsupportedClientVersion = 2,                                        
                                                                               
        TokenExpired = 10,                                                   
        TokenRevoked = 11,                                                   
        DeviceBanned = 12,                                                   
                                                                               
        NotEnoughEnergy = 20,
        NotEnoughCurrency = 21,
        EnergyFull = 22,
        NotEnoughHints = 23,

        LevelLocked = 30,                                                    
        SessionExpired = 31,                                                 
        SessionAlreadyClaimed = 32,                                          
        InvalidSolution = 33,                                                
        VersionConflict = 34,                                                
                                                                               
        RateLimited = 40,                                                    
        ServerUnavailable = 41                                               
    }     
}