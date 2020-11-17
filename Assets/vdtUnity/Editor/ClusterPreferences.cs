using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace vdtUnity
{
    public class ClusterPreferences
    {
        public static string ConfigFile
        {
            get
            {
                return EditorPrefs.GetString("VDTUnity_ConfigFile", "C:\\ElbedomeShared\\configs\\config.json");
            }
            set
            {
                EditorPrefs.SetString("VDTUnity_ConfigFile", value);
            }
        }

        [PreferenceItem("vdtUnity")]
        static void PreferencesGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            ConfigFile = EditorGUILayout.TextField("Config file path", ConfigFile);

            EditorGUILayout.EndVertical();
        }
    }
}

