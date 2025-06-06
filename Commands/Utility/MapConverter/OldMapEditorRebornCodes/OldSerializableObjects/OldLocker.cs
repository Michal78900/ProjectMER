using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldLocker
    {
        public OldLockerType LockerType { get; set; }
        public Dictionary<int, List<LockerItemSerializable>> Chambers { get; set; } = new()
        {
            { 0, new () { new () } },
        };
        //List<string> is the original code. I have no idea why you can't use RoleTypeId
        public List<string> AllowedRoleTypes { get; set; } = new()
        {
            "Scp0492",
            "Scp049",
            "Scp096",
            "Scp106",
            "Scp173",
            "Scp93953",
            "Scp93989",
            "ClassD",
            "Scientist",
            "FacilityGuard",
            "NtfPrivate",
            "NtfSergeant",
            "NtfSpecialist",
            "NtfCaptain",
            "ChaosConscript",
            "ChaosRifleman",
            "ChaosRepressor",
            "ChaosMarauder",
            "Tutorial",
        };
        public bool ShuffleChambers { get; set; } = true;
        public OldKeycardPermissions KeycardPermissions { get; set; } = OldKeycardPermissions.None;
        public ushort OpenedChambers { get; set; }
        public bool InteractLock { get; set; }
        public float Chance { get; set; } = 100f;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = Vector3.one;
        public OldRoomType RoomType { get; set; } = OldRoomType.Unknown;
    }

    public class LockerItemSerializable
    {
        public string Item { get; set; } = "Coin";
        public uint Count { get; set; } = 1;
        public List<OldAttachmentName> Attachments { get; set; }
        public int Chance { get; set; } = 100;
    }
}
