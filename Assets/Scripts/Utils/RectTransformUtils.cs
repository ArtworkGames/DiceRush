using UnityEngine;

public static class RectTransformUtils
{
	public static void SetDefaultScale(this RectTransform trans)
	{
		trans.localScale = Vector3.one;
	}

	public static void SetPivotAndAnchors(this RectTransform trans, Vector2 vec)
	{
		trans.pivot = vec;
		trans.anchorMin = vec;
		trans.anchorMax = vec;
	}

	public static Vector2 GetSize(this RectTransform trans)
	{
		return trans.rect.size;
	}

	public static float GetWidth(this RectTransform trans)
	{
		return trans.rect.width;
	}

	public static float GetHeight(this RectTransform trans)
	{
		return trans.rect.height;
	}

	public static void SetPivotPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
	}

	public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
	}

	public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1.0f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
	}

	public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x - ((1.0f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
	}

	public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
	{
		trans.localPosition = new Vector3(newPos.x - ((1.0f - trans.pivot.x) * trans.rect.width), newPos.y - ((1.0f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
	}

	public static void SetSize(this RectTransform trans, Vector2 newSize)
	{
		Vector2 oldSize = trans.rect.size;
		Vector2 deltaSize = newSize - oldSize;
		trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
		trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1.0f - trans.pivot.x), deltaSize.y * (1.0f - trans.pivot.y));
	}

	public static void SetWidth(this RectTransform trans, float newSize)
	{
		SetSize(trans, new Vector2(newSize, trans.rect.size.y));
	}

	public static void SetHeight(this RectTransform trans, float newSize)
	{
		SetSize(trans, new Vector2(trans.rect.size.x, newSize));
	}

	public static Rect GetScreenRect(this RectTransform rectTransform, Canvas canvas)
	{
		Vector3[] corners = new Vector3[4];
		Vector3[] screenCorners = new Vector3[2];

		rectTransform.GetWorldCorners(corners);

		if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
		{
			screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
			screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
		}
		else
		{
			screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
			screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
		}

		screenCorners[0].y = Screen.height - screenCorners[0].y;
		screenCorners[1].y = Screen.height - screenCorners[1].y;

		return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
	}
}
