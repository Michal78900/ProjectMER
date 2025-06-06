using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldItemSpawnPoint
    {
        public string Item { get; set; }
        public string AttachmentsCode { get; set; }
        public int SpawnChance { get; set; }
        public uint NumberOfItems { get; set; }
        public int NumberOfUses { get; set; }
        public bool UseGravity { get; set; }
        public bool CanBePickedUp { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public OldRoomType RoomType { get; set; }
    }
}
