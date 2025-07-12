using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using MapGeneration.Distributors;
using Mirror;
using ProjectMER.Features.Extensions;
using UnityEngine;

namespace ProjectMER.Features.Serializable;

public class SerializeableGenerator : SerializableObject
{
    public bool IsUnlocked { get; set; } = false;

    public DoorPermissionFlags Permissions { get; set; } = DoorPermissionFlags.ArmoryLevelTwo;

    public float TotalActivationTime { get; set; } = 124f;

    public bool Engaged { get; set; } = false;
    public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
    {
        Scp079Generator generator = instance == null ? UnityEngine.Object.Instantiate(PrefabManager.Generator) : instance.GetComponent<Scp079Generator>();
        Vector3 position = room.GetAbsolutePosition(Position);
        Quaternion rotation = room.GetAbsoluteRotation(Rotation);
        _prevIndex = Index;
        
        generator.transform.SetPositionAndRotation(position, rotation);
        generator.transform.localScale = Scale;
        
        generator.IsUnlocked = IsUnlocked;
        generator.RequiredPermissions = Permissions;
        generator.TotalActivationTime = TotalActivationTime;
        generator.Engaged = Engaged;
        generator.name = "Generator";
        
        if (generator.TryGetComponent(out StructurePositionSync structurePositionSync))
        {
            structurePositionSync.Network_position = generator.transform.position;
            structurePositionSync.Network_rotationY = (sbyte)Mathf.RoundToInt(generator.transform.rotation.eulerAngles.y / 5.625f);
        }
        NetworkServer.UnSpawn(generator.gameObject);
        NetworkServer.Spawn(generator.gameObject);
        
        return generator.gameObject;
    }
}