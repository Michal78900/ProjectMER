using ProjectMER.Features.Enums;
using System.ComponentModel;

namespace ProjectMER.Configs;

public class Config
{
    [Description("Whether the object will be auto selected when spawning it.")]
    public bool AutoSelect { get; set; } = true;

    [Description(
    "\n" +
    "# ------------------------------Actions on event------------------------------\n" +
    "# Below is the list of in-game events that you can use to call certain action.\n" +
    "# ----------------------------------------------------------------------------\n" +
    "# \n" +
    "# Map loading/unloading\n" +
    "# You can use it to load or unload a map on demand. It supports basic pattern matching, loading/unloading multiple maps or loading/unloading a random map from a list. Loading the same map again reloads it. Unloading the already unloaded map won't do anything.\n" +
    "# \n" +
    "# - load:CoolMap\n" +
    "#   Loads a map that is called CoolMap\n" +
    "# \n" +
    "# - unload:CoolMap\n" +
    "#   Unloads a map that is called CoolMap\n" +
    "# \n" +
    "# - load:LczMap,HczMap,EzMap\n" +
    "#   Loads ALL of the maps listed. You can also just load them individualy with multiple loads.\n" +
    "# \n" +
    "# - load:VariantA||VariantB||VariantC\n" +
    "#   Loads ONE of the maps listed, chances are equal, you can increase them by typing same map name multiple times\n" +
    "# \n" +
    "# - load:*\n" +
    "#   Loads all saved maps\n" +
    "# \n" +
    "# - unload:*\n" +
    "#   Loads all loaded maps, including the Untitled one\n" +
    "# \n" +
    "# Console command\n" +//What the hell console? This plugin is named what? Map Editor Reborn, not Auto Console Reborn.
    "# You can use it to run a custom console command. Remote Admin commands must be prefixed with \"/\"\n" +
    "# \n" +
    "# - console:buildinfo\n" +
    "#   Prints a buildinfo of the server\n" +
    "# \n" +
    "# - console:/bc 10 MER is cool\n" +
    "#   Sends a broadcast to all players\n"
    )]
    public List<HandleAction> OnWaitingForPlayers { get; set; } = new();
    public List<HandleAction> OnRoundStarted { get; set; } = new List<HandleAction>()
    {
        new HandleAction() { Action = HandleActionType.Load, MapName = "stub_map_name" },
    };
    public List<HandleAction> OnLczDecontaminationStarted { get; set; } = new();
    public List<HandleAction> OnWarheadStarted { get; set; } = new();
    public List<HandleAction> OnWarheadStopped { get; set; } = new();
    public List<HandleAction> OnWarheadDetonated { get; set; } = new();
}

public class HandleAction
{
    public HandleActionType Action { get; set; }
    public string MapName { get; set; }
}
