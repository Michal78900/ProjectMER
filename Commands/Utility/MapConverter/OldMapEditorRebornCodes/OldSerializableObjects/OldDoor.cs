using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldDoor
    {
        public OldDoorType DoorType { get; set; }
        public bool IsOpen { get; set; }
        public bool IsLocked { get; set; }
        public OldKeycardPermissions KeycardPermissions { get; set; }
        public OldDoorDamageType IgnoredDamageSources { get; set; }
        public float DoorHealth { get; set; }
        public OldLockOnEvent LockOnEvent { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public OldRoomType RoomType { get; set; }
    }
}
