using AdminToys;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Interfaces;
using ProjectMER.Features.Objects;
using UnityEngine;
using static HarmonyLib.Code;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace ProjectMER.Features.Serializable;

public class SerializableTeleporter : SerializableObject, IIndicatorDefinition
{
    public int TeleporterId { get; set; }

    public List<TargetTeleporter> TargetTeleporters { get; set; } = new List<TargetTeleporter>()
    {
        new TargetTeleporter(),
    };

    public float Cooldown { get; set; } = 10f;

    public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
    {
        GameObject primitive = instance == null ? GameObject.CreatePrimitive(PrimitiveType.Cube) : instance;
        Vector3 position = room.GetAbsolutePosition(Position);
        Quaternion rotation = room.GetAbsoluteRotation(Rotation);
        _prevIndex = Index;

        primitive.transform.SetPositionAndRotation(position, rotation);
        primitive.transform.localScale = Scale;

        return primitive;
    }

    public GameObject SpawnOrUpdateIndicator(Room room, GameObject? instance = null)
    {
        PrimitiveObjectToy primitive = instance == null ? UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObjectPrefab) : instance.GetComponent<PrimitiveObjectToy>();
        Vector3 position = room.GetAbsolutePosition(Position);
        Quaternion rotation = room.GetAbsoluteRotation(Rotation);
        _prevIndex = Index;

        primitive.transform.SetPositionAndRotation(position, rotation);
        primitive.transform.localScale = Scale;
        primitive.NetworkMovementSmoothing = 60;

        primitive.PrimitiveFlags = PrimitiveFlags.Visible;
        primitive.PrimitiveType = PrimitiveType.Cube;
        Color transparentColor = "#03fcec".GetColorFromString();

        primitive.NetworkMaterialColor = transparentColor;

        if (instance == null)
            NetworkServer.Spawn(primitive.gameObject);

        return primitive.gameObject;
    }
}

