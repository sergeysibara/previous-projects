using UnityEngine;

public class CardHandController : LocalMonoSingleton<CardHandController>
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
				card.gameObject.SetActive(true);
				card.parent = tr;
				card.position = tr.position;
				card.rotation = tr.rotation;

				card.GetComponent<CardController>().State = CardState.InHand;
				EventAggregator.Publish(GameEvent.AddCardToHand, this);
				return true;
			}
		}
		return false;
	}

}
