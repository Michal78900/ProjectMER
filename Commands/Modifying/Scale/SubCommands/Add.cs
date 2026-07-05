using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using ProjectMER.Features.Objects;
using ProjectMER.Features.ToolGun;
using UnityEngine;
using static ProjectMER.Features.Extensions.StructExtensions;

namespace ProjectMER.Commands.Modifying.Scale.SubCommands;

public class Add : ICommand
{
	/// <inheritdoc/>
	public string Command => "add";

	/// <inheritdoc/>
	public string[] Aliases { get; } = [];

	/// <inheritdoc/>
	public string Description => string.Empty;

	/// <inheritdoc/>
	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission($"mpr.scale"))
		{
			response = $"You don't have permission to execute this command. Required permission: mpr.scale";
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
			response = "You need to select an object first!";
			return false;
		}

		if (arguments.Count >= 3 && TryGetVector(arguments.At(0), arguments.At(1), arguments.At(2), out Vector3 newScale))
		{
            if(mapEditorObject.GetComponentInParent<AdminToys.WaypointToy>() != null || mapEditorObject.GetComponentInChildren<AdminToys.WaypointToy>() != null)
			{
				AdminToys.WaypointToy waypointToy = mapEditorObject.GetComponent<AdminToys.WaypointToy>();
                WaypointToy waypointToy1 = WaypointToy.Get(waypointToy);
                waypointToy1.BoundsSize += newScale;
				response = waypointToy1.BoundsSize.ToString("F3");
                return true;
            }

            mapEditorObject.Base.Scale += newScale;
			mapEditorObject.UpdateObjectAndCopies();

			response = mapEditorObject.Base.Scale.ToString("F3");
			return true;
		}

		response = "Invalid values.";
		return false;
	}
}