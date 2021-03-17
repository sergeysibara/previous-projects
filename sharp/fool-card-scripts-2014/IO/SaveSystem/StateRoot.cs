using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using UnityEngine;

namespace SaveSystem
{
	[Serializable]
	public class StateRoot
	{
		public List<Card> Cards;
		public int BurnedCardsCount; //не сохранять напрямую, т.к. 2 разных логики работы получаются. Либо сделать 2 очерних класса: currentState, Saved  
	}

	[Serializable]
	public class Card
	{
		public bool IsActive;
		public bool ColliderIsEnabled;
		public int TextureID;
		public CardState State;
		public string ParentTransformPath;

		public void Fill(GameObject gameObject, CardState state, Texture tex)
		{
			IsActive = gameObject.activeSelf;
			ColliderIsEnabled = gameObject.GetComponent<Collider>().enabled;
			TextureID = tex.GetInstanceID();
			State = state;
			ParentTransformPath = Utils.TransformUtils.GetFullPath(gameObject.transform.parent);
		}
	}
}