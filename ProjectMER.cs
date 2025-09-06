global using Logger = LabApi.Features.Console.Logger;

using HarmonyLib;
using LabApi.Events.CustomHandlers;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Plugins;
using MEC;
using ProjectMER.Configs;
using ProjectMER.Events.Handlers.Internal;
using ProjectMER.Features;
using ProjectMER.Features.Extensions;

namespace ProjectMER;

public class ProjectMER : Plugin<Config>
{
	private Harmony _harmony;
	private FileSystemWatcher _mapFileSystemWatcher;

	public static ProjectMER Singleton { get; private set; }

	/// <summary>
	/// Gets the MapEditorReborn parent folder path.
	/// </summary>
	public static string PluginDir { get; private set; }

	/// <summary>
	/// Gets the folder path in which the maps are stored.
	/// </summary>
	public static string MapsDir { get; private set; }

	/// <summary>
	/// Gets the folder path in which the schematics are stored.
	/// </summary>
	public static string SchematicsDir { get; private set; }

	public GenericEventsHandler GenericEventsHandler { get; } = new();

	public ToolGunEventsHandler ToolGunEventsHandler { get; } = new();

	public ActionOnEventHandlers AcionOnEventHandlers { get; } = new();

	public PickupEventsHandler PickupEventsHandler { get; } = new();

	public override void Enable()
	{
		Singleton = this;
		_harmony = new Harmony($"michal78900.mapEditorReborn-{DateTime.Now.Ticks}");
		_harmony.PatchAll();

		PluginDir = Path.Combine(PathManager.Configs.FullName, "ProjectMER");
		MapsDir = Path.Combine(PluginDir, "Maps");
		SchematicsDir = Path.Combine(PluginDir, "Schematics");

		if (!Directory.Exists(PluginDir))
		{
			Logger.Warn("Plugin directory does not exist. Creating...");
			Directory.CreateDirectory(PluginDir);
		}

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
		CustomHandlersManager.RegisterEventsHandler(AcionOnEventHandlers);
		CustomHandlersManager.RegisterEventsHandler(PickupEventsHandler);

		_harmony = new Harmony($"michal78900.mapEditorReborn-{DateTime.Now.Ticks}");
		_harmony.PatchAll();

		if (Config!.EnableFileSystemWatcher)
		{
			_mapFileSystemWatcher = new FileSystemWatcher(MapsDir)
			{
				NotifyFilter = NotifyFilters.LastWrite,
				Filter = "*.yml",
				EnableRaisingEvents = true,
			};

			_mapFileSystemWatcher.Changed += OnMapFileChanged;

			Logger.Debug("FileSystemWatcher enabled!");
		}

		// plugin loading is synchronous (minus exiled delaying 1 tick), so after all plugins loaded, try checking for Exiled and Exiled CI
		Timing.CallDelayed(1, ExiledExtensions.TryInitialize);
	}

	private void OnMapFileChanged(object _, FileSystemEventArgs ev)
	{
		string mapName = ev.Name.Split('.')[0];
		if (!MapUtils.LoadedMaps.ContainsKey(mapName))
			return;

		Timing.CallDelayed(0.01f, () =>
		{
			try
			{
				MapUtils.LoadMap(mapName);
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		});
	}

	public override void Disable()
	{
		Singleton = null!;
		_harmony.UnpatchAll();

		CustomHandlersManager.UnregisterEventsHandler(GenericEventsHandler);
		CustomHandlersManager.UnregisterEventsHandler(ToolGunEventsHandler);
		CustomHandlersManager.UnregisterEventsHandler(AcionOnEventHandlers);
		CustomHandlersManager.UnregisterEventsHandler(PickupEventsHandler);

		_harmony.UnpatchAll();
		_mapFileSystemWatcher?.Dispose();
	}

	public override string Name => "ProjectMER";

	public override string Description => "MER LabAPI";

	public override string Author => "Michal78900";

	public override Version Version => new Version(2025, 8, 4, 2);

	public override Version RequiredApiVersion => new Version(1, 0, 0, 0);
}
