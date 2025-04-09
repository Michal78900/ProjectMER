using System.Collections;
using System.Reflection;
using System.Text;
using LabApi.Features.Console;
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
			else if (typeof(ICollection).IsAssignableFrom(property.PropertyType))
			{
				StringBuilder sb = StringBuilderPool.Shared.Rent();
				foreach (object? item in (ICollection)property.GetValue(instance))
					sb.Append($"{item}, ");

				if (sb.Length > 0)
					sb.Remove(sb.Length - 2, 2);

				yield return $"{property.Name}: <color=yellow><b>{StringBuilderPool.Shared.ToStringReturn(sb)}</b></color>";
			}
			else
			{
				yield return $"{property.Name}: <color=yellow><b>{property.GetValue(instance) ?? "NULL"}</b></color>";
			}
		}
	}
}
