using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldPrimitive
    {
        public OldPrimitiveType PrimitiveType { get; set; } = OldPrimitiveType.Cube;
        public string Color { get; set; } = "red";
        public OldPrimitiveFlags PrimitiveFlags { get; set; } = (OldPrimitiveFlags)3;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = Vector3.one;
        public OldRoomType RoomType { get; set; } = OldRoomType.Unknown;
    }
}
