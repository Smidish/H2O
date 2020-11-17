using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Linq;
using Mirror;
using UnityEditor.SceneManagement;
using vdtUnity;

namespace vdtUnity
{
    public class ClusterBuilderMenu
    {
        [MenuItem("vdtUnity/Setup Tags", priority = 0)]
        static void MenuSetupTags()
        {
            SetupTags();
        }

        [MenuItem("vdtUnity/Create Network Object (Telepathy)", priority = 1)]
        static void MenuCreateNetworkObject()
        {
            //VDTUnityBuilderWindow window = EditorWindow.GetWindow<VDTUnityBuilderWindow>();
            //window.Show();
            //string configFilename = EditorUtility.OpenFilePanel("Pick ConfigFile", Directory.GetParent(Application.dataPath).FullName, "json");
            string configFilename = ClusterPreferences.ConfigFile;
            if (configFilename != null && configFilename != "")
            {
                SetupTags();
                var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/vdtUnity/Prefabs/Player.prefab");
                GameObject networkObject = ClusterBuilder.CreateNetworkRoot<TelepathyTransport>(configFilename, playerPrefab);
                if (networkObject != null)
                {
                    Undo.RegisterCreatedObjectUndo(networkObject, "Create Network Object");
                }
            }
        }
        
        // [MenuItem("vdtUnity/Create Network Object (Apathy)", priority = 1)]
        // static void MenuCreateApathyNetworkObject()
        // {
        //     //VDTUnityBuilderWindow window = EditorWindow.GetWindow<VDTUnityBuilderWindow>();
        //     //window.Show();
        //     //string configFilename = EditorUtility.OpenFilePanel("Pick ConfigFile", Directory.GetParent(Application.dataPath).FullName, "json");
        //     string configFilename = ClusterPreferences.ConfigFile;
        //     if (configFilename != null && configFilename != "")
        //     {
        //         SetupTags();
        //         var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/vdtUnity/Prefabs/Player.prefab");
        //         GameObject networkObject = ClusterBuilder.CreateNetworkRoot<ApathyTransport>(configFilename, playerPrefab);
        //         if (networkObject != null)
        //         {
        //             Undo.RegisterCreatedObjectUndo(networkObject, "Create Network Object");
        //         }
        //     }
        // }

        //[MenuItem("vdtUnity/Create Network Object (EditorOnly)", priority = 2)]
        //static void MenuCreateNetworkObjectEditorOnly()
        //{
        //    //VDTUnityBuilderWindow window = EditorWindow.GetWindow<VDTUnityBuilderWindow>();
        //    //window.Show();
        //    string configFilename = EditorUtility.OpenFilePanel("Pick ConfigFile", Directory.GetParent(Application.dataPath).FullName, "json");
        //    if (configFilename != null && configFilename != "")
        //    {
        //        SetupTags();
        //        var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/vdtUnity/Prefabs/Player.prefab");
        //        GameObject networkObject = vdtUnity.ClusterBuilder.CreateNetworkRoot(configFilename, playerPrefab);
        //        if (networkObject != null)
        //        {
        //            networkObject.name += " (EditorOnly)";
        //            //GameObject editorOnlyObject = new GameObject("[EditorOnly]");
        //            //editorOnlyObject.tag = "EditorOnly";
        //            networkObject.AddComponent<SlaveSimulator>();
        //            Undo.RegisterCreatedObjectUndo(networkObject, "Create Network Object");
        //        }
        //    }
        //}

        [MenuItem("vdtUnity/Create Dome Object", priority = 2)]
        static void MenuCreateDomeObject()
        {
            CreateDomeObject();
        }

        [MenuItem("vdtUnity/Replace Dome Objects in all Build Scenes", priority = 3)]
        static void MenuReplaceDomeObjectsInAllBuildScenes()
        {
            var scenePath = EditorSceneManager.GetActiveScene().path;
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    Debug.Log("Opening scene " + scene.path);
                    EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                    var clusterUCNetwork = GameObject.FindObjectOfType<ClusterUCNetwork>();
                    if (clusterUCNetwork)
                    {
                        Object.DestroyImmediate(clusterUCNetwork.gameObject);
                    }
                    CreateDomeObject();
                    EditorSceneManager.SaveOpenScenes();
                }
            }
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        [MenuItem("vdtUnity/Create SteamVR Controller", priority = 4)]
        static void MenuCreateSteamVRController()
        {
            var steamVRController = CreateSteamVRController();

            // DisableAllRotationCompressions(steamVRController.gameObject);
        }

        static void CreateDomeObject()
        {
            string configFilename = ClusterPreferences.ConfigFile;
            if (configFilename != null && configFilename != "")
            {
                SetupTags();
                GameObject domeObject = vdtUnity.ClusterBuilder.CreateDomeObject(configFilename);
                if (domeObject != null)
                {
                    CreateSteamVRController();

                    // DisableAllRotationCompressions(domeObject);

                    Undo.RegisterCreatedObjectUndo(domeObject, "Create Dome Object");
                }
            }
        }

        static vdtUnity.SteamVRTrackedController CreateSteamVRController()
        {
            var pointerMaterial = new Material(Shader.Find("Unlit/Color"));
            var steamVRController = vdtUnity.ClusterBuilder.CreateSteamVRTrackedController("SteamVRTrackedController");

            var controllerModelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/vdtUnity/Models/ViveController/ViveControllerPrefab.prefab");
            var controllerModel = PrefabUtility.InstantiatePrefab(controllerModelPrefab) as GameObject;
            controllerModel.transform.parent = steamVRController.transform;
            steamVRController.renderModel = controllerModel;

            var pointer = steamVRController.gameObject.AddComponent<vdtUnity.SteamVRPointer>();
            pointer.pointerMaterial = pointerMaterial;

            var grabber = steamVRController.gameObject.AddComponent<vdtUnity.SteamVRGrabber>();

            return steamVRController;
        }

        // static void DisableAllRotationCompressions(GameObject domeObject)
        // {
        //     var networkTransforms = domeObject.GetComponentsInChildren<Mirror.NetworkTransform>(true);
        //     foreach (var networkTransform in networkTransforms)
        //     {
        //         DisableRotationCompression(networkTransform);
        //     }
        // }
        //
        // static void DisableRotationCompression(Mirror.NetworkTransform networkTransform)
        // {
        //     SerializedObject so = new SerializedObject(networkTransform);
        //     SerializedProperty compressRotation = so.FindProperty("compressRotation");
        //     compressRotation.enumValueIndex = (int)Mirror.NetworkTransformBase.Compression.None;
        //     so.ApplyModifiedProperties();
        // }

        static void SetupTags()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            IEnumerable<string> tags = typeof(vdtUnity.ClusterConstants.Tags).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly).Select(fi => (string)fi.GetRawConstantValue());

            foreach (string s in tags)
            {
                bool found = false;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                    if (t.stringValue.Equals(s))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    tagsProp.InsertArrayElementAtIndex(0);
                    SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                    n.stringValue = s;
                }
            }
            tagManager.ApplyModifiedProperties();
        }
    }

}
