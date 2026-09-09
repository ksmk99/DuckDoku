using System.Collections.Generic;
using UnityEngine;

namespace DuckDoku.Presentation
{
    [CreateAssetMenu(fileName = "Color Palette Config", menuName = "DuckDoku    /Color Palette Config")]
    public class ColorPaletteConfig : ScriptableObject
    {
        [SerializeField] private Color[] _colors;
        
        public IReadOnlyList<Color> Colors => _colors;
    }
}