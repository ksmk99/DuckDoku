using System;
using DuckDoku.Puzzle;
using UnityEngine;

namespace DuckDoku.Presentation
{
    [Serializable]
    public class GameplaySettings
    {
        [SerializeField] [Range(PuzzleDefinition.MinSize, PuzzleDefinition.MaxSize)]
        private int _size = PuzzleDefinition.MinSize;

        [SerializeField] private PuzzleDifficulty _difficulty = PuzzleDifficulty.Normal;

        public int Size => _size;

        public PuzzleDifficulty Difficulty => _difficulty;
    }
}