using System.IO;
using System.Text;
using UnityEditor;
using System.Text.RegularExpressions;

public static class GenerateLayersClass
{
    const string path = "Assets/Scripts/Consts/";
    const string fileName = "Layers.cs";

    private static void TryCreatedPath()
    {
        if (Directory.Exists(path))
            return;
        Directory.CreateDirectory(path);
    }

    [MenuItem("Tools/CodeGen/Generate Layers Class")]
    public static void GenerateLayers()
    {
        TryCreatedPath();
        var fileStream = new FileStream(path+fileName, FileMode.Create);
        var writer = new StreamWriter(fileStream, Encoding.UTF8);

        writer.WriteLine("namespace Consts \n{");
        writer.WriteLine("\tpublic static class Layers\n\t{");
        for (int i = 0; i < 32; i++)
        {
            string name = UnityEditorInternal.InternalEditorUtility.GetLayerName(i);
            if (!string.IsNullOrEmpty(name))
            {
                if (Regex.IsMatch(name, "^[0-9]."))
                {
                    name = "_" + name;
                }
                writer.WriteLine(string.Format("\t\tpublic const int {0} = {1};", name.Replace(" ", ""), i.ToString()));
            }
        }
        writer.WriteLine("\t}");
        writer.WriteLine("}");

        writer.Close();
        fileStream.Close();
       
        AssetDatabase.Refresh();
    }
}