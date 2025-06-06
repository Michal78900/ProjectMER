using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldLocker
    {
        public OldLockerType LockerType { get; set; }
        public Dictionary<int, List<LockerItemSerializable>> Chambers { get; set; }
        public List<string> AllowedRoleTypes { get; set; }//List<string> is the original code. I have no idea why you can't use RoleTypeId
        public bool ShuffleChambers { get; set; }
        public OldKeycardPermissions KeycardPermissions { get; set; }
        public ushort OpenedChambers { get; set; }
        public bool InteractLock { get; set; }
        public float Chance { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public OldRoomType RoomType { get; set; }
    }

    public class LockerItemSerializable
    {
        public string Item { get; set; }
        public uint Count { get; set; }
        public List<OldAttachmentName> Attachments { get; set; }
        public int Chance { get; set; }
    }
}
