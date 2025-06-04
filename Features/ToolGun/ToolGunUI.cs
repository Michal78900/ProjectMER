using System.Reflection;
using System.Text;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using UnityEngine;
using UserSettings.ServerSpecific;
using YamlDotNet.Serialization;

namespace ProjectMER.Features.ToolGun;

public static class ToolGunUI
{
	public static string GetHintHUD(Player player)
	{
		StringBuilder sb = StringBuilderPool.Shared.Rent();

		sb.Append("<font=\"LiberationSans SDF\">");

		int offset = 0;
		object instance = null!;
		List<PropertyInfo> properties = [];
		if (ToolGunHandler.TryGetSelectedMapObject(player, out MapEditorObject mapEditorObject) && mapEditorObject != null)
		{
			instance = mapEditorObject.GetType().GetField("Base").GetValue(mapEditorObject);
			properties = instance.GetType().GetProperties().ToList();
			offset = properties.Count - properties.Count(x => Attribute.IsDefined(x, typeof(YamlIgnoreAttribute))) + 2 + 2;
		}

		for (int i = 0; i < 36 - offset; i++)
		{
			sb.Append("<size=50%> </size>");
			sb.AppendLine();
		}

		if (mapEditorObject != null)
		{
			sb.Append($"<size=50%>MapName: {MapUtils.GetColoredMapName(mapEditorObject.MapName)}</size>");
			sb.AppendLine();
			sb.Append($"<size=50%>ID: {MapUtils.GetColoredString(mapEditorObject.Id)}</size>");
			sb.AppendLine();
		}

		foreach (string property in properties.GetColoredProperties(instance))
			{
				sb.Append($"<size=50%>");
				sb.Append(property);
				sb.Append("</size>");
				sb.AppendLine();
			}

		if (offset > 0)
			sb.AppendLine();

		if (!player.CurrentItem.IsToolGun(out ToolGunItem toolGun))
			return StringBuilderPool.Shared.ToStringReturn(sb);

		sb.Append($"<size=50%>");
		sb.Append(GetToolGunModeString(player, toolGun));
		sb.Append("</size>");

		sb.AppendLine();


		sb.Append($"<size=50%>");
		sb.Append($"{player.Position:F3}");
		sb.Append("</size>");

		sb.AppendLine();

		sb.Append($"<size=50%>");
		sb.Append(GetRoomString(player));
		sb.Append("</size>");
		sb.Append("</font>");

		return StringBuilderPool.Shared.ToStringReturn(sb);
	}

	private static string GetToolGunModeString(Player player, ToolGunItem toolGun)
	{
		if (toolGun.CreateMode)
		{
			string output;
			if (toolGun.SelectedObjectToSpawn == ToolGunObjectType.Schematic)
			{
				if (ServerSpecificSettingsSync.TryGetSettingOfUser(player.ReferenceHub, 0, out SSDropdownSetting dropdownSetting) && dropdownSetting.TryGetSyncSelectionText(out string schematicName))
					output = schematicName.ToUpper();
				else
					output = "Please select schematic in options";
			}
			else
			{
				output = toolGun.SelectedObjectToSpawn.ToString().ToUpper();
			}

			return $"<color=green>CREATE</color>\n<color=yellow>{output}</color>";
		}

		string name = " ";
		if (ToolGunHandler.Raycast(player, out RaycastHit hit))
		{
			if (hit.transform.TryGetComponentInParent(out MapEditorObject mapEditorObject))
			{
				if (mapEditorObject is IndicatorObject indicatorObject)
					mapEditorObject = IndicatorObject.Dictionary[indicatorObject];

				if (mapEditorObject.gameObject.TryGetComponent(out SchematicObject schematicObject))
				{
					name = schematicObject.Name.ToUpper();
				}
				else
				{
					name = mapEditorObject.Base.ToString().Split('.').Last().Replace("Serializable", "").ToUpper();
				}
			}
		}

		if (toolGun.DeleteMode)
			return $"<color=red>DELETE</color>\n<color=yellow>{name}</color>";

		if (toolGun.SelectMode)
			return $"<color=yellow>SELECT</color>\n<color=yellow>{name}</color>";

		return "\n ";
	}

	private static string GetRoomString(Player player)
	{
		Room room = RoomExtensions.GetRoomAtPosition(player.Camera.transform.position);
		List<Room> list = ListPool<Room>.Shared.Rent(Room.List.Where(x => x.Base != null && x.Zone == room.Zone && x.Shape == room.Shape && x.Name == room.Name));

		string roomString;
		if (list.Count == 1)
		{
			roomString = room.GetRoomStringId();
		}
		else
		{
			roomString = $"{room.GetRoomStringId()} ({list.IndexOf(room)}) ({list.Count})";
		}

		ListPool<Room>.Shared.Return(list);
		return roomString;
	}
}
