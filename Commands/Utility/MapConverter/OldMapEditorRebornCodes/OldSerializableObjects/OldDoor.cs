using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldDoor
    {
        public OldDoorType DoorType { get; set; }
        public bool IsOpen { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public OldKeycardPermissions KeycardPermissions { get; set; }
        public OldDoorDamageType IgnoredDamageSources { get; set; } = OldDoorDamageType.Weapon;
        public float DoorHealth { get; set; } = 150f;
        public OldLockOnEvent LockOnEvent { get; set; } = OldLockOnEvent.None;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = Vector3.one;
        public OldRoomType RoomType { get; set; } = OldRoomType.Unknown;
    }
}
