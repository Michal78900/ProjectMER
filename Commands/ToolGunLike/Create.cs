using System.Text;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using ProjectMER.Configs;
using ProjectMER.Features;
using ProjectMER.Features.Enums;
using ProjectMER.Features.ToolGun;
using UnityEngine;
using static ProjectMER.Features.Extensions.StructExtensions;

namespace ProjectMER.Commands.ToolGunLike;

public class Create : ICommand
{
	/// <inheritdoc/>
	public string Command => "create";

	/// <inheritdoc/>
	public string[] Aliases { get; } = ["cr", "spawn"];

	/// <inheritdoc/>
	public string Description => "Creates a selected object at the point you are looking at.";

	/// <inheritdoc/>
	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission($"mpr.{Command}"))
		{
			response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
			return false;
		}

		Player? player = Player.Get(sender)!;

		if (arguments.Count == 0)
		{
			StringBuilder sb = StringBuilderPool.Shared.Rent();
			sb.AppendLine();
			sb.Append("List of all spawnable objects:");
			sb.AppendLine();
			sb.AppendLine();
			foreach (ToolGunObjectType objectType in ToolGunItem.TypesDictionary.Keys)
			{
				if (objectType == ToolGunObjectType.Schematic)
					continue;

				sb.Append($"- {objectType} ({(int)objectType})");
				sb.AppendLine();
			}

			sb.AppendLine();
			sb.Append("To spawn a custom schematic, please use it's file name as an argument.");

			response = StringBuilderPool.Shared.ToStringReturn(sb);
			return true;
		}

		Vector3 position = Vector3.zero;
		if (arguments.Count >= 4 && !TryGetVector(arguments.At(1), arguments.At(2), arguments.At(3), out position))
		{
			response = "Invalid arguments. Usage: mp create <object> <posX> <posY> <posZ>";
			return false;
		}

		if (arguments.Count == 1)
		{
			if (!ToolGunHandler.Raycast(player, out RaycastHit hit))
			{
				response = "Couldn't find a valid surface on which the object could be spawned!";
				return false;
			}

			position = hit.point;
		}
		else if (arguments.Count < 4)
		{
			response = "Invalid arguments. Usage: mp create <object> optionally: <posX> <posY> <posZ>";
			return false;
		}

		string objectName = arguments.At(0);

		if (Enum.TryParse(objectName, true, out ToolGunObjectType parsedEnum) && Enum.IsDefined(typeof(ToolGunObjectType), parsedEnum))
		{
			ToolGunHandler.CreateObject(position, parsedEnum);
			if (Config.AutoSelect && player is not null)
				ToolGunHandler.SelectObject(player, MapUtils.UntitledMap.SpawnedObjects.Last());

			response = $"{objectName} has been successfully spawned!";
			return true;
		}

		try
		{
			_ = MapUtils.GetSchematicDataByName(objectName);
		}
		catch (Exception e)
		{
			response = e.Message.ToString();
			return false;
		}

		ToolGunHandler.CreateObject(position, ToolGunObjectType.Schematic, objectName);
		if (Config.AutoSelect && player is not null)
			ToolGunHandler.SelectObject(player, MapUtils.UntitledMap.SpawnedObjects.LastOrDefault());

		response = $"{objectName} has been successfully spawned!";
		return true;
	}

	private static Config Config => ProjectMER.Singleton.Config!;
}
