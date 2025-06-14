using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldShootingTarget
    {
        public OldShootingTargetType TargetType { get; set; } = OldShootingTargetType.Sport;
        public bool IsFunctional { get; set; } = true;
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = Vector3.one;
        public OldRoomType RoomType { get; set; } = OldRoomType.Unknown;
    }
}
