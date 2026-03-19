using UnityEngine;

namespace StepanoffGames.DiceRush.Math
{
	public class BezierUtils
	{
		public static void TransformPoints(Vector3[] pathPoints, Vector3 fromPos, Vector3 toPos)
		{
			if (pathPoints == null || pathPoints.Length < 2)
			{
				Debug.LogWarning("Path must contain at least 2 points.");
				return;
			}

			Vector3 originalStart = pathPoints[0];
			Vector3 originalEnd = pathPoints[pathPoints.Length - 1];

			Vector3 originalVector = originalEnd - originalStart;
			Vector3 targetVector = toPos - fromPos;

			float originalLength = originalVector.magnitude;
			float targetLength = targetVector.magnitude;

			if (originalLength < 0.0001f)
			{
				Debug.LogWarning("Original path is too short.");
				return;
			}

			Quaternion rotation = Quaternion.identity;

			if (targetLength >= 0.0001f)
			{
				rotation = Quaternion.FromToRotation(originalVector, targetVector);
			}

			float scale = targetLength / originalLength;

			for (int i = 0; i < pathPoints.Length; i++)
			{
				Vector3 localOffset = pathPoints[i] - originalStart;
				Vector3 transformedOffset = rotation * (localOffset * scale);
				pathPoints[i] = fromPos + transformedOffset;
			}

			// Чтобы гарантированно убрать возможную погрешность
			pathPoints[0] = fromPos;
			pathPoints[pathPoints.Length - 1] = toPos;
		}
	}
}
