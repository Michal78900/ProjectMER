using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldLightSource
    {
        public string Color { get; set; } = "white";
        public float Intensity { get; set; } = 1f;
        public float Range { get; set; } = 1f;
        public bool Shadows { get; set; } = true;
        public Vector3 Position { get; set; }
        public OldRoomType RoomType { get; set; } = OldRoomType.Unknown;
    }
}