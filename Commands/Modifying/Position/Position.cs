using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using ProjectMER.Commands.Modifying.Position.SubCommands;

namespace ProjectMER.Commands.Modifying.Position;

/// <summary>
/// Command used for modifing object's position.
/// </summary>
public class Position : ParentCommand
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Position"/> class.
	/// </summary>
	public Position() => LoadGeneratedCommands();

	/// <inheritdoc/>
	public override string Command => "position";

	/// <inheritdoc/>
	public override string[] Aliases { get; } = ["pos"];

	/// <inheritdoc/>
	public override string Description => "Modifies object's posistion.";

	/// <inheritdoc/>
	public override void LoadGeneratedCommands()
	{
		RegisterCommand(new Add());
		RegisterCommand(new Set());
		RegisterCommand(new Bring());
		RegisterCommand(new Grab());
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

		response = "\nUsage:\n";
		response += "mp position set (x) (y) (z)\n";
		response += "mp position add (x) (y) (z)\n";
		response += "mp position bring\n";
		response += "mp position grab";

		return false;
	}
}
