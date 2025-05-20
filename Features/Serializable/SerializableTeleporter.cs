using AdminToys;
using LabApi.Features.Wrappers;
using Mirror;
using PlayerRoles;
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
        PrimitiveObjectToy root;
        PrimitiveObjectToy trigger;
        PrimitiveObjectToy cylinder;
        PrimitiveObjectToy arrowY;
        PrimitiveObjectToy arrowX;
        PrimitiveObjectToy arrow;

        Vector3 position = room.GetAbsolutePosition(Position - new Vector3(0, 0.5f, 0));
        Quaternion rotation = room.GetAbsoluteRotation(Rotation);

        if (instance == null)
        {
            root = UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObjectPrefab);
            root.NetworkPrimitiveFlags = PrimitiveFlags.None;
            root.name = "Indicator";
            root.transform.position = position;

            trigger = UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObjectPrefab);
            trigger.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
            trigger.name = "Trigger";
            trigger.NetworkPrimitiveType = PrimitiveType.Cube;
            trigger.transform.localScale = Scale;
            trigger.transform.position = position + new Vector3(0, 0.5f, 0);
            trigger.transform.parent = root.transform;

            cylinder = GameObject.Instantiate(PrefabManager.PrimitiveObjectPrefab, root.transform);
            cylinder.transform.localPosition = Vector3.zero;
            cylinder.NetworkPrimitiveType = PrimitiveType.Cylinder;
            cylinder.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
            cylinder.transform.localScale = new Vector3(1f, 0.001f, 1f);

            arrowY = UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObjectPrefab);
            arrowY.NetworkPrimitiveFlags = PrimitiveFlags.None;
            arrowY.name = "Arrow Y Axis";
            arrowY.transform.parent = root.transform;

            arrowX = UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObjectPrefab);
            arrowX.NetworkPrimitiveFlags = PrimitiveFlags.None;
            arrowX.name = "Arrow X Axis";
            arrowX.transform.parent = arrowY.transform;

            arrow = GameObject.Instantiate(PrefabManager.PrimitiveObjectPrefab, arrowX.transform);
            arrow.transform.localPosition = root.transform.forward;
            arrow.NetworkPrimitiveType = PrimitiveType.Cube;
            arrow.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
            arrow.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
        }
        else
        {
            root = instance.GetComponent<PrimitiveObjectToy>();

            trigger = root.transform.Find("Trigger").GetComponent<PrimitiveObjectToy>();
            arrowY = root.transform.Find("Arrow Y Axis").GetComponent<PrimitiveObjectToy>();
            arrowX = arrowY.transform.Find("Arrow X Axis").GetComponent<PrimitiveObjectToy>();

            trigger.transform.localScale = Scale;
        }

        root.transform.position = position;
        arrowY.transform.localPosition = Vector3.up * 1.6f;
        arrowY.transform.localEulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
        arrowX.transform.localPosition = Vector3.zero;
        arrowX.transform.localEulerAngles = new Vector3(-rotation.eulerAngles.x, 0f, 0f);

        foreach (PrimitiveObjectToy primitive in root.GetComponentsInChildren<PrimitiveObjectToy>())
        {
            if (Targets.Count > 0)
            {
                primitive.NetworkMaterialColor = new Color(0.11f, 0.98f, 0.92f, 0.5f);
            }
            else
            {
                primitive.NetworkMaterialColor = new Color(1f, 1f, 1f, 0.25f);
            }
        }

        return root.gameObject;
    }
}