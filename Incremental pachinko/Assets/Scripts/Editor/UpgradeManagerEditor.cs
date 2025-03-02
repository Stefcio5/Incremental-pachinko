//// filepath: /F:/Projects/Unity projects/Incremental-pachinko/Incremental pachinko/Assets/Editor/UpgradeManagerEditor.cs
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using BreakInfinity;

[CustomEditor(typeof(UpgradeManager))]
public class UpgradeManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector for UpgradeManager

        UpgradeManager manager = (UpgradeManager)target;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Upgrades Debug", EditorStyles.boldLabel);

        if (manager.upgradeMap == null || manager.upgradeMap.Count == 0)
        {
            EditorGUILayout.LabelField("No upgrades available.");
            return;
        }

        // Loop through each upgrade in the upgradeMap
        foreach (KeyValuePair<string, Upgrade> kvp in manager.upgradeMap)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(kvp.Key, GUILayout.MaxWidth(150));

            //TODO: Change to BigDouble
            // For simplicity, we display currentLevel as a float (assuming it can be represented as float)
            float currentLevel = (float)kvp.Value.CurrentLevel;
            float newLevel = EditorGUILayout.FloatField(currentLevel);

            // If the level is changed, update it via the provided API (which also fires the event)
            if (!Mathf.Approximately(currentLevel, newLevel))
            {
                kvp.Value.UpdateLevel(newLevel);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Reset All Upgrades (Set Level=0)"))
        {
            foreach (var kvp in manager.upgradeMap)
            {
                kvp.Value.UpdateLevel(0);
            }
        }
    }
}