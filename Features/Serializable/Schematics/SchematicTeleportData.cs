using UnityEngine;

namespace ProjectMER.Features.Serializable.Schematics;

/// <summary>
/// Stores teleport data exported from a Unity schematic.
/// Used to recreate teleports on the server.
/// </summary>
public class SchematicTeleportData
{
    // Original Unity component parameters.
    // Some of these parameters are not currently implemented.
    public int RoomType { get; set; }

    // Possible destination teleporters and their selection chances.
    public List<SchematicTeleportTargetData> TargetTeleporters { get; set; } = new();

    public List<string> AllowedRoles { get; set; } = new();

    public float Cooldown { get; set; } = 10f;

    public int TeleportFlags { get; set; }

    public int LockOnEvent { get; set; }

    public int TeleportSoundId { get; set; } = -1;

    public float PlayerRotationX { get; set; }

    public float PlayerRotationY { get; set; }

    // Object name and identifiers from Unity.
    public string Name { get; set; } = string.Empty;

    public int ObjectId { get; set; }

    // ID of the parent object used to determine the teleport's local position.
    public int ParentId { get; set; }

    // Teleport's local position, rotation, and scale.
    public Vector3 Position { get; set; }

    public Vector3 Rotation { get; set; }

    public Vector3 Scale { get; set; } = Vector3.one;
}

/// <summary>
/// Represents one possible teleport destination and its selection chance.
/// </summary>
public class SchematicTeleportTargetData
{
    // ObjectId of the destination teleporter in the Unity schematic.
    public int Id { get; set; }

    // Destination selection weight.
    // For example, values of 70 and 30 result in 70% and 30% chances.
    public float Chance { get; set; } = 100f;
}
