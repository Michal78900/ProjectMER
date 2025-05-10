using GameCore;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Serializable;
using UnityEngine;

namespace ProjectMER.Features.Objects;

public class TeleporterObject : MapEditorObject
{
    // TODO:
    // Implement OnTeleporting event
    // Implement conditional teleport
    // Implement pickup teleport

    public new SerializableTeleporter Base;

    public override MapEditorObject Init(SerializableObject serializableObject, string mapName, string id, Room room)
    {
        MapName = mapName;
        Id = id;
        Room = room;

        if (serializableObject is not SerializableTeleporter serializableTeleporter)
        {
            return this;
        }

        Base = serializableTeleporter;

        GetComponent<BoxCollider>().isTrigger = true;

        return this;
    }

    private TeleporterObject GetTarget()
    {
        return null;
    }

    private bool TryGetTarget(out TeleporterObject teleporterObject)
    {
        teleporterObject = GetTarget();

        return teleporterObject != null;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!CanBeTeleported(collider))
            return;

        Player? player = Player.Get(collider.gameObject);
        if (player is null)
            return;

        if (!TryGetTarget(out TeleporterObject target))
        {
            return;
        }

        player.Position = target.transform.position;
    }

    private bool CanBeTeleported(Collider collider)
    {
        return true;
    }
}

