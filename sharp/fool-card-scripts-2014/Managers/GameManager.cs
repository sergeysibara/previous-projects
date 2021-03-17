using UnityEngine;
using System;
using System.Collections;

public static class GameManager
{
	public static GameState GameState;
}

public enum GameState
{
	Normal,
	Loading,
}
