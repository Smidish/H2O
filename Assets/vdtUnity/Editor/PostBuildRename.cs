using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class PostBuildRename
{
    public static string DefaultName = "vdtUnityDemo";

    //[PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        FileInfo fileInfo = new FileInfo(pathToBuiltProject);
        DirectoryInfo dirInfo = fileInfo.Directory;

        string oldDataPath = dirInfo.FullName + "/" + dirInfo.Name + "_Data";
        string newDataPath = dirInfo.FullName + "/" + DefaultName + "_Data";

        string oldExecutablePath = pathToBuiltProject;
        string newExecutablePath = dirInfo.FullName + "/" + DefaultName + ".exe";

        if (!Directory.Exists(oldDataPath))
        {
            Debug.LogError("Data diredctory does not exist");
        }
        if (!File.Exists(oldExecutablePath))
        {
            Debug.LogError("Executable does not exist");
        }
        try
        {
            if (Directory.Exists(newDataPath))
            {
                Directory.Delete(newDataPath, true);
            }
            Directory.Move(oldDataPath, newDataPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
        try
        {
            File.Delete(newExecutablePath);
            Directory.Move(oldExecutablePath, newExecutablePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        Debug.Log(pathToBuiltProject + " - " + target.ToString());
    }
}
