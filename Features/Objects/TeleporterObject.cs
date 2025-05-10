using GameCore;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Serializable;
using UnityEngine;

namespace ProjectMER.Features.Objects;

public class TeleporterObject : MonoBehaviour
{
    // TODO:
    // Implement OnTeleporting event
    // Implement conditional teleport
    // Implement pickup teleport
    // Implement chance-based target select.

    /// <summary>
    /// Gets a <see cref="DateTime"/> indicating when this teleporter will next be usable.
    /// </summary>
    public DateTime WhenWillBeUsable { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this teleporter is currently usable.
    /// </summary>
    public bool IsUsable { get => DateTime.Now > WhenWillBeUsable; }

    /// <summary>
    /// Gets or sets the base <see cref="SerializableTeleporter"/> for this object.
    /// </summary>
    public SerializableTeleporter Base { get; set; }

    /// <summary>
    /// Gets or sets the global position of this object.
    /// </summary>
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }

        set
        {
            transform.position = value;
        }
    }

    /// <summary>
    /// Gets or sets the global rotation of this object.
    /// </summary>
    public Quaternion Rotation
    {
        get
        {
            return transform.rotation;
        }

        set
        {
            transform.rotation = value;
        }
    }

    /// <summary>
    /// Gets or sets the global euler angles of this object.
    /// </summary>
    public Vector3 EulerAngles
    {
        get
        {
            return Rotation.eulerAngles;
        }

        set
        {
            Rotation = Quaternion.Euler(value);
        }
    }

    /// <summary>
    /// Gets or sets the localScale for this object.
    /// </summary>
    public Vector3 Scale
    {
        get
        {
            return transform.localScale;
        }

        set
        {
            transform.localScale = value;
        }
    }

    /// <summary>
    /// Gets a Dictionary to access teleporters by their ID.
    /// </summary>
    internal static Dictionary<int, TeleporterObject> TeleportersFromId { get; private set; } = new ();

    private TeleporterObject GetTarget()
    {
        return TeleportersFromId[Base.TargetTeleporters.RandomItem().Id];
    }

    private bool TryGetTarget(out TeleporterObject teleporterObject)
    {
        teleporterObject = GetTarget();

        return teleporterObject != null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsUsable || !CanBeTeleported(other) || !TryGetTarget(out TeleporterObject target))
        {
            return;
        }

        Player? player = Player.Get(other.gameObject);
        if (player is null)
        {
            return;
        }

        player.Position = target.Position;
        WhenWillBeUsable = DateTime.Now.AddSeconds(Base.Cooldown);
        target.WhenWillBeUsable = DateTime.Now.AddSeconds(target.Base.Cooldown);
    }

    private void Start()
    {
        TeleportersFromId.Add(Base.TeleporterId, this);
    }

    private void OnDestroy()
    {
        TeleportersFromId.Remove(Base.TeleporterId);
    }

    private bool CanBeTeleported(Collider collider)
    {
        return true;
    }
}
