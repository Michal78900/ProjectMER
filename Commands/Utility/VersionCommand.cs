using System;
using CommandSystem;
using LabApi.Features.Permissions;

namespace ProjectMER.Commands.Utility;

public class VersionCommand : ICommand
{
	public string Command => "version";

	public string[] Aliases => ["ver", "v"];

	public string Description => "Shows the current version of ProjectMER.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission($"mpr.{Command}"))
		{
			response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
			return false;
		}

		response = $"ProjectMER version: <color=yellow>{ProjectMER.Singleton.Version}</color>";
		return true;
	}
}
