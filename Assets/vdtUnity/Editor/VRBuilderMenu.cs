using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace vdtUnityVR
{
    public class VRBuilderMenu
    {
        [MenuItem("vdtUnity/VR/Create VR Manager", priority = 0)]
        static void MenuCreateVRManagerObject()
        {
            var vrTransformHandlePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/vdtUnity/Prefabs/TransformHandles/VRTransformHandle.prefab");
            var vrTransformHandle = PrefabUtility.InstantiatePrefab(vrTransformHandlePrefab) as GameObject;

            var vrControllerMenuPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/vdtUnity/Prefabs/ControllerMenus/VRControllerMenu.prefab");
            var vrControllerMenu = PrefabUtility.InstantiatePrefab(vrControllerMenuPrefab) as GameObject;

            var drawingMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/vdtUnity/Materials/VRLineMaterial.mat");
            
            var vrManagerObject = VRBuilder.CreateVRManagerObject(vrTransformHandle, vrControllerMenu, drawingMaterial);

            Undo.RegisterCreatedObjectUndo(vrManagerObject, "Create VR Manager");
        }
    }
}

