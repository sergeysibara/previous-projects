using SaveSystem;
using UnityEngine;

public class SaveManager : LocalMonoSingleton<SaveManager>
{
	void Start()
	{
		if (GameManager.GameState == GameState.Loading)
		{
			GameDataState.Load();
			EventAggregator.Publish(GameEvent.EndLoadGameData, this, publichToInactive: true);
		}
		GameManager.GameState = GameState.Normal;
	}

	public void Save()
	{
		GameDataState.Clear();
		EventAggregator.Publish(GameEvent.StartSaveGame, this, publichToInactive: true);
		GameDataState.Save();
	}

	public void Load()
	{
		if (GameDataState.HasSaveFile())
		{
			Application.LoadLevel(0);
			GameManager.GameState = GameState.Loading;
		}
		else
			Debug.LogWarning("Can not load file");
	}
}
