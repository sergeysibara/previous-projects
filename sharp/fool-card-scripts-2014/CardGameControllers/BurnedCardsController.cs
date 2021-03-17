using System.Collections.Generic;
using SaveSystem;
using UnityEngine;
using ConfigSystem;

public class BurnedCardsController : LocalMonoSingleton<BurnedCardsController>
{
	[SerializeField]
	Transform _view;

	int _burnedCardsCount;

	void Start()
	{
		_view.gameObject.SetActive(false);
		EventAggregator.Subscribe(GameEvent.InitCards, this, OnInitCards);
		EventAggregator.Subscribe(GameEvent.StartSaveGame, this, OnSaveGame);
	}

	public void AddCards(IEnumerable<Transform> cards)
	{
		foreach (var card in cards)
		{
			_burnedCardsCount++;
			DestroyObject(card.gameObject);
		}

		_view.gameObject.SetActive(_burnedCardsCount != 0);
		UpdateHeight();
	}

	public void OnMouseDown()
	{
		CardMovementManager.TryMoveToBurnedCards();
	}

	void OnInitCards()
	{
		_burnedCardsCount = GameDataState.Root.BurnedCardsCount;
		_view.gameObject.SetActive(_burnedCardsCount != 0);
		UpdateHeight();
	}

	void OnSaveGame()
	{
		GameDataState.Root.BurnedCardsCount = _burnedCardsCount;
	}

	void UpdateHeight()
	{
		_view.parent = null; //чтобы использовался глобальный scale, а не локальный
		Utils.TransformationUtils.UpdateHeightDependingFromCount(_view, _burnedCardsCount, Config.Instance.CardHeightInCardDesk);
		_view.parent = transform;
		Utils.TransformationUtils.LocateAboveSurface(_view, 0);
	}
}
