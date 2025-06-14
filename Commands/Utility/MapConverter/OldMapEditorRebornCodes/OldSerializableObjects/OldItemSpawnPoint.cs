using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldItemSpawnPoint
    {
        public string Item { get; set; } = "KeycardJanitor";
        public string AttachmentsCode { get; set; } = "-1";
        public int SpawnChance { get; set; } = 100;
        public uint NumberOfItems { get; set; } = 1;
        public int NumberOfUses { get; set; } = 1;
        public bool UseGravity { get; set; } = true;
        public bool CanBePickedUp { get; set; } = true;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = Vector3.one;
        public OldRoomType RoomType { get; set; } = OldRoomType.Unknown;
    }
}
