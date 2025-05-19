using System.Globalization;
using UnityEngine;

namespace ProjectMER.Features.Extensions;

public static class StructExtensions
{
	/// <summary>
	/// Gets the corresponding <see cref="Color"/> given a specified <see cref="string"/>.
	/// </summary>
	/// <param name="colorText">The specified <see cref="string"/> to convert.</param>
	/// <returns>The corresponding <see cref="Color"/>.</returns>
	public static Color GetColorFromString(this string colorText)
	{
		Color color = new(-1f, -1f, -1f);
		string[] charTab = colorText.Split(':');
		if (charTab.Length >= 4)
		{
			if (charTab[0].TryParseToFloat(out float red))
				color.r = red / 255f;

			if (charTab[1].TryParseToFloat(out float green))
				color.g = green / 255f;

			if (charTab[2].TryParseToFloat(out float blue))
				color.b = blue / 255f;

			if (charTab[3].TryParseToFloat(out float alpha))
				color.a = alpha;

			return color != new Color(-1f, -1f, -1f) ? color : Color.magenta * 3f;
		}

		if (colorText[0] != '#' && colorText.Length == 8)
			colorText = '#' + colorText;

		return ColorUtility.TryParseHtmlString(colorText, out color) ? color : Color.magenta * 3f;

	}

	public static Vector3 ToVector3(this string s)
	{
		s = s.Trim('(', ')').Replace(" ", "");
		string[] split = s.Split(',');

		float x = float.Parse(split[0], CultureInfo.InvariantCulture);
		float y = float.Parse(split[1], CultureInfo.InvariantCulture);
		float z = float.Parse(split[2], CultureInfo.InvariantCulture);

		return new Vector3(x, y, z);
	}

	public static bool TryParseToFloat(this string s, out float result) => float.TryParse(s.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result);

	public static bool TryGetVector(string x, string y, string z, out Vector3 vector)
	{
		vector = Vector3.zero;

		if (!x.TryParseToFloat(out float xValue) || !y.TryParseToFloat(out float yValue) || !z.TryParseToFloat(out float zValue))
			return false;

		vector = new Vector3(xValue, yValue, zValue);
		return true;
	}
}
