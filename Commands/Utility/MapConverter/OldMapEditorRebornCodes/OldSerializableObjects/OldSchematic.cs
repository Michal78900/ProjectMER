using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldSchematic
    {
        public string SchematicName { get; set; }
        public OldCullingType CullingType { get; set; }
        public bool IsPickable { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public OldRoomType RoomType { get; set; }
    }
}
