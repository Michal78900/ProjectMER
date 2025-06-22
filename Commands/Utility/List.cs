using System.Text;
using CommandSystem;
using LabApi.Features.Permissions;
using NorthwoodLib.Pools;
using ProjectMER.Features;
using ProjectMER.Features.Serializable;
using Utils.NonAllocLINQ;

namespace ProjectMER.Commands.Utility;

/// <summary>
/// Command used for listing all saved maps and schematics.
/// </summary>
public class List : ICommand
{
	/// <inheritdoc/>
	public string Command => "list";

	/// <inheritdoc/>
	public string[] Aliases { get; } = ["li", "ls"];

	/// <inheritdoc/>
	public string Description => "Shows the list of all available maps.";

	/// <inheritdoc/>
	public bool SanitizeResponse => false;

	/// <inheritdoc/>
	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission($"mpr.{Command}") && !ProjectMER.EventUsage())
		{
			response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
			return false;
		}

		StringBuilder builder = StringBuilderPool.Shared.Rent();

		builder.AppendLine();
		builder.AppendLine();
		builder.Append("<color=green><b>List of maps:</b></color>");

		List<MapStatus> mapStatuses = ListPool<MapStatus>.Shared.Rent();

		foreach (string filePath in Directory.GetFiles(ProjectMER.MapsDir))
			mapStatuses.AddIfNotContains(new MapStatus(Path.GetFileNameWithoutExtension(filePath)));
		foreach (string loaderMapName in MapUtils.LoadedMaps.Keys)
			mapStatuses.AddIfNotContains(new MapStatus(loaderMapName));

		foreach (MapStatus mapStatus in mapStatuses.OrderByDescending(x => x.IsLoaded).ThenByDescending(x => x.IsDirty))
		{
			builder.AppendLine();
			builder.Append($"- {mapStatus}");
		}

		ListPool<MapStatus>.Shared.Return(mapStatuses);

		builder.AppendLine();
		builder.AppendLine();
		builder.Append("<color=orange><b>List of schematics:</b></color>");

		foreach (string schematicName in MapUtils.GetAvailableSchematicNames())
		{
			builder.AppendLine();
			builder.Append($"- <color=yellow>{schematicName}</color>");
		}

		response = StringBuilderPool.Shared.ToStringReturn(builder);
		return true;
	}

	private readonly struct MapStatus
	{
		public MapStatus(string mapName)
		{
			MapName = mapName;
			IsLoaded = MapUtils.LoadedMaps.TryGetValue(MapName, out MapSchematic map);
			IsDirty = map != null && map.IsDirty;
		}

		public readonly string MapName;
		public readonly bool IsLoaded;
		public readonly bool IsDirty;

		public override readonly string ToString()
		{
			StringBuilder sb = StringBuilderPool.Shared.Rent(MapUtils.GetColoredMapName(MapName));
			sb.Append(" ");

			if (IsLoaded && !IsDirty)
				sb.Append("<color=green>(loaded, saved)</color>");
			else if (IsLoaded && IsDirty)
				sb.Append("<color=red>(loaded, not saved)</color>");
			else
				sb.Append("<color=grey>(unloaded)</color>");

			return StringBuilderPool.Shared.ToStringReturn(sb);
		}
	}
}