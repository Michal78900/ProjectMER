using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using UnityEngine;

namespace ProjectMER.Features.Serializable;

public class SerializableDoor : SerializableObject
{
	public DoorType DoorType { get; set; } = DoorType.Lcz;
	public bool IsOpen { get; set; } = false;
	public bool IsLocked { get; set; } = false;
	public DoorPermissionFlags RequiredPermissions { get; set; } = DoorPermissionFlags.None;
	public bool RequireAll { get; set; } = true;

	public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		DoorVariant doorVariant;
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		if (instance == null)
		{
			doorVariant = GameObject.Instantiate(DoorPrefab);
			if (doorVariant.TryGetComponent(out DoorRandomInitialStateExtension doorRandomInitialStateExtension))
				GameObject.Destroy(doorRandomInitialStateExtension);
		}
		else
		{
			doorVariant = instance.GetComponent<DoorVariant>();
		}
  
		doorVariant.transform.SetPositionAndRotation(position, rotation);
		doorVariant.transform.localScale = Scale;

		_prevType = DoorType;
		SetupDoor(doorVariant);

		NetworkServer.UnSpawn(doorVariant.gameObject);
		NetworkServer.Spawn(doorVariant.gameObject);

		return doorVariant.gameObject;
	}

	public void SetupDoor(DoorVariant doorVariant)
	{
		doorVariant.NetworkTargetState = IsOpen;
		doorVariant.ServerChangeLock(DoorLockReason.SpecialDoorFeature, IsLocked);
		doorVariant.RequiredPermissions = new DoorPermissionsPolicy(RequiredPermissions, RequireAll);
	}

	private DoorVariant DoorPrefab
	{
		get
		{
			DoorVariant prefab = DoorType switch
			{
				DoorType.Lcz => PrefabManager.DoorLcz,
				DoorType.Hcz => PrefabManager.DoorHcz,
				DoorType.Ez => PrefabManager.DoorEz,
				DoorType.Bulkdoor => PrefabManager.DoorHeavyBulk,
				_ => throw new InvalidOperationException(),
			};

			return prefab;
		}
	}

	public override bool RequiresReloading => DoorType != _prevType || base.RequiresReloading;

	internal DoorType _prevType;
}
