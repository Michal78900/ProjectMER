using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldTeleport
    {
        public List<TargetTeleporter> TargetTeleporters { get; set; }
        public List<string> AllowedRoles { get; set; }//List<string> is the original code. I have no idea why you can't use RoleTypeId
        public float Cooldown { get; set; }
        public OldTeleportFlags TeleportFlags { get; set; }
        public OldLockOnEvent LockOnEvent { get; set; }
        public int TeleportSoundId { get; set; }
        public float? PlayerRotationX { get; set; }
        public float? PlayerRotationY { get; set; }
        public OldRoomType RoomType { get; set; }
        public int ObjectId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
    }

    public class TargetTeleporter
    {
        public int Id { get; set; }
        public float Chance { get; set; }
    }
}
