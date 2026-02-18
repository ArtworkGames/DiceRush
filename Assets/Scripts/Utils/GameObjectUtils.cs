using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectUtils
{
	public static void ChangeLayersRecursively(this GameObject gameObject, string name)
	{
		gameObject.layer = LayerMask.NameToLayer(name);
		foreach (Transform child in gameObject.transform)
		{
			child.gameObject.ChangeLayersRecursively(name);
		}
	}
}
