using System;
using DuckDoku.Domain;
using UnityEditor;
using UnityEngine;

namespace DuckDoku.Presentation.Editor
{
    [CustomEditor(typeof(RuleCardConfig))]
    public class RuleCardConfigEditor : UnityEditor.Editor
    {
        private const float CellSize = 32f;
        private const int MaxRegions = 8;

        private static readonly CellState[] CycleStates = { CellState.Empty, CellState.Duck, CellState.Cross };

        private SerializedProperty _title;
        private SerializedProperty _size;
        private SerializedProperty _cells;
        private SerializedProperty _regionColors;
        private GUIStyle _cellLabelStyle;

        private void OnEnable()
        {
            _title = serializedObject.FindProperty("_title");
            _size = serializedObject.FindProperty("_size");
            _cells = serializedObject.FindProperty("_cells");
            _regionColors = serializedObject.FindProperty("_regionColors");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_title);

            int size = Mathf.Clamp(_size.intValue, 2, 5);
            int newSize = EditorGUILayout.IntSlider("Size", size, 2, 5);

            if (newSize != size || _cells.arraySize != size * size)
            {
                Resize(newSize);
                size = newSize;
            }

            if (_regionColors.arraySize == 0)
            {
                ResizeRegionColors(1);
            }

            EditorGUILayout.Space();
            DrawRegionColors();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("LMB — cell state, RMB — region", EditorStyles.miniLabel);
            EditorGUILayout.Space(4);

            DrawGrid(size);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRegionColors()
        {
            EditorGUILayout.LabelField("Regions", EditorStyles.boldLabel);

            int regionCount = _regionColors.arraySize;
            int newRegionCount = EditorGUILayout.IntSlider("Region count", regionCount, 1, MaxRegions);

            if (newRegionCount != regionCount)
            {
                ResizeRegionColors(newRegionCount);
            }

            for (int i = 0; i < _regionColors.arraySize; i++)
            {
                SerializedProperty colorProperty = _regionColors.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(colorProperty, new GUIContent($"Region {i}"));
            }
        }

        private void ResizeRegionColors(int newCount)
        {
            int oldCount = _regionColors.arraySize;
            _regionColors.arraySize = newCount;

            for (int i = oldCount; i < newCount; i++)
            {
                _regionColors.GetArrayElementAtIndex(i).colorValue = DefaultRegionColor(i);
            }
        }

        private static Color DefaultRegionColor(int index)
        {
            return Color.HSVToRGB((index * 0.37f) % 1f, 0.35f, 0.95f);
        }

        private void DrawGrid(int size)
        {
            for (int row = 0; row < size; row++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                for (int column = 0; column < size; column++)
                {
                    DrawCell(size, row, column);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawCell(int size, int row, int column)
        {
            SerializedProperty cell = _cells.GetArrayElementAtIndex(row * size + column);
            SerializedProperty stateProperty = cell.FindPropertyRelative("State");
            SerializedProperty regionProperty = cell.FindPropertyRelative("Region");

            Rect rect = GUILayoutUtility.GetRect(CellSize, CellSize, GUILayout.Width(CellSize), GUILayout.Height(CellSize));

            CellState state = GetEnumValue(stateProperty);
            int regionCount = Mathf.Max(1, _regionColors.arraySize);
            int region = ((regionProperty.intValue % regionCount) + regionCount) % regionCount;

            EditorGUI.DrawRect(rect, _regionColors.GetArrayElementAtIndex(region).colorValue);
            DrawCellBorder(rect);
            DrawCellLabel(rect, state);

            Event evt = Event.current;
            if (evt.type != EventType.MouseDown || !rect.Contains(evt.mousePosition))
            {
                return;
            }

            if (evt.button == 0)
            {
                SetEnumValue(stateProperty, NextState(state));
                evt.Use();
                GUI.changed = true;
                Repaint();
            }
            else if (evt.button == 1)
            {
                regionProperty.intValue = (region + 1) % regionCount;
                evt.Use();
                GUI.changed = true;
                Repaint();
            }
        }

        private void DrawCellLabel(Rect rect, CellState state)
        {
            string label = state switch
            {
                CellState.Duck => "●",
                CellState.Cross => "✕",
                _ => string.Empty
            };

            if (string.IsNullOrEmpty(label))
            {
                return;
            }

            _cellLabelStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18
            };

            Color previousColor = GUI.color;
            GUI.color = state == CellState.Cross ? new Color(0.75f, 0.2f, 0.2f) : Color.black;
            GUI.Label(rect, label, _cellLabelStyle);
            GUI.color = previousColor;
        }

        private void Resize(int newSize)
        {
            var oldCells = new RuleCellConfig[_cells.arraySize];
            for (int i = 0; i < _cells.arraySize; i++)
            {
                SerializedProperty c = _cells.GetArrayElementAtIndex(i);
                oldCells[i] = new RuleCellConfig
                {
                    Row = c.FindPropertyRelative("Row").intValue,
                    Column = c.FindPropertyRelative("Column").intValue,
                    Region = c.FindPropertyRelative("Region").intValue,
                    State = GetEnumValue(c.FindPropertyRelative("State"))
                };
            }

            _size.intValue = newSize;
            _cells.arraySize = newSize * newSize;

            for (int row = 0; row < newSize; row++)
            {
                for (int column = 0; column < newSize; column++)
                {
                    SerializedProperty c = _cells.GetArrayElementAtIndex(row * newSize + column);
                    c.FindPropertyRelative("Row").intValue = row;
                    c.FindPropertyRelative("Column").intValue = column;

                    int existingIndex = Array.FindIndex(oldCells, x => x.Row == row && x.Column == column);
                    bool found = existingIndex >= 0;

                    c.FindPropertyRelative("Region").intValue = found ? oldCells[existingIndex].Region : 0;
                    SetEnumValue(c.FindPropertyRelative("State"), found ? oldCells[existingIndex].State : CellState.Empty);
                }
            }
        }

        private static void DrawCellBorder(Rect rect)
        {
            Color border = new Color(0.15f, 0.15f, 0.15f, 0.6f);

            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), border);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1, rect.width, 1), border);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, 1, rect.height), border);
            EditorGUI.DrawRect(new Rect(rect.xMax - 1, rect.y, 1, rect.height), border);
        }

        private static CellState NextState(CellState current)
        {
            int index = Array.IndexOf(CycleStates, current);
            if (index < 0)
            {
                index = 0;
            }

            return CycleStates[(index + 1) % CycleStates.Length];
        }

        private static CellState GetEnumValue(SerializedProperty property)
        {
            return (CellState)Enum.GetValues(typeof(CellState)).GetValue(property.enumValueIndex);
        }

        private static void SetEnumValue(SerializedProperty property, CellState value)
        {
            property.enumValueIndex = Array.IndexOf((CellState[])Enum.GetValues(typeof(CellState)), value);
        }
    }
}
