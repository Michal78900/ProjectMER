using Interactables.Interobjects;
using LabApi.Features.Wrappers;
using MapGeneration.Distributors;
using Mirror;
using ProjectMER.Features.Extensions;
using UnityEngine;

namespace ProjectMER.Features.Serializable;

public class SerializeableGate : SerializableObject
{
    public bool IsLocked { get; set; } = false;

    public bool Is106Passable { get; set; } = true;

    public bool Is106PassableWhenLocked { get; set; } = false;
    
    public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
    {
        var gate = instance == null ? UnityEngine.Object.Instantiate(PrefabManager.Gate) : instance.GetComponent<PryableDoor>();

        Vector3 position = room.GetAbsolutePosition(Position);
        Quaternion rotation = room.GetAbsoluteRotation(Rotation);

        gate.transform.SetPositionAndRotation(position, rotation);
        gate.transform.localScale = Scale;

        if (gate.TryGetComponent(out StructurePositionSync structurePositionSync))
        {
            structurePositionSync.Network_position = gate.transform.position;
            structurePositionSync.Network_rotationY =
                (sbyte)Mathf.RoundToInt(gate.transform.rotation.eulerAngles.y / 5.625f);
        }

        NetworkServer.UnSpawn(gate.gameObject);
        NetworkServer.Spawn(gate.gameObject);
        return gate.gameObject;
    }
}