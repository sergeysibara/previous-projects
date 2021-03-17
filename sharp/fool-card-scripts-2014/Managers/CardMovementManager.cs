using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Проверяет возможность перемещения карт и перемещает их
/// </summary>
public static class CardMovementManager
{
	public static HashSet<Transform> SelectedCards = new HashSet<Transform>();

	public static void MoveCardToTrumpPlace(Transform trumpCard, Transform trumpCardPlace)
	{
		trumpCard.parent = trumpCardPlace;
		trumpCard.position = trumpCardPlace.position;
		trumpCard.rotation = trumpCardPlace.rotation;
		trumpCard.GetComponent<CardController>().State = CardState.InTrump;
		trumpCard.gameObject.SetActive(true);
		trumpCard.GetComponent<Collider>().enabled = false;
	}

	public static void TryAddTrumpCardToHand()
	{
		ClearSelectedCards();
		if (CardDeskController.Instance.GetCardsCount() == 1)
			TryAddCardToHand(CardDeskController.Instance.GetTopCard());
	}

	public static void TryAddCardToHand(Transform card)
	{
		ClearSelectedCards();
		CardHandController.Instance.TryAddCard(card);
	}

	public static void TryMoveCardToGameField()
	{
		if (SelectedCards.Count > 0)
		{
			var card = SelectedCards.First().GetComponent<CardController>();
			if (card.State == CardState.InHand)
			{
				GameFieldController.Instance.TryAddCard(card.transform);
			}
		}
		ClearSelectedCards();
	}

	public static void TryMoveToBurnedCards()
	{
		if (SelectedCards.Count > 0)
		{
			var card = SelectedCards.First().GetComponent<CardController>();
			if (card.State == CardState.InGameField)
			{
				BurnedCardsController.Instance.AddCards(SelectedCards);
			}
		}
		ClearSelectedCards();
	}

	public static void Swap(Transform card1, Transform card2)
	{
		ClearSelectedCards();

		var tmpPos = card1.position;
		var tmpRot = card1.rotation;
		var tmpParent = card1.parent;

		card1.position = card2.position;
		card1.rotation = card2.rotation;
		card1.parent = card2.parent;

		card2.position = tmpPos;
		card2.rotation = tmpRot;
		card2.parent = tmpParent;
	}

	public static void ClearSelectedCards()
	{
		foreach (var card in SelectedCards)
			card.GetComponent<CardController>().VisualUnselect();

		SelectedCards.Clear();
	}
}
