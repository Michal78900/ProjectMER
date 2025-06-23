using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using ProjectMER.Commands.Modifying.Rotation.SubCommands;

namespace ProjectMER.Commands.Modifying.Rotation;

/// <summary>
/// Command used for modifing object's rotation.
/// </summary>
public class Rotation : ParentCommand
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Rotation"/> class.
	/// </summary>
	public Rotation() => LoadGeneratedCommands();

	/// <inheritdoc/>
	public override string Command => "rotation";

	/// <inheritdoc/>
	public override string[] Aliases { get; } = ["rot"];

	/// <inheritdoc/>
	public override string Description => "Modifies object's rotation.";

	/// <inheritdoc/>
	public override void LoadGeneratedCommands()
	{
		RegisterCommand(new Add());
		RegisterCommand(new Set());
	}

	/// <inheritdoc/>
	protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission($"mpr.{Command}") && !ProjectMER.EventUsage())
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

		response = "\nUsage:";
		response += "\nmp rotation set (x) (y) (z)";
		response += "\nmp rotation add (x) (y) (z)";
		return false;
	}
}