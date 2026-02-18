using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FloatRange
{
	public float Min;
	public float Max;

	public FloatRange(float min, float max)
	{
		Min = min;
		Max = max;
	}

	public float GetRandom()
	{
		return UnityEngine.Random.Range(Min, Max);
	}
}
