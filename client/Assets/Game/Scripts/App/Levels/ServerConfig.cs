using UnityEngine;

namespace DuckDoku.App
{
    [CreateAssetMenu(fileName = "ServerConfig", menuName = "DuckDoku/Server Config")]
    public class ServerConfig : ScriptableObject
    {
        [SerializeField] private string _baseUrl = "http://localhost:5190";

        public string BaseUrl => _baseUrl.TrimEnd('/');
    }
}