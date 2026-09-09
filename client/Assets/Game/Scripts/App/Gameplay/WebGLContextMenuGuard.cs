using System.Runtime.InteropServices;
using UnityEngine;

namespace DuckDoku.App
{
    public class WebGLContextMenuGuard : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void DisableContextMenu();
#endif

        private void Awake()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            DisableContextMenu();
#endif
        }
    }
}
