using System.Text.RegularExpressions;
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

	public override void OnServerWaitingForPlayers() => Timing.RunCoroutine(HandleActionList(Config.OnWaitingForPlayers));
	public override void OnServerRoundStarted() => Timing.RunCoroutine(HandleActionList(Config.OnRoundStarted));
	public override void OnServerLczDecontaminationStarted() => Timing.RunCoroutine(HandleActionList(Config.OnLczDecontaminationStarted));
	public override void OnWarheadStarted(WarheadStartedEventArgs ev) => Timing.RunCoroutine(HandleActionList(Config.OnWarheadStarted));
	public override void OnWarheadStopped(WarheadStoppedEventArgs ev) => Timing.RunCoroutine(HandleActionList(Config.OnWarheadStopped));
	public override void OnWarheadDetonated(WarheadDetonatedEventArgs ev) => Timing.RunCoroutine(HandleActionList(Config.OnWarheadDetonated));

	private IEnumerator<float> HandleActionList(List<string> list)
	{
		foreach (string element in list)
		{
			string[] segments = element.Split(',');

			foreach (string segment in segments)
			{
				string[] actionSplit = segment.Split(':');
				if (actionSplit.Length < 2)
				{
					Logger.Error($"Invalid action segment: {segment}");
					continue;
				}

				string action = actionSplit[0].Trim().ToLowerInvariant();
				string argument = actionSplit[1].Trim();

				switch (action)
				{
					case "load":
					case "l":
						{
							List<string> allMaps = ListPool<string>.Shared.Rent(Directory.GetFiles(ProjectMER.MapsDir).Select(Path.GetFileNameWithoutExtension));
							HandleMapLoading(argument, allMaps);
							ListPool<string>.Shared.Return(allMaps);
							break;
						}

					case "unload":
					case "unl":
						{
							List<string> allMaps = ListPool<string>.Shared.Rent(MapUtils.LoadedMaps.Keys);
							HandleMapUnloading(argument, allMaps);
							ListPool<string>.Shared.Return(allMaps);
							break;
						}

					case "wait":
					case "w":
						{
							if (int.TryParse(argument, out int ms))
								yield return Timing.WaitForSeconds(ms / 1000f);
							else
								Logger.Error($"Invalid wait duration: {argument}");
							break;
						}

					case "console":
					case "cs":
						{
							Server.RunCommand(argument);
							break;
						}

					default:
						{
							Logger.Error($"Unknown action: {action}");
							break;
						}
				}
			}
		}
	}

	private void HandleMapLoading(string argument, List<string> allMaps)
	{
		string[] orSplit = argument.Split('|', '|');
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
			if (Regex.IsMatch(mapName, WildCardToRegular(argument)))
				MapUtils.LoadMap(mapName);
		}
	}

	private void HandleMapUnloading(string argument, List<string> allMaps)
	{
		string[] orSplit = argument.Split('|', '|');
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
			if (Regex.IsMatch(mapName, WildCardToRegular(argument)))
				MapUtils.UnloadMap(mapName);
		}
	}

	private static string WildCardToRegular(string value) => "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
}
