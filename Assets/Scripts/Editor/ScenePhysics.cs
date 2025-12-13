using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VerySeriousAndNotUnnecessarilyLongNameForAwesomePhysicsToolThatDoesntWorkProperly
{
    public class ScenePhysics : EditorWindow
    {
        bool isPlaying = false;

        private void OnEnable()
        {
            Undo.undoRedoEvent += UndoRedoEvent;
        }

        private void OnDisable()
        {
            Undo.undoRedoEvent -= UndoRedoEvent;

            if (isPlaying)
                Stop();
        }

        private void UndoRedoEvent(in UndoRedoInfo undo)
        {
            Stop();
        }

        private void OnGUI()
        {
            if (!isPlaying)
            {
                if (GUILayout.Button("Start Simulations"))
                {
                    isPlaying = true;
                    RecordUndo();
                    EditorApplication.update += StepPhysics;
                }
            }
            else
            {
                if (GUILayout.Button("Stop Simulations") && isPlaying) Stop();
            }
        }

        Rigidbody[] rigidbodies;

        void Stop()
        {
            isPlaying = false;
            EditorApplication.update -= StepPhysics;

            if (isPlaying)
                return;

            if (rigidbodies != null)
            {
                foreach (var rb in rigidbodies)
                {
                    rb.angularVelocity = Vector3.zero;
                    rb.linearVelocity = Vector3.zero;
                }
            }
        }

        void RecordUndo()
        {
            rigidbodies = GameObject.FindObjectsByType<Rigidbody>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            var transforms = rigidbodies.Select(x => (UnityEngine.Object)x.transform).ToArray();
            Undo.RecordObjects(transforms, "Simulate Physics");
        }

        private void StepPhysics()
        {
            var simMode = Physics.simulationMode;
            Physics.simulationMode = SimulationMode.Script;
            Physics.Simulate(Time.fixedDeltaTime);
            Physics.simulationMode = simMode;

            foreach (var rb in rigidbodies)
                EditorUtility.SetDirty(rb.transform);
        }

        [MenuItem("Tools/Editor Physics")]
        private static void OpenWindow()
        {
            GetWindow<ScenePhysics>(false, "Editor Physics", true);
        }
    }
}