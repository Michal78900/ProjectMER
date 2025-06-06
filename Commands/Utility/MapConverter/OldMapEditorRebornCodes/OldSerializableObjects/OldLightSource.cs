using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldLightSource
    {
        public string Color { get; set; }
        public float Intensity { get; set; }
        public float Range { get; set; }
        public bool Shadows { get; set; }
        public Vector3 Position { get; set; }
        public OldRoomType RoomType { get; set; }
    }
}