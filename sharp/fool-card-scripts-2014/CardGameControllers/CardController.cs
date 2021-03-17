using System.Linq;
using ConfigSystem;
using SaveSystem;
using UnityEngine;

public class CardController : MonoBehaviour
{
	public CardState State = CardState.InCardDeck;

	float _localStartHeight;
	bool _mouseOver;

	void Awake()
	{
		_localStartHeight = transform.GetChild(0).localPosition.y;
		if (State == CardState.InCardDeck)
			gameObject.SetActive(false);

		EventAggregator.Subscribe(GameEvent.StartSaveGame, this, Save);
	}

	void OnMouseEnter()
	{
		_mouseOver = true;
		if (State.In(CardState.InHand, CardState.InGameField))
			transform.GetChild(0).SetLocalY(_localStartHeight + Config.Instance.SelectedCardHeight);
	}

	void OnMouseExit()
	{
		_mouseOver = false;
		if (State.In(CardState.InHand, CardState.InGameField) && !CardMovementManager.SelectedCards.Contains(transform))
			transform.GetChild(0).SetLocalY(_localStartHeight);
	}

	void OnMouseDown()
	{
		if (State.In(CardState.InTrump))
		{
			CardMovementManager.TryAddTrumpCardToHand();
			return;
		}

		if (CardMovementManager.SelectedCards.Count == 0)
		{
			CardMovementManager.SelectedCards.Add(transform);
			VisualSelect();
			return;
		}

		var selectedCard = CardMovementManager.SelectedCards.First().GetComponent<CardController>();
		if (selectedCard.State != State)
		{
			CardMovementManager.ClearSelectedCards();
			return;
		}

		switch (selectedCard.State)
		{
			case CardState.InHand:
				CardMovementManager.Swap(selectedCard.transform, transform);
				break;

			case CardState.InGameField:
				VisualSelect();
				CardMovementManager.SelectedCards.Add(transform);
				break;
		}
	}

	void OnDestroy()
	{
		EventAggregator.UnSubscribe(GameEvent.StartSaveGame, this);
	}

	public Material GetFrontMaterial()
	{
		return transform.GetChild(0).GetComponent<Renderer>().materials[0];
	}

	public void VisualUnselect()
	{
		if (!_mouseOver)
			transform.GetChild(0).SetLocalY(_localStartHeight);
	}

	void VisualSelect()
	{
		transform.GetChild(0).SetLocalY(_localStartHeight + Config.Instance.SelectedCardHeight);
	}

	void Save()
	{
		var tex = GetFrontMaterial().mainTexture;
		Card card = GameDataState.Root.Cards.FirstOrDefault(c => c.TextureID == tex.GetInstanceID());
		if (card == null)
		{
			card = new Card();
			card.Fill(gameObject, State, tex);
			GameDataState.Root.Cards.Add(card);
			return;
		}

		card.Fill(gameObject, State, tex);
	}

	public void Load()
	{
		var tex = GetFrontMaterial().mainTexture;
		var card = GameDataState.Root.Cards.First(c => c.TextureID == tex.GetInstanceID());
		State = card.State;

		transform.parent = null;
		transform.localScale = new Vector3(1, 1, 1);

		var parentTr = GameObject.Find(card.ParentTransformPath).transform;
		transform.parent = parentTr;

		gameObject.SetActive(card.IsActive);
		GetComponent<Collider>().enabled = card.ColliderIsEnabled;
		transform.position = parentTr.position;
		transform.rotation = parentTr.rotation;
	}
}
