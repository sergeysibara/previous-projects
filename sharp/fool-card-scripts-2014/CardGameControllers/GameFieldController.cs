using UnityEngine;

public class GameFieldController : LocalMonoSingleton<GameFieldController>
{
	void Start()
	{

	}

	public bool TryAddCard(Transform card)
	{
		foreach (Transform tr in transform)
		{
			if (tr.childCount == 0)
			{
				card.parent = tr;
				card.position = tr.position;
				card.rotation = tr.rotation;

				card.GetComponent<CardController>().State = CardState.InGameField;
				return true;
			}
		}
		return false;
	}

	void OnMouseDown()
	{
		CardMovementManager.TryMoveCardToGameField();
	}
}
