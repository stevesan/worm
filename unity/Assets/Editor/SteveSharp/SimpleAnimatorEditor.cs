
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

namespace SteveSharp
{
    [CustomEditor(typeof(SimpleAnimator))]
    public class SimpleAnimatorEditor : Editor
    {
        SimpleAnimator.Key newKey = new SimpleAnimator.Key();

        SimpleAnimator anim { get { return (SimpleAnimator)target; } }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            GUILayout.BeginVertical("box");
            
            newKey.motionTime = EditorGUILayout.FloatField("Motion Time", newKey.motionTime);
            newKey.afterTime = EditorGUILayout.FloatField("After Time", newKey.afterTime);

            if( GUILayout.Button("Add Key Frame" ) )
            {
                newKey.position = anim.gameObject.transform.position;
                anim.keys.Add(newKey);
                newKey = new SimpleAnimator.Key();
            }

            GUILayout.EndVertical();
        }
    }
}
