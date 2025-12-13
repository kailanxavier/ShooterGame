using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectSpawner))]
public class SpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ObjectSpawner spawner = (ObjectSpawner)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Start"))
        {
            spawner.isPlaying = true;
        }

        if (GUILayout.Button("Stop"))
        {
            spawner.isPlaying = false;
        }
    }
}