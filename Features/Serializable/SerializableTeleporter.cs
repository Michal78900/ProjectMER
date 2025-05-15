using AdminToys;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Interfaces;
using ProjectMER.Features.Objects;
using UnityEngine;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace ProjectMER.Features.Serializable;

public class SerializableTeleporter : SerializableObject, IIndicatorDefinition
{
    // TODO:
    // Add proper ID management
    private int teleporterId;

    /// <summary>
    /// Gets or sets the teleporter ID for this teleporter.
    /// </summary>
    public int Id
    {
        get => teleporterId;
        set
        {
            if (TeleporterObject.TeleportersFromId.ContainsKey(value) || value < 1)
            {
                return;
            }

            teleporterId = value;
        }
    }

    public List<int> Targets { get; set; } = new List<int>();

    public float Cooldown { get; set; } = 10f;

    public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
    {
        GameObject primitive = instance == null ? GameObject.CreatePrimitive(PrimitiveType.Cube) : instance;
        Vector3 position = room.GetAbsolutePosition(Position);
        Quaternion rotation = room.GetAbsoluteRotation(Rotation);
        _prevIndex = Index;

        primitive.transform.SetPositionAndRotation(position, rotation);
        primitive.transform.localScale = Scale;

        if (!primitive.TryGetComponent(out TeleporterObject teleporter))
        {
            teleporter = primitive.AddComponent<TeleporterObject>();
        }

        teleporter.Base = this;

        if (primitive.TryGetComponent(out BoxCollider collider))
        {
            collider.isTrigger = true;
        }

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
        Color transparentColor = new Color(0.11f, 0.98f, 0.92f, 0.5f);

        primitive.NetworkMaterialColor = transparentColor;

        if (instance == null)
            NetworkServer.Spawn(primitive.gameObject);

        return primitive.gameObject;
    }
}