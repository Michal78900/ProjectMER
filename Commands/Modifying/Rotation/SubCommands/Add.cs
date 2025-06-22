using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using ProjectMER.Features.Objects;
using ProjectMER.Features.ToolGun;
using UnityEngine;
using static ProjectMER.Features.Extensions.StructExtensions;

namespace ProjectMER.Commands.Modifying.Rotation.SubCommands;

/// <summary>
/// Modifies object's rotation by adding a certain value to the current one.
/// </summary>
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
		if (!sender.HasAnyPermission($"mpr.rotation") && !ProjectMER.EventUsage())
		{
			response = $"You don't have permission to execute this command. Required permission: mpr.rotation";
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

		if (arguments.Count >= 3 && TryGetVector(arguments.At(0), arguments.At(1), arguments.At(2), out Vector3 newRotation))
		{
			mapEditorObject.Base.Rotation += newRotation;
			mapEditorObject.UpdateObjectAndCopies();

			response = mapEditorObject.Base.Rotation.ToString("F3");
			return true;
		}

		response = "Invalid values.";
		return false;
	}
}
