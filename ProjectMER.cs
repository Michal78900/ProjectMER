
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Plugins;
using ProjectMER.Configs;
using ProjectMER.Events.Handlers.Internal;

namespace ProjectMER;

public class ProjectMER : Plugin<Config>
{
	/// <summary>
	/// Gets the MapEditorReborn parent folder path.
	/// </summary>
	public static string PluginDir { get; } = Path.Combine(PathManager.Configs.ToString(), "ProjectMER");

	/// <summary>
	/// Gets the folder path in which the maps are stored.
	/// </summary>
	public static string MapsDir { get; } = Path.Combine(PluginDir, "Maps");

	/// <summary>
	/// Gets the folder path in which the schematics are stored.
	/// </summary>
	public static string SchematicsDir { get; } = Path.Combine(PluginDir, "Schematics");

	public static ProjectMER Singleton { get; private set; }

	public GenericEventsHandler GenericEventsHandler { get; } = new();

	public ToolGunEventsHandler ToolGunEventsHandler { get; } = new();

	public MapOnEventHandlers MapOnEventHandlers { get; } = new();

	public PickupEventsHandler PickupEventsHandler { get; } = new();

	public override void Enable()
	{
		Singleton = this;

		if (!Directory.Exists(MapsDir))
		{
			Logger.Warn("Maps directory does not exist. Creating...");
			Directory.CreateDirectory(MapsDir);
		}

		if (!Directory.Exists(SchematicsDir))
		{
			Logger.Warn("Schematics directory does not exist. Creating...");
			Directory.CreateDirectory(SchematicsDir);
		}

		CustomHandlersManager.RegisterEventsHandler(GenericEventsHandler);
		CustomHandlersManager.RegisterEventsHandler(ToolGunEventsHandler);
		CustomHandlersManager.RegisterEventsHandler(MapOnEventHandlers);
		CustomHandlersManager.RegisterEventsHandler(PickupEventsHandler);
	}

	public override void Disable()
	{
		Singleton = null!;

		CustomHandlersManager.UnregisterEventsHandler(GenericEventsHandler);
		CustomHandlersManager.UnregisterEventsHandler(ToolGunEventsHandler);
		CustomHandlersManager.UnregisterEventsHandler(MapOnEventHandlers);
		CustomHandlersManager.UnregisterEventsHandler(PickupEventsHandler);
	}

	public override string Name => "ProjectMER";

	public override string Description => "MER LabAPI";

	public override string Author => "Michal78900";

	public override Version Version => new Version(2025, 3, 17, 1);

	public override Version RequiredApiVersion => new Version(1, 0, 0, 0);
}
