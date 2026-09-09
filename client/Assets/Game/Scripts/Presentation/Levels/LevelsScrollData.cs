using System;
using UnityEngine;
using UnityEngine.UI;

namespace DuckDoku.Presentation
{
    [Serializable]
    public class LevelsScrollData
    {
        [field: SerializeField] public ScrollRect ScrollRect { get; set; }
        [field: SerializeField] public RectTransform Viewport { get; set; }
        [field: SerializeField] public RectTransform Content { get; set; }
        [field: SerializeField] public LevelView LevelPrefab { get; set; }

        [field: SerializeField] public int Columns { get; set; } = 5;
        [field: SerializeField] public float CellHeight { get; set; } = 200f;
        [field: SerializeField] public float SpacingX { get; set; } = 10f;
        [field: SerializeField] public float SpacingY { get; set; } = 10f;
        [field: SerializeField] public float PaddingLeft { get; set; }
        [field: SerializeField] public float PaddingRight { get; set; }
        [field: SerializeField] public float PaddingTop { get; set; }
        [field: SerializeField] public float PaddingBottom { get; set; }
    }
}
