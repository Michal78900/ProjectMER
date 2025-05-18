using GameCore;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Serializable;
using UnityEngine;
using MEC;

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
    public bool IsUsable => DateTime.Now > WhenWillBeUsable;

    /// <summary>
    /// Gets or sets the base <see cref="SerializableTeleporter"/> for this object.
    /// </summary>
    public SerializableTeleporter Base { get; set; }

    /// <summary>
    /// Gets or sets the global position of the object.
    /// </summary>
    public Vector3 Position
    {
        get => transform.position;
        set
        {
            transform.position = value;
        }
    }

    /// <summary>
    /// Gets or sets the global rotation of the object.
    /// </summary>
    public Quaternion Rotation
    {
        get => transform.rotation;
        set
        {
            transform.rotation = value;
        }
    }

    /// <summary>
    /// Gets or sets the global euler angles of the object.
    /// </summary>
    public Vector3 EulerAngles
    {
        get => Rotation.eulerAngles;
        set => Rotation = Quaternion.Euler(value);
    }

    /// <summary>
    /// Gets or sets the scale of the object.
    /// </summary>
    public Vector3 Scale
    {
        get => transform.localScale;
        set
        {
            transform.localScale = value;
        }
    }

    /// <summary>
    /// Gets a Dictionary to access teleporters by their ID.
    /// </summary>
    internal static List<TeleporterObject> Teleporters { get; private set; } = new ();

    private TeleporterObject GetTarget()
    {
        return Teleporters.FirstOrDefault(t => t.Base.Id == Base.Targets.RandomItem());
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

        WhenWillBeUsable = DateTime.Now.AddSeconds(Base.Cooldown);
        target.WhenWillBeUsable = DateTime.Now.AddSeconds(target.Base.Cooldown);


        Timing.CallDelayed(0.05f, () =>
        {
            player.Position = target.Position;
            try
            {
                player.LookRotation = target.EulerAngles;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        });
    }

    private void Start()
    {
        Teleporters.Add(this);
    }

    private void OnDestroy()
    {
        Teleporters.Remove(this);
    }

    private bool CanBeTeleported(Collider collider)
    {
        return true;
    }
}
