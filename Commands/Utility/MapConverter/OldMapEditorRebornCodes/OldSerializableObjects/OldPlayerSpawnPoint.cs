using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldPlayerSpawnPoint
    {
        public OldSpawnableTeam SpawnableTeam { get; set; }
        public Vector3 Position { get; set; }
        public OldRoomType RoomType { get; set; }
    }
}
