using System;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    [Serializable]
    public class LevelFactoryGridData
    {
        [field: SerializeField]
        public RectTransform Grid { get; set; }
        [field: SerializeField]
        public  GridLayoutGroup Layout  { get; set; }
        [field: SerializeField]
        public  LevelView LevelPrefab  { get; set; }
        [field: SerializeField]
        public int RowCount { get; set; } = 5;
    }
}