using DuckDoku.Contracts;
using UnityEngine;
using Zenject;

namespace DuckDoku.App
{
    public class StartupMarker
    {
        public string Marker { get; private set; } = string.Empty;

        [Inject]
        public StartupMarker()
        {

        }

        public string GetMarker()
        {
            Marker = ContractsVersion.Value;
            Debug.Log(Marker);
            
            return Marker;
        }
    }
}