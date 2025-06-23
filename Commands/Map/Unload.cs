using CommandSystem;
using LabApi.Features.Permissions;
using ProjectMER.Features;

namespace ProjectMER.Commands.Map;

public class Unload : ICommand
{
	public string Command => "unload";

	public string[] Aliases => ["unl"];

	public string Description => "Unloads a map";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission($"mpr.{Command}") && !ProjectMER.EventUsage())
		{
			response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
			return false;
		}

		if (arguments.Count == 0)
		{
			foreach (string mapName in MapUtils.LoadedMaps.Keys.ToList())
			{
				MapUtils.UnloadMap(mapName);
			}

			response = "Unloaded all maps!";
			return true;
		}

		MapUtils.UnloadMap(arguments.At(0));
		response = $"Unload {arguments.At(0)} map!";
		return true;
	}
}
