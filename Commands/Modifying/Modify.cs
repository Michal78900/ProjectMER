using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using ProjectMER.Features;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using ProjectMER.Features.ToolGun;
using Utils.NonAllocLINQ;

namespace ProjectMER.Commands.Modifying;

/// <summary>
/// Command used for modifying the objects.
/// </summary>
public class Modify : ICommand
{
	/// <inheritdoc/>
	public string Command => "modify";

	/// <inheritdoc/>
	public string[] Aliases { get; } = ["mod"];

	/// <inheritdoc/>
	public string Description => "Allows modifying properties of the selected object.";

	/// <inheritdoc/>
	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission($"mpr.{Command}"))
		{
			response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
			return false;
		}

		Player? player = Player.Get(sender);
		if (player is null)
		{
			response = "This command can't be run from the server console.";
			return false;
		}

		if (!ToolGunHandler.TryGetSelectedMapObject(player, out MapEditorObject mapEditorObject))
		{
			response = "You haven't selected any object!";
			return false;
		}

		object instance = mapEditorObject.GetType().GetField("Base").GetValue(mapEditorObject);
		List<PropertyInfo> properties = instance.GetType().GetModifiableProperties().ToList();

		if (arguments.Count == 0)
		{
			StringBuilder sb = StringBuilderPool.Shared.Rent();
			sb.AppendLine();
			sb.Append("Object properties:");
			sb.AppendLine();
			sb.AppendLine();
			sb.Append($"MapName: {MapUtils.GetColoredMapName(mapEditorObject.MapName)}");
			sb.AppendLine();
			sb.Append($"ID: {MapUtils.GetColoredString(mapEditorObject.Id)}");
			sb.AppendLine();
			foreach (string property in properties.GetColoredProperties(instance))
			{
				sb.Append(property);
				sb.AppendLine();
			}

			response = StringBuilderPool.Shared.ToStringReturn(sb);
			return true;
		}

		string propertyName = arguments.At(0).ToUpperInvariant();
		if (propertyName.Contains("MAP"))
			return HandleMap(out response);
		else if (propertyName == "ID")
			return HandleId(out response);

		PropertyInfo? foundProperty = properties.FirstOrDefault(x => x.Name.ToUpperInvariant().Contains(propertyName));
		if (foundProperty == null)
		{
			response = $"There isn't any object property that contains \"{arguments.At(0)}\" in it's name!";
			return false;
		}

		bool result;
		if (typeof(ICollection).IsAssignableFrom(foundProperty.PropertyType))
			result = HandleCollection(out response);
		else if (foundProperty.PropertyType != typeof(string))
			result = HandleNonString(out response);
		else result = HandleString(out response);

		if (!result)
			return false;

		mapEditorObject.UpdateObjectAndCopies();
		response = "You've successfully modified the object!";
		return true;




		bool HandleMap(out string response)
		{
			if (arguments.Count < 2)
			{
				response = "Not enough arguments!";
				return false;
			}

			string newMapName = arguments.At(1);
			if (mapEditorObject.MapName == newMapName)
			{
				response = $"This object is already a part of this map!";
				return false;
			}

			if (newMapName == MapUtils.UntitledMapName)
			{
				response = $"This map name is reserved for internal use!";
				return false;
			}

			MapSchematic oldMap = mapEditorObject.Map;
			if (!MapUtils.LoadedMaps.TryGetValue(newMapName, out MapSchematic newMap)) // Map is already loaded
				if (!MapUtils.TryGetMapData(newMapName, out newMap)) // Map isn't loaded but map file exists
				{ // Map isn't loaded and map file doesn't exist

					newMap = new MapSchematic(newMapName);
					MapUtils.LoadedMaps.Add(newMapName, newMap);
				}


			oldMap.TryRemoveElement(mapEditorObject.Id);
			newMap.TryAddElement(mapEditorObject.Id, mapEditorObject.Base);

			oldMap.Reload();
			newMap.Reload();
			response = "You've successfully modified the object's map!";
			return true;
		}

		bool HandleId(out string response)
		{
			if (arguments.Count < 2)
			{
				response = "Not enough arguments!";
				return false;
			}

			string newId = arguments.At(1);

			if (mapEditorObject.Map.SpawnedObjects.Any(x => x.Id == newId))
			{
				response = $"This ID is already used by an other object!";
				return false;
			}

			mapEditorObject.Map.TryAddElement(newId, mapEditorObject.Base);
			mapEditorObject.Map.TryRemoveElement(mapEditorObject.Id);
			mapEditorObject.Map.Reload();
			response = "You've successfully modified the object's ID!";
			return true;
		}

		bool HandleCollection(out string response)
		{
			object listInstance = foundProperty.GetValue(instance);
			Type listType = foundProperty.PropertyType.GetInterfaces().First(x => x.IsGenericType).GetGenericArguments()[0];

			switch (arguments.At(1).ToLower())
			{
				case "a":
				case "add":
					{
						for (int i = 2; i < arguments.Count; i++)
						{
							try
							{
								object value = TypeDescriptor.GetConverter(listType).ConvertFromInvariantString(arguments.At(i));
								foundProperty.PropertyType.GetMethod("Add").Invoke(listInstance, [value]);
							}
							catch (Exception)
							{
								response = $"\"{arguments.At(i)}\" is not a valid argument! The value should be a {listType} type.";
								return false;
							}
						}
						break;
					}

				case "rm":
				case "remove":
					{
						for (int i = 2; i < arguments.Count; i++)
						{
							try
							{
								object value = TypeDescriptor.GetConverter(listType).ConvertFromInvariantString(arguments.At(i));
								foundProperty.PropertyType.GetMethod("Remove").Invoke(listInstance, [value]);
							}
							catch (Exception)
							{
								response = $"\"{arguments.At(i)}\" is not a valid argument! The value should be a {listType} type.";
								return false;
							}
						}
						break;
					}

				default:
					response = "Invalid arguments! Use add/remove.";
					return false;
			}

			response = string.Empty;
			return true;
		}

		bool HandleNonString(out string response)
		{
			if (arguments.Count < 2 && !foundProperty.PropertyType.IsEnum)
			{
				response = $"You need to provide a {foundProperty.PropertyType} value!";
				return false;
			}

			try
			{
				object value = TypeDescriptor.GetConverter(foundProperty.PropertyType).ConvertFromInvariantString(arguments.At(1));
				foundProperty.SetValue(instance, value);
			}
			catch (Exception)
			{
				StringBuilder sb = StringBuilderPool.Shared.Rent();
				if (arguments.Count > 1)
				{
					sb.Append($"\"{arguments.At(1)}\" is not a valid argument! The value should be a {foundProperty.PropertyType} type.");
				}

				if (foundProperty.PropertyType.IsEnum)
				{
					sb.AppendLine();
					sb.Append($"{foundProperty.PropertyType.ToString().Split('.').Last()} values (use either text name or number, sum numbers for multiple flags)");
					sb.AppendLine();
					foreach (object value in Enum.GetValues(foundProperty.PropertyType))
					{
						sb.Append($"- {value} = {Enum.Format(foundProperty.PropertyType, value, "d")}");
						sb.AppendLine();
					}

					sb.Remove(sb.Length - 1, 1);
				}

				response = StringBuilderPool.Shared.ToStringReturn(sb);
				return false;
			}

			response = string.Empty;
			return true;
		}

		bool HandleString(out string response)
		{
			if (arguments.Count < 2)
			{
				response = "You need to provide a string value!";
				return false;
			}

			StringBuilder spacedStringBuilder = StringBuilderPool.Shared.Rent(arguments.At(1));
			for (int i = 1; i < arguments.Count - 1; i++)
			{
				spacedStringBuilder.Append($" {arguments.At(1 + i)}");
			}

			try
			{
				foundProperty.SetValue(instance, TypeDescriptor.GetConverter(foundProperty.PropertyType).ConvertFromInvariantString(StringBuilderPool.Shared.ToStringReturn(spacedStringBuilder)));
			}
			catch (Exception)
			{
				response = $"\"{arguments.At(1)}\" is not a valid argument! The value should be a {foundProperty.PropertyType} type.";
				return false;
			}

			response = string.Empty;
			return true;
		}
	}
}