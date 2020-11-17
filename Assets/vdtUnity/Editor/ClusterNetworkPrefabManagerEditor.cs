using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Linq;

namespace vdtUnity
{
    [CustomEditor(typeof(ClusterNetworkPrefabManager))]
    public class ClusterNetworkPrefabManagerEditor : Editor
    {
        private class DragDropListAdaptor : SerializedPropertyAdaptor, IReorderableListDropTarget
        {
            public DragDropListAdaptor(SerializedProperty serializedProperty) : base(serializedProperty)
            {
            }

            public bool CanDropInsert(int insertionIndex)
            {
                if (!ReorderableListControl.CurrentListPosition.Contains(Event.current.mousePosition))
                    return false;

                return DragAndDrop.objectReferences.All(o => o is GameObject);
            }

            public void ProcessDropInsertion(int insertionIndex)
            {
                if (Event.current.type == EventType.DragPerform)
                {
                    var draggedObjects = DragAndDrop.objectReferences;

                    foreach (var draggedObject in draggedObjects)
                    {
                        GameObject obj = draggedObject as GameObject;
                        if (obj)
                        {
                            Insert(insertionIndex);
                            this[insertionIndex].objectReferenceValue = obj;
                            insertionIndex++;
                        }
                    }
                }
            }
        }

        SerializedProperty networkPrefabs;

        private void OnEnable()
        {
            networkPrefabs = serializedObject.FindProperty("networkPrefabs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DragDropListAdaptor listAdaptor = new DragDropListAdaptor(networkPrefabs);
            ReorderableListGUI.Title("Network Prefabs");
            ReorderableListGUI.ListField(listAdaptor);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

