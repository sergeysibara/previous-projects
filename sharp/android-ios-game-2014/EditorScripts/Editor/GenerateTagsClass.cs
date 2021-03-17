using System.IO;
using System.Text;
using UnityEditor;

public static class GenerateTagsClass
{
    private const string path = "Assets/Scripts/Consts/";
    private const string fileName = "Tags.cs";

    private static void TryCreatedPath()
    {
        if (Directory.Exists(path))
            return;
        Directory.CreateDirectory(path);
    }

    [MenuItem("Tools/CodeGen/Generate Tags Class")]
    public static void GenerateTags()
    {
        TryCreatedPath();

        var fileStream = new FileStream(path + fileName, FileMode.Create);
        var writer = new StreamWriter(fileStream, Encoding.UTF8);

        writer.WriteLine("namespace Consts\n{");
        writer.WriteLine("\tpublic static class Tags\n\t{");

        foreach (var tag in UnityEditorInternal.InternalEditorUtility.tags)
        {
            writer.WriteLine(string.Format("\t\tpublic const string {0} = \"{1}\";", tag.Replace(" ", ""), tag));
        }

        writer.WriteLine("\t}");
        writer.WriteLine("}");

        writer.Close();
        fileStream.Close();

        AssetDatabase.Refresh();
    }
}