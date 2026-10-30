using LabApi.Features.Wrappers;
using ProjectMER.Features.Serializable.Schematics;
using UnityEngine;

namespace ProjectMER.Features.Objects;

public class SchematicTeleportObject : MonoBehaviour
{
    // Teleport data loaded from the Unity schematic.
    public SchematicTeleportData Base;

    // Dictionary containing all teleports in the current schematic.
    // Used to find destination teleporters by their IDs.
    public Dictionary<int, SchematicTeleportObject> AllTeleports;

    // Time until this teleporter can be used again.
    public DateTime NextTimeUse;

    /// <summary>
    /// Selects a random destination teleporter based on the configured chances.
    /// </summary>
    public SchematicTeleportObject GetRandomTarget()
    {
        if (Base.TargetTeleporters == null || Base.TargetTeleporters.Count == 0)
            return null;

        // Calculate the total chance of all possible destinations.
        float totalChance = Base.TargetTeleporters.Sum(x => x.Chance);

        if (totalChance <= 0f)
            return null;

        // Generate a random value within the total chance range.
        float randomValue = UnityEngine.Random.Range(0f, totalChance);

        foreach (var target in Base.TargetTeleporters)
        {
            // Subtract each destination's chance until
            // the random value falls within its range.
            randomValue -= target.Chance;

            if (randomValue <= 0f)
            {
                // Find the selected destination using its ID.
                if (AllTeleports.TryGetValue(target.Id, out SchematicTeleportObject teleport))
                    return teleport;

                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// Called by Unity when another collider enters this object's trigger area.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Check whether the entering object belongs to a player.
        Player player = Player.Get(other.gameObject);

        if (player == null)
            return;

        // Check whether the teleport cooldown has expired.
        if (NextTimeUse > DateTime.Now)
            return;

        // Select a destination based on the configured chances.
        SchematicTeleportObject target = GetRandomTarget();

        if (target == null)
            return;

        // Set the next available use time for both the source and destination teleports.
        DateTime nextUse = DateTime.Now.AddSeconds(Base.Cooldown);

        NextTimeUse = nextUse;
        target.NextTimeUse = nextUse;

        // Move the player to the destination and set their view direction
        // according to the destination teleporter's rotation.
        player.Position = target.transform.position;
        player.LookRotation = target.transform.eulerAngles;
    }
}
