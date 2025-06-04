using System.IO.Enumeration;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MEC;
using NorthwoodLib.Pools;
using ProjectMER.Configs;
using ProjectMER.Features;

namespace ProjectMER.Events.Handlers.Internal;

public class ActionOnEventHandlers : CustomEventsHandler
{
	private static Config Config => ProjectMER.Singleton.Config!;

	public override void OnServerWaitingForPlayers() => Timing.CallDelayed(0.1f, () => HandleActionList(Config.OnWaitingForPlayers));
	public override void OnServerRoundStarted() => HandleActionList(Config.OnRoundStarted);
	public override void OnServerLczDecontaminationStarted() => HandleActionList(Config.OnLczDecontaminationStarted);
	public override void OnWarheadStarted(WarheadStartedEventArgs ev) => HandleActionList(Config.OnWarheadStarted);
	public override void OnWarheadStopped(WarheadStoppedEventArgs ev) => HandleActionList(Config.OnWarheadStopped);
	public override void OnWarheadDetonated(WarheadDetonatedEventArgs ev) => HandleActionList(Config.OnWarheadDetonated);

	private void HandleActionList(List<string> list)
	{
		foreach (string element in list)
		{
			string[] actionSplit = element.Split(':');
			string action = actionSplit[0];
			string argument = actionSplit[1];

			switch (action.ToLowerInvariant())
			{
				case "load":
				case "l":
					{
						List<string> allMaps = ListPool<string>.Shared.Rent(Directory.GetFiles(ProjectMER.MapsDir).Select(Path.GetFileNameWithoutExtension));
						HandleMapLoading(argument, allMaps);
						ListPool<string>.Shared.Return(allMaps);
						continue;
					}

				case "unload":
				case "unl":
					{
						List<string> allMaps = ListPool<string>.Shared.Rent(MapUtils.LoadedMaps.Keys);
						HandleMapUnloading(argument, allMaps);
						ListPool<string>.Shared.Return(allMaps);
						continue;
					}

				case "console":
				case "cs":
					{
						Server.RunCommand(argument);
						continue;
					}

				default:
					{
						Logger.Error($"Unknown action: {action}");
						continue;
					}
			}
		}
	}

	private void HandleMapLoading(string argument, List<string> allMaps)
	{
		string[] orSplit = argument.Split("||");
		string[] andSplit = argument.Split(',');

		if (orSplit.Length > 1 || andSplit.Length > 1)
		{
			if (andSplit.Length > orSplit.Length)
				andSplit.ForEach(x => HandleMapLoading(x, allMaps));
			else
				HandleMapLoading(orSplit.RandomItem(), allMaps);

			return;
		}

		foreach (string mapName in allMaps)
		{
			if (FileSystemName.MatchesSimpleExpression(argument, mapName))
				MapUtils.LoadMap(mapName);
		}
	}

	private void HandleMapUnloading(string argument, List<string> allMaps)
	{
		string[] orSplit = argument.Split("||");
		string[] andSplit = argument.Split(',');

		if (orSplit.Length > 1 || andSplit.Length > 1)
		{
			if (andSplit.Length > orSplit.Length)
				andSplit.ForEach(x => HandleMapLoading(x, allMaps));
			else
				HandleMapLoading(orSplit.RandomItem(), allMaps);

			return;
		}

		foreach (string mapName in allMaps)
		{
			if (FileSystemName.MatchesSimpleExpression(argument, mapName))
				MapUtils.UnloadMap(mapName);
		}
	}
}
