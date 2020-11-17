using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Globalization;
using System.Threading;
using System;

public class DebugCalibrationDataBuilder : EditorWindow
{
    [MenuItem("vdtUnity/Debug/Create Debug CalibrationData")]
    static void MenuCreateDomeObject()
    {
        //VDTUnityBuilderWindow window = EditorWindow.GetWindow<VDTUnityBuilderWindow>();
        //window.Show();
        string calibrationDataPath = EditorUtility.OpenFolderPanel("Pick CalibrationData Folder", "D:\\Data\\DebugCalibrationData", "");
        if (Directory.Exists(calibrationDataPath) == false)
        {
            Debug.LogError("CalibrationDataPath does not exist");
            return;
        }

        int num_displays_x = 2;
        int num_displays_z = 2;

        float offset_z = 0;// 1700;
        float offset_y = -1000;

        float display_width = 800f;
        float display_height = 600f;
        for (int iz = 0; iz < num_displays_z; ++iz)
        {
            for (int ix = 0; ix < num_displays_x; ++ix)
            {
                float x = (ix - num_displays_x * 0.5f + 0.5f) * display_width;
                float y = offset_y;
                float z = (iz - num_displays_z * 0.5f + 0.5f) * display_height + offset_z;

                int i = iz * num_displays_z + ix + 1;

                TargetData targetData = new TargetData(x, y, z, display_width, display_height);
                string targetFilename = calibrationDataPath + "\\target_" + i + ".csv";
                WriteCSVFile(targetFilename, new TargetData[] { targetData });

                string shapeFilename = calibrationDataPath + "\\shape_" + i + ".csv";
                WriteCSVFile(shapeFilename, CreateShape(targetData, 30, 20));
            }
        }
    }

    class TargetData
    {
        public float x;
        public float y;
        public float z;
        public float heading;
        public float pitch;
        public float bank;
        public float left;
        public float right;
        public float bottom;
        public float top;
        public float width;
        public float height;
        public float normalx;
        public float normaly;
        public float normalz;
        public float c0x;
        public float c0y;
        public float c0z;
        public float c1x;
        public float c1y;
        public float c1z;
        public float c2x;
        public float c2y;
        public float c2z;
        public float c3x;
        public float c3y;
        public float c3z;

        public TargetData(float x, float y, float z, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.heading = 180;
            this.pitch = 0;
            this.bank = 0;
            this.left = -width / 2;
            this.right = width / 2;
            this.bottom = -height / 2;
            this.top = height / 2;
            this.width = width;
            this.height = height;
            this.normalx = 0;
            this.normaly = -1;
            this.normalz = 0;
            this.c0x = x - left;
            this.c0y = y;
            this.c0z = z + top;
            this.c1x = x - right;
            this.c1y = y;
            this.c1z = z + top;
            this.c2x = x - right;
            this.c2y = y;
            this.c2z = z + bottom;
            this.c3x = x - left;
            this.c3y = y;
            this.c3z = z + bottom;
        }
    }

    class ShapeVertex
    {
        public float x;
        public float y;
        public float z;
        public float u;
        public float v;
        public float column;
        public float row;
    }

    static IEnumerable<ShapeVertex> CreateShape(TargetData data, int columns, int rows)
    {
        List<ShapeVertex> shapeData = new List<ShapeVertex>();
        Vector3 c0 = new Vector3(data.c0x, data.c0y, data.c0z);
        Vector3 c1 = new Vector3(data.c1x, data.c1y, data.c1z);
        Vector3 c3 = new Vector3(data.c3x, data.c3y, data.c3z);
        for (int row = 0; row < rows; ++row)
        {
            float v = (float)row / (float)(rows - 1);
            for (int col = 0; col < columns; ++col)
            {
                float u = (float)col / (float)(columns - 1);
                Vector3 p = c0 + (c1 - c0) * u + (c3 - c0) * v;
                ShapeVertex vertex = new ShapeVertex
                {
                    x = p.x,
                    y = p.y,
                    z = p.z,
                    u = u,
                    v = v,
                    column = col,
                    row = row
                };
                shapeData.Add(vertex);
            }
        }
        return shapeData;
    }

    static void WriteCSVFile<T>(string filename, IEnumerable<T> data)
    {
        var currentCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        try
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                FieldInfo[] fields = typeof(T).GetFields();
                writer.WriteLine(string.Join(";", fields.Select(f => f.Name)));
                foreach (T line in data)
                {
                    writer.WriteLine(string.Join(";", fields.Select(f => f.GetValue(line))));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
    }

    void OnGUI()
    {
    }
}
