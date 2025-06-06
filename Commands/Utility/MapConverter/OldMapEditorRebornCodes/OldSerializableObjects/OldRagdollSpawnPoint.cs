using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldRagdollSpawnPoint
    {
        public string Name { get; set; } = string.Empty;
        public OldRoleTypeId RoleType { get; set; } = OldRoleTypeId.ClassD;
        public string DeathReason { get; set; } = "None";
        public int SpawnChance { get; set; } = 100;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public OldRoomType RoomType { get; set; } = OldRoomType.Unknown;
    }
}
