using AdminToys;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Interfaces;
using UnityEngine;
using static AdminToys.InvisibleInteractableToy;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace ProjectMER.Features.Serializable;

public class SerializableInteractable : SerializableObject, IIndicatorDefinition
{
	public ColliderShape ColliderShape { get; set; } = ColliderShape.Box;
	public float InteractionDuration { get; set; } = 0f;
	public bool IsLocked { get; set; } = false;

	public override GameObject? SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		InvisibleInteractableToy interactable = instance == null ? UnityEngine.Object.Instantiate(PrefabManager.Interactable) : instance.GetComponent<InvisibleInteractableToy>();
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		interactable.transform.SetPositionAndRotation(position, rotation);
		interactable.transform.localScale = Scale;
		interactable.NetworkMovementSmoothing = 60;

		interactable.NetworkShape = ColliderShape;
		interactable.NetworkInteractionDuration = InteractionDuration;
		interactable.NetworkIsLocked = IsLocked;

		if (instance == null)
			NetworkServer.Spawn(interactable.gameObject);

		return interactable.gameObject;
	}

	public GameObject SpawnOrUpdateIndicator(Room room, GameObject? instance = null)
	{
		PrimitiveObjectToy cube;

		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);

		if (instance == null)
		{
			cube = UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObject);
			cube.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
			cube.NetworkMaterialColor = new Color(1f, 1f, 0f, 0.9f);
		}
		else
		{
			cube = instance.GetComponent<PrimitiveObjectToy>();
		}

		cube.NetworkPrimitiveType = PrimitiveType;
		cube.transform.localScale = Scale;

		cube.transform.SetPositionAndRotation(position, rotation);

		return cube.gameObject;
	}

	private PrimitiveType PrimitiveType
	{
		get
		{
			PrimitiveType primitiveType = ColliderShape switch
			{
				ColliderShape.Box => PrimitiveType.Cube,
				ColliderShape.Sphere => PrimitiveType.Sphere,
				ColliderShape.Capsule => PrimitiveType.Capsule,
				_ => throw new InvalidOperationException(),
			};

			return primitiveType;
		}
	}
}
