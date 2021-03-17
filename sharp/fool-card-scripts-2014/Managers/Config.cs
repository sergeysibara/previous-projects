using UnityEngine;

namespace ConfigSystem
{
	public class Config : GlobalMonoSingleton<Config>
	{
		/// <summary>
		/// Высота карты в колоде и в бите
		/// </summary>
		public int CardHeightInCardDesk = 5;

		/// <summary>
		/// Высота, на которую поднимается карта при выделении
		/// </summary>
		public float SelectedCardHeight = 0.2f;
	}
}