using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldPrimitive
    {
        public OldPrimitiveType PrimitiveType { get; set; }
        public string Color { get; set; }
        public OldPrimitiveFlags PrimitiveFlags { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public OldRoomType RoomType { get; set; }
    }
}
