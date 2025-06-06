using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldRoomLight
    {
        public string Color { get; set; } = "red";
        public float ShiftSpeed { get; set; }
        public bool OnlyWarheadLight { get; set; } = false;
        public OldRoomType RoomType { get; set; } = OldRoomType.Unknown;
    }
}
