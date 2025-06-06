using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MEC;
using NorthwoodLib.Pools;
using ProjectMER.Configs;
using ProjectMER.Features;
using ProjectMER.Features.Enums;

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

    private void HandleActionList(List<HandleAction> handleActionList)
    {
        foreach (var handleAction in handleActionList)
        {
            switch (handleAction.Action)
            {
                case HandleActionType.Load:
                    {
                        List<string> allMaps = ListPool<string>.Shared.Rent(Directory.GetFiles(ProjectMER.MapsDir).Select(Path.GetFileNameWithoutExtension));
                        HandleMapLoading(handleAction.MapName, allMaps);
                        ListPool<string>.Shared.Return(allMaps);
                        break;
                    }
                case HandleActionType.Unload:
                    {
                        List<string> allMaps = ListPool<string>.Shared.Rent(MapUtils.LoadedMaps.Keys);
                        HandleMapUnloading(handleAction.MapName, allMaps);
                        ListPool<string>.Shared.Return(allMaps);
                        break;
                    }
                case HandleActionType.Console:
                    {
                        var command = handleAction.MapName;
                        Server.RunCommand(command);
                        break;
                    }
                default:
                    {
                        Logger.Error($"Unknown action: {handleAction.Action}");
                        continue;
                    }
            }
        }
    }

    private void HandleMapLoading(string argument, List<string> allMaps)
    {
        string[] orSplit = argument.Split(["||"], StringSplitOptions.None);//Dude, are you ******* nuts? You're pushing code to production that doesn't even compile.
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
            //if (FileSystemName.MatchesSimpleExpression(argument, mapName))//Dude, are you ******* nuts? You're pushing code to production that doesn't even compile.
            MapUtils.LoadMap(mapName);
        }
    }

    private void HandleMapUnloading(string argument, List<string> allMaps)
    {
        string[] orSplit = argument.Split(["||"], StringSplitOptions.None);
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
            //if (FileSystemName.MatchesSimpleExpression(argument, mapName))//Dude, are you ******* nuts? You're pushing code to production that doesn't even compile.
            MapUtils.UnloadMap(mapName);
        }
    }
}
