using ConfigSystem;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Контроллер колоды.
/// </summary>
public class CardDeskController : LocalMonoSingleton<CardDeskController>
{
	[SerializeField]
	Transform _trumpCardPlace;

	[SerializeField, Tooltip("объект, отображающий колоду")]
	Transform _view;

	[SerializeField]
	Transform _cardsPlace;

	void Start()
	{
		EventAggregator.Subscribe(GameEvent.AddCardToHand, this, TryDisableCollider);
		EventAggregator.Subscribe(GameEvent.InitCards, this, OnInitCards);
	}

	public int GetCardsCount()
	{
		if (_trumpCardPlace.childCount > 0)
			return _cardsPlace.childCount + 1;
		return 0;
	}

	public Transform GetTopCard()
	{
		if (_cardsPlace.childCount > 0)
		{
			var tr = _cardsPlace.GetChild(0);
			UpdateHeight();
			return tr;
		}
		return _trumpCardPlace.GetChild(0);
	}

	public void OnMouseDown()
	{
		CardMovementManager.TryAddCardToHand(GetTopCard());
	}

	void OnInitCards()
	{
		if (GameManager.GameState == GameState.Normal)
		{
			int cardsCount = CardsCreator.Instance.transform.childCount;
			for (int i = 0; i < cardsCount; i++)
			{
				int index = Random.Range(0, CardsCreator.Instance.transform.childCount);
				var card = CardsCreator.Instance.transform.GetChild(index);
				card.parent = _cardsPlace;
			}
			CardMovementManager.MoveCardToTrumpPlace(GetTopCard(), _trumpCardPlace);
		}

		UpdateHeight();
		TryDisableCollider();
	}

	void UpdateHeight()
	{
		Utils.TransformationUtils.UpdateHeightDependingFromCount(_view, _cardsPlace.childCount, Config.Instance.CardHeightInCardDesk);
		Utils.TransformationUtils.LocateAboveSurface(_view, 0);
	}

	void TryDisableCollider()
	{
		if (GetCardsCount() <= 1)
		{
			gameObject.SetActive(false);
			if (GetCardsCount() == 1) //сделать козырь активным, если в колоде остался только козырь
				_trumpCardPlace.GetChild(0).GetComponent<Collider>().enabled = true;
		}
	}
}
