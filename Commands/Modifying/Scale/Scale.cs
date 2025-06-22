using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using ProjectMER.Commands.Modifying.Scale.SubCommands;

namespace ProjectMER.Commands.Modifying.Scale;

public class Scale : ParentCommand
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Scale"/> class.
	/// </summary>
	public Scale() => LoadGeneratedCommands();

	/// <inheritdoc/>
	public override string Command => "scale";

	/// <inheritdoc/>
	public override string[] Aliases { get; } = ["scl"];

	/// <inheritdoc/>
	public override string Description => "Modifies object's scale.";

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

		response = "\nUsage:\n";
		response += "mp scale set (x) (y) (z)\n";
		response += "mp scale add (x) (y) (z)\n";
		return false;
	}
}