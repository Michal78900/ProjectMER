using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects;
using System.ComponentModel;

namespace ProjectMER.Commands.Utility.MapConverter
{
    [Serializable]
    public class OldMapSchematic
    {
        [Description("Whether the default player spawnpoints should be removed. Keep in mind, that given role spawnpoint will be removed only if there is at least one custom spawn point of that type.")]
        public bool RemoveDefaultSpawnPoints { get; set; }
        [Description("List of possible names for ragdolls spawned by RagdollSpawnPoints.")]
        public Dictionary<OldRoleTypeId, List<string>> RagdollRoleNames { get; set; }
        public Dictionary<string, OldVanillaDoor> VanillaDoors { get; set; }
        public OldVanillaTeslaProperties VanillaTeslaProperties { get; set; }
        public List<OldDoor> Doors { get; set; }
        public List<OldWorkstation> WorkStations { get; set; }
        public List<OldItemSpawnPoint> ItemSpawnPoints { get; set; }
        public List<OldPlayerSpawnPoint> PlayerSpawnPoints { get; set; }
        public List<OldRagdollSpawnPoint> RagdollSpawnPoints { get; set; }
        public List<OldShootingTarget> ShootingTargets { get; set; }
        public List<OldPrimitive> Primitives { get; set; }
        public List<OldLightSource> LightSources { get; set; }
        public List<OldRoomLight> RoomLights { get; set; }
        public List<OldTeleport> Teleports { get; set; }
        public List<OldLocker> Lockers { get; set; }
        public List<OldSchematic> Schematics { get; set; }
    }
}
