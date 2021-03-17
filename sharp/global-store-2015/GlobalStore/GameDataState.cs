using UnityEngine;

namespace SaveSystem
{
	public class GameDataState : BaseDataManager<StateRoot>
	{
		protected override string Filename
		{
			get { return "Save"; }
		}

		public static void Save()
		{
			var instance = ((GameDataState)Instance);
			Debug.Log(string.Format("save to {0}", instance.GetPath + instance.Filename));
			instance.GetIOStrategy().Save(instance.GetPath, instance.Filename, instance._root);
		}

		public static void Clear()
		{
			Root.BurnedCardsCount = 0;
			Root.Cards.Clear();
		}

		protected override string GetPath
		{
			get
			{
				if (string.IsNullOrEmpty(_path))
					_path = Application.persistentDataPath + "/";
				return _path;
			}
		}
	}
}