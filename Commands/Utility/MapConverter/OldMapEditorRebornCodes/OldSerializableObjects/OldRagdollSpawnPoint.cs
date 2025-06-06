using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldRagdollSpawnPoint
    {
        public string Name { get; set; }
        public OldRoleTypeId RoleType { get; set; }
        public string DeathReason { get; set; }
        public int SpawnChance { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public OldRoomType RoomType { get; set; }
    }
}
