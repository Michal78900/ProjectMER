using InventorySystem.Items.Firearms.Attachments;
using LabApi.Features.Wrappers;
using MapGeneration.Distributors;
using Mirror;
using ProjectMER.Features.Extensions;
using UnityEngine;

namespace ProjectMER.Features.Serializable;

public class SerializableWorkstation : SerializableObject
{
	/// <summary>
	/// Gets or sets a value indicating whether the player can interact with the workstation.
	/// </summary>
	public bool IsInteractable { get; set; } = true;

	public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		WorkstationController workstation = instance == null ? UnityEngine.Object.Instantiate(PrefabManager.Workstation) : instance.GetComponent<WorkstationController>();
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		workstation.transform.SetPositionAndRotation(position, rotation);
		workstation.transform.localScale = Scale;

		workstation.NetworkStatus = (byte)(IsInteractable ? 0 : 4);

		if (workstation.TryGetComponent(out StructurePositionSync structurePositionSync))
		{
			structurePositionSync.Network_position = workstation.transform.position;
			structurePositionSync.Network_rotationY = (sbyte)Mathf.RoundToInt(workstation.transform.rotation.eulerAngles.y / 5.625f);
		}

		NetworkServer.UnSpawn(workstation.gameObject);
		NetworkServer.Spawn(workstation.gameObject);

		return workstation.gameObject;
	}
}
