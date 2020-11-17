using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class InputManagerManager
{
    public enum AxisType
    {
        KeyOrMouseButton = 0,
        MouseMovement = 1,
        JoystickAxis = 2
    };

    public class InputAxis
    {
        public string name;
        public string descriptiveName;
        public string descriptiveNegativeName;
        public string negativeButton;
        public string positiveButton;
        public string altNegativeButton;
        public string altPositiveButton;

        public float gravity;
        public float dead;
        public float sensitivity;

        public bool snap = false;
        public bool invert = false;

        public AxisType type;

        public int axis;
        public int joyNum;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append("name:").Append(name).Append(", ");
            if (!string.IsNullOrEmpty(descriptiveName)) sb.Append("descriptiveName:").Append(descriptiveName).Append(", ");
            if (!string.IsNullOrEmpty(descriptiveNegativeName)) sb.Append("descriptiveNegativeName:").Append(descriptiveNegativeName).Append(", ");
            if (!string.IsNullOrEmpty(negativeButton)) sb.Append("negativeButton:").Append(negativeButton).Append(", ");
            if (!string.IsNullOrEmpty(positiveButton)) sb.Append("positiveButton:").Append(positiveButton).Append(", ");
            if (!string.IsNullOrEmpty(altNegativeButton)) sb.Append("altNegativeButton:").Append(altNegativeButton).Append(", ");
            if (!string.IsNullOrEmpty(altPositiveButton)) sb.Append("altPositiveButton:").Append(altPositiveButton).Append(", ");
            sb.Append("gravity:").Append(gravity).Append(", ");
            sb.Append("dead:").Append(dead).Append(", ");
            sb.Append("sensitivity:").Append(sensitivity).Append(", ");
            sb.Append("snap:").Append(snap).Append(", ");
            sb.Append("invert:").Append(invert).Append(", ");
            sb.Append("type:").Append(type).Append(", ");
            sb.Append("axis:").Append(axis).Append(", ");
            sb.Append("joyNum:").Append(joyNum);
            sb.Append("}");
            return sb.ToString();
        }
    }

    static InputManagerManager()
    {
        AddInputAxesMenu();
    }

    [MenuItem("Debug/AddInputAxes")]
    static void AddInputAxesMenu()
    {
        AddAxis(new InputAxis()
        {
            name = "UpDown",
            dead = 0.2f,
            sensitivity = 1f,
            type = AxisType.JoystickAxis,
            axis = 3,
            joyNum = 0,
        });
        AddAxis(new InputAxis()
        {
            name = "Yaw",
            dead = 0.2f,
            sensitivity = 1f,
            type = AxisType.JoystickAxis,
            axis = 4,
            joyNum = 0,
        });
    }

    [MenuItem("Debug/GetInputAxisInfo")]
    static void GetInputAxisInfoMenu()
    {
        SerializedObject serializedObject = LoadInputManager();
        if (serializedObject != null)
        {
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                var axis = axesProperty.Copy();
                axis.Next(true);
                Debug.Log(ToInputAxis(axis));
            }
        }
    }

    private static void AddAxis(InputAxis axis)
    {
        if (AxisDefined(axis.name)) return;

        SerializedObject serializedObject = LoadInputManager();
        if (serializedObject != null)
        {
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);
            SetAxisProperty(axisProperty, axis);

            serializedObject.ApplyModifiedProperties();
        }
    }

    private static InputAxis GetAxis(string axisName)
    {
        SerializedProperty axis = null;

        SerializedObject serializedObject = LoadInputManager();
        if (serializedObject != null)
        {
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");
            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                axis = axesProperty.Copy();
                axis.Next(true);
                if (axis.stringValue == axisName)
                {
                    return ToInputAxis(axis);
                }
            }
        }
        return null;
    }

    private static InputAxis ToInputAxis(SerializedProperty axis)
    {
        return new InputAxis()
        {
            name = GetChildProperty(axis, "m_Name")?.stringValue,
            descriptiveName = GetChildProperty(axis, "descriptiveName")?.stringValue,
            descriptiveNegativeName = GetChildProperty(axis, "descriptiveNegativeName")?.stringValue,
            negativeButton = GetChildProperty(axis, "negativeButton")?.stringValue,
            positiveButton = GetChildProperty(axis, "positiveButton")?.stringValue,
            altNegativeButton = GetChildProperty(axis, "altNegativeButton")?.stringValue,
            altPositiveButton = GetChildProperty(axis, "altPositiveButton")?.stringValue,
            gravity = GetChildProperty(axis, "gravity")?.floatValue ?? 0,
            dead = GetChildProperty(axis, "dead")?.floatValue ?? 0,
            sensitivity = GetChildProperty(axis, "sensitivity")?.floatValue ?? 0,
            snap = GetChildProperty(axis, "snap")?.boolValue ?? false,
            invert = GetChildProperty(axis, "invert")?.boolValue ?? false,
            type = (AxisType)(GetChildProperty(axis, "type")?.intValue ?? 0),
            axis = (GetChildProperty(axis, "axis")?.intValue ?? 0) + 1,
            joyNum = GetChildProperty(axis, "joyNum")?.intValue ?? 0
        };
    }

    private static void SetAxisProperty(SerializedProperty axisProperty, InputAxis axis)
    {
        GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
        GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
        GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
        GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
        GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
        GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
        GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
        GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
        GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
        GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
        GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
        GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
        GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
        GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
        GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;
    }

    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
    {
        SerializedProperty child = parent.Copy();
        child.Next(true);
        do
        {
            if (child.name == name) return child;
        }
        while (child.Next(false));
        return null;
    }

    private static SerializedObject LoadInputManager()
    {
        var inputManagerAssetPath = "ProjectSettings/InputManager.asset";
        var inputManagerAssets = AssetDatabase.LoadAllAssetsAtPath(inputManagerAssetPath);
        if ((inputManagerAssets == null) || (inputManagerAssets.Length == 0))
        {
            Debug.LogError("Unable to load InputManager from " + inputManagerAssetPath);
            return null;
        }
        return new SerializedObject(inputManagerAssets[0]);
    }

    private static bool AxisDefined(string axisName)
    {
        SerializedObject serializedObject = LoadInputManager();
        if (serializedObject != null)
        {
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                axis.Next(true);
                if (axis.stringValue == axisName) return true;
            }
        }
        return false;
    }
}
