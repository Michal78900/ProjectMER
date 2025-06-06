using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldVanillaDoor
    {
        public bool IsOpen { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public OldKeycardPermissions KeycardPermissions { get; set; }
        public OldDoorDamageType IgnoredDamageSources { get; set; } = OldDoorDamageType.Weapon;
        public float DoorHealth { get; set; } = 150f;
        public OldLockOnEvent LockOnEvent { get; set; } = OldLockOnEvent.None;
    }
}
