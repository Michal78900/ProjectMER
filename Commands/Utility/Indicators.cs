using CommandSystem;
using LabApi.Features.Permissions;
using ProjectMER.Features.Objects;

namespace ProjectMER.Commands.Utility;

public class Indicators : ICommand
{
	public string Command => "indicators";

	public string[] Aliases => ["i", "si"];

	public string Description => "Shows indicators for invisible objects.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission($"mpr.{Command}") && !ProjectMER.EventUsage())
		{
			response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
			return false;
		}

		if (IndicatorObject.Dictionary.Count > 0)
		{
			IndicatorObject.ClearIndicators();
			response = "Removed all indicators!";
			return true;
		}

		IndicatorObject.RefreshIndicators();
		response = "Indicators have been shown!";
		return true;
	}
}
