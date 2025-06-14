using System.Text;
using CommandSystem;
using LabApi.Features.Permissions;
using NorthwoodLib.Pools;
using ProjectMER.Commands.Map;
using ProjectMER.Commands.Modifying;
using ProjectMER.Commands.Modifying.Position;
using ProjectMER.Commands.Modifying.Rotation;
using ProjectMER.Commands.Modifying.Scale;
using ProjectMER.Commands.ToolGunLike;
using ProjectMER.Commands.Utility;
using ProjectMER.Commands.Utility.MapConverter;

namespace ProjectMER.Commands;

/// <summary>
/// The base parent command.
/// </summary>
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class MapEditorParentCommand : ParentCommand
{
	public MapEditorParentCommand() => LoadGeneratedCommands();

	public override string Command => "mapeditor";

	public override string[] Aliases { get; } = ["mer", "mp"];

	public override string Description => "The MapEditorReborn parent command";

	public override void LoadGeneratedCommands()
	{
        RegisterCommand(new Load());
        RegisterCommand(new Save());
        RegisterCommand(new Unload());

        RegisterCommand(new Position());
        RegisterCommand(new Rotation());
        RegisterCommand(new Scale());
        RegisterCommand(new Modify());

        RegisterCommand(new Create());
        RegisterCommand(new Delete());
        RegisterCommand(new Select());

        RegisterCommand(new Indicators());
        RegisterCommand(new List());
        RegisterCommand(new MapConverter());
        RegisterCommand(new Merge());
        RegisterCommand(new ToggleToolGun());
	}

	protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		StringBuilder sb = StringBuilderPool.Shared.Rent();
		sb.AppendLine();
		sb.Append("Please enter a valid subcommand:");

		foreach (ICommand command in AllCommands)
		{
			if (sender.HasAnyPermission($"mpr.{command.Command}"))
			{
				sb.Append($"\n\n<color=yellow><b>- {command.Command} ({string.Join(", ", command.Aliases)})</b></color>\n<color=white>{command.Description}</color>");
			}
		}

		response = StringBuilderPool.Shared.ToStringReturn(sb);
		return false;
	}
}
