using System.Collections.Generic;
using System.Linq;
using SaveSystem;
using UnityEngine;

public class CardsCreator : LocalMonoSingleton<CardsCreator>
{
	[SerializeField]
	Transform _cardPrefab;

	[SerializeField]
	public List<Texture> _cardTextures;

	void Start()
	{
		if (GameManager.GameState != GameState.Loading)
		{
			foreach (var tex in _cardTextures)
			{
				var card = Instantiate(_cardPrefab);
				card.GetComponent<CardController>().GetFrontMaterial().mainTexture = tex;
				card.parent = transform;
			}
			EventAggregator.Publish(GameEvent.InitCards, this, publichToInactive: true);
		}

		EventAggregator.Subscribe(GameEvent.EndLoadGameData, this, OnGameLoaded);
	}

	void OnGameLoaded()
	{
		foreach (var tex in _cardTextures)
		{
			var cardData = GameDataState.Root.Cards.FirstOrDefault(c => c.TextureID == tex.GetInstanceID());
			if (cardData != null)
			{
				var card = Instantiate(_cardPrefab);
				var cardController = card.GetComponent<CardController>();
				cardController.GetFrontMaterial().mainTexture = tex;
				card.parent = transform;
				cardController.Load();
			}			
		}

		EventAggregator.Publish(GameEvent.InitCards, this, publichToInactive: true);
	}
}
