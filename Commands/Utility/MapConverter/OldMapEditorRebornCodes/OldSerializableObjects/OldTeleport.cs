using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldTeleport
    {
        public List<TargetTeleporter> TargetTeleporters { get; set; } = new()
        {
            new TargetTeleporter(),
        };
        //List<string> is the original code. I have no idea why you can't use RoleTypeId
        public List<string> AllowedRoles { get; set; } = new()
        {
            "Scp0492",
            "Scp049",
            "Scp096",
            "Scp106",
            "Scp173",
            "Scp939",
            "Scp3114",
            "ZombieFlamingo",
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
            "AlphaFlamingo",
            "Flamingo",
        };
        public float Cooldown { get; set; } = 10f;
        public OldTeleportFlags TeleportFlags { get; set; } = OldTeleportFlags.Player;
        public OldLockOnEvent LockOnEvent { get; set; } = OldLockOnEvent.None;
        public int TeleportSoundId { get; set; } = -1;
        public float? PlayerRotationX { get; set; } = null;
        public float? PlayerRotationY { get; set; } = null;
        public OldRoomType RoomType { get; set; } = OldRoomType.Unknown;
        public int ObjectId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = Vector3.one;
    }

    public class TargetTeleporter
    {
        public int Id { get; set; }
        public float Chance { get; set; } = 100f;
    }
}
