using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldVanillaDoor
    {
        public OldDoorType DoorType { get; set; }
        public bool IsOpen { get; set; }
        public bool IsLocked { get; set; }
        public OldKeycardPermissions KeycardPermissions { get; set; }
        public OldDoorDamageType IgnoredDamageSources { get; set; }
        public float DoorHealth { get; set; }
        public OldLockOnEvent LockOnEvent { get; set; }
    }
}
