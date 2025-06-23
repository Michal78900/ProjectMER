using CommandSystem;
using LabApi.Features.Permissions;
using ProjectMER.Features;

namespace ProjectMER.Commands.Map;

public class Save : ICommand
{
	public string Command => "save";

	public string[] Aliases => ["s"];

	public string Description => "Saves a map";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission($"mpr.{Command}") && !ProjectMER.EventUsage())
		{
			response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
			return false;
		}

		if (arguments.Count == 0)
		{
			response = "You need to provide a map name!";
			return false;
		}

		MapUtils.SaveMap(arguments.At(0));

		response = $"Map named {arguments.At(0)} has been successfully saved!";
		return true;
	}
}
