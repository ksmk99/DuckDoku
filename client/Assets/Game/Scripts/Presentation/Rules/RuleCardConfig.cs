using System;
using System.Collections.Generic;
using DuckDoku.Domain;
using UnityEngine;

namespace DuckDoku.Presentation
{
    [Serializable]
    public struct RuleCellConfig
    {
        public int Row;
        public int Column;
        public int Region;
        public CellState State;
    }

    [CreateAssetMenu(fileName = "Rule Card Config", menuName = "DuckDoku/Rules/Rule Card Config")]
    public class RuleCardConfig : ScriptableObject
    {
        [SerializeField] private string _title;
        [SerializeField] private int _size = 3;
        [SerializeField] private RuleCellConfig[] _cells;
        [SerializeField] private Color[] _regionColors = { Color.white };

        public string Title => _title;
        public int Size => _size;
        public IReadOnlyList<RuleCellConfig> Cells => _cells;
        public IReadOnlyList<Color> RegionColors => _regionColors;
    }
}
