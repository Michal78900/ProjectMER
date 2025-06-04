using System.Collections;
using System.Reflection;
using System.Text;
using NorthwoodLib.Pools;
using UnityEngine;
using YamlDotNet.Serialization;

namespace ProjectMER.Features.Extensions;

public static class ReflectionExtensions
{
	public static IEnumerable<PropertyInfo> GetModifiableProperties(this Type type)
	{
		foreach (PropertyInfo property in type.GetProperties())
		{
			if (!property.CanWrite)
				continue;

			if (Attribute.IsDefined(property, typeof(YamlIgnoreAttribute)))
				continue;

			if (property.Name == "Position" || property.Name == "Rotation" || property.Name == "Scale")
				continue;

			yield return property;
		}
	}

	public static IEnumerable<string> GetColoredProperties(this List<PropertyInfo> properties, object instance)
	{
		foreach (PropertyInfo property in properties)
		{
			if (!property.CanWrite)
				continue;

			if (property.PropertyType == typeof(bool))
			{
				yield return $"{property.Name}: {((bool)property.GetValue(instance) ? "<color=green><b>TRUE</b></color>" : "<color=red><b>FALSE</b></color>")}";
			}
			else if (property.PropertyType == typeof(Vector3))
			{
				yield return $"{property.Name}: <color=yellow><b>{(Vector3)property.GetValue(instance):F3}</b></color>";
			}
			else if (property.Name.Contains("Color"))
			{
				string colorString = property.GetValue(instance).ToString();
				yield return $"{property.Name}: <color={colorString.GetColorFromString().ToHex()}><b>{colorString}</b></color>";
			}
			else if (property.Name == "Text")
			{
				StringBuilder sb = StringBuilderPool.Shared.Rent(property.GetValue(instance).ToString());
				if (sb.Length > 32)
				{
					sb.Remove(32, sb.Length - 32);
					sb.Append("...");
				}
				yield return $"{property.Name}: <noparse>{StringBuilderPool.Shared.ToStringReturn(sb)}</noparse>";
			}
			else if (typeof(ICollection).IsAssignableFrom(property.PropertyType))
			{
				StringBuilder sb = StringBuilderPool.Shared.Rent();
				ICollection collection = (ICollection)property.GetValue(instance);
				if (collection.GetType().GetGenericArguments()[0].IsEnum)
				{
					foreach (object? item in collection)
						sb.Append($"{item} ");

					if (sb.Length > 0)
						sb.Remove(sb.Length - 1, 1);

					sb.Insert(0, "<color=yellow><b>");
					sb.Append("</b></color>");
				}
				else
				{
					foreach (object? item in collection)
						sb.Append($"{MapUtils.GetColoredString(item.ToString())} ");

					if (sb.Length > 0)
						sb.Remove(sb.Length - 1, 1);
				}

				yield return $"{property.Name}: {StringBuilderPool.Shared.ToStringReturn(sb)}";
			}
			else
			{
				yield return $"{property.Name}: <color=yellow><b>{property.GetValue(instance) ?? "NULL"}</b></color>";
			}
		}
	}
}
