using UnityEngine;
using System;

public abstract class BaseDataManager<T> : GlobalMonoSingleton<BaseDataManager<T>>
{
	[SerializeField]
	protected T _root;
	public static T Root { get { return Instance._root; } }

	protected static readonly Type _serializedType = typeof(T);

	protected static string _path;
	protected abstract string GetPath { get; }


	protected abstract string Filename { get; }

	protected IIOStrategy _ioStrategy;

	protected override void Awake()
	{
		base.Awake();
		_ioStrategy = GetIOStrategy();
	}

	public static bool Load()
	{
		T newRoot;
		var isSuccess = Instance._ioStrategy.TryLoad<T>(Instance.GetPath, Instance.Filename, out newRoot);
		if (isSuccess)
			Instance._root = newRoot;
		return isSuccess;
	}

	public static bool HasSaveFile()
	{
		return Instance._ioStrategy.HasSaveFile(Instance.GetPath, Instance.Filename);
	}

	[ContextMenu("Save")]
	protected void SaveToFile()
	{
		Debug.Log(string.Format("save to {0}", GetPath + Filename));
		GetIOStrategy().Save<T>(GetPath, Filename, _root);
	}

	[ContextMenu("Load")]
	protected void LoadFromInspector()
	{
		T newRoot;
		var isSuccess = GetIOStrategy().TryLoad<T>(GetPath, Filename, out newRoot);
		if (isSuccess)
			_root = newRoot;
	}

	protected IIOStrategy GetIOStrategy()
	{
		return new JsonSerializationUtils();
	}

	public static string ContentToString()
	{
		return Instance._ioStrategy.SerializeToString(Root);
	}
}