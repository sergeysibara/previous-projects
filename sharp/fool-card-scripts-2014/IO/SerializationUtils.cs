using System.IO;
using Pathfinding.Serialization.JsonFx;

/// <summary>
/// Input Output file format strategy
/// </summary>
public interface IIOStrategy
{
	bool TryLoad<T>(string path, string fileName, out T retData);
	bool HasSaveFile(string path, string fileName);
	void Save<T>(string path, string fileName, T rootObj);
	string SerializeToString<T>(T value);
}

public class JsonSerializationUtils : IIOStrategy
{
	public bool TryLoad<T>(string path, string fileName, out T retData)
	{
		var fullpath = path + fileName + ".txt";
		if (!File.Exists(fullpath))
		{
			UnityEngine.Debug.LogWarning("path not found!");
			retData = default(T);
			return false;
		}
		var streamReader = new StreamReader(fullpath);
		string data = streamReader.ReadToEnd();
		streamReader.Close();

		retData = JsonReader.Deserialize<T>(data);
		return true;
	}

	public bool HasSaveFile(string path, string fileName)
	{
		var fullpath = path + fileName + ".txt";
		return File.Exists(fullpath);
	}

	public void Save<T>(string path, string fileName, T rootObj)
	{
		string data = JsonWriter.Serialize(rootObj);
		if (!Directory.Exists(path))
			Directory.CreateDirectory(path);

		var streamWriter = new StreamWriter(path + fileName + ".txt");
		streamWriter.Write(data);
		streamWriter.Close();
	}

	public string SerializeToString<T>(T value)
	{
		return JsonWriter.Serialize(value).Replace(",", ",\n").Replace("{", "{\n");
	}
}