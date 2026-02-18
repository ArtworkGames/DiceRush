using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MyUtils
{
	public static string GetUnits(int num, string one, string two, string five)
	{
		string units = five;
		if (((num % 100) <= 10) || ((num % 100) >= 20))
		{
			int end = num % 10;
			if (end == 1) units = one;
			else if ((end == 2) || (end == 3) || (end == 4)) units = two;
		}
		return units;
	}

	public static float RoundTo(float val, int digits)
	{
		float dec = Mathf.Pow(10.0f, digits);
		val = Mathf.Round(val * dec) / dec;
		return val;
	}

	public static Vector2 RoundTo(Vector2 val, int digits)
	{
		val = new Vector2(RoundTo(val.x, digits), RoundTo(val.y, digits));
		return val;
	}

	public static Vector3 RoundTo(Vector3 val, int digits)
	{
		val = new Vector3(RoundTo(val.x, digits), RoundTo(val.y, digits), RoundTo(val.z, digits));
		return val;
	}

	public static Quaternion RoundTo(Quaternion val, int digits)
	{
		val = new Quaternion(RoundTo(val.x, digits), RoundTo(val.y, digits), RoundTo(val.z, digits), RoundTo(val.w, digits));
		return val;
	}

	public static string ColorToHex(Color color)
	{
		string hex = ((int)Mathf.Floor(color.r * 255.0f)).ToString("X2") +
			((int)Mathf.Floor(color.g * 255.0f)).ToString("X2") +
				((int)Mathf.Floor(color.b * 255.0f)).ToString("X2");
		return hex;
	}

	public static string ColorToHex(Color32 color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}

	public static Color HexToColor(string hex)
	{
		int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
		Color color = new Color((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f, 1.0f);
		return color;
	}

	public static Color32 HexToColor32(string hex)
	{
		int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
		Color32 color = new Color32((byte)r, (byte)g, (byte)b, 255);
		return color;
	}

	public static bool IsPointerOverUIElement(Vector3 touchPos)
	{
		if (EventSystem.current.IsPointerOverGameObject()) return true;

		PointerEventData eventData = new PointerEventData(EventSystem.current);
		eventData.position = touchPos;
		List<RaycastResult> raysastResults = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, raysastResults);

		for (int index = 0; index < raysastResults.Count; index++)
		{
			RaycastResult curRaysastResult = raysastResults[index];
			if ((curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI")) ||
				(curRaysastResult.gameObject.layer == LayerMask.NameToLayer("Windows")))
				return true;
		}
		return false;
	}
}
