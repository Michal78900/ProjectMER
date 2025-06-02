using System;
using AdminToys;
using LabApi.Features.Wrappers;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Interfaces;
using ProjectMER.Features.Objects;
using UnityEngine;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace ProjectMER.Features.Serializable;

public class SerializableTeleport : SerializableObject, IIndicatorDefinition
{
	public List<string> Targets { get; set; } = [];

    public int Chance { get; set; } = 100;

    public List<string> AllowedRoles { get; set; } = new List<string>()
    {
        "Scp0492",
        "Scp049",
        "Scp096",
        "Scp106",
        "Scp173",
        "Scp939",
        "Scp3114",
        "ClassD",
        "Scientist",
        "FacilityGuard",
        "NtfPrivate",
        "NtfSergeant",
        "NtfSpecialist",
        "NtfCaptain",
        "ChaosConscript",
        "ChaosRifleman",
        "ChaosRepressor",
        "ChaosMarauder",
        "Tutorial",
    };

    public float Cooldown { get; set; } = 5f;

    public override GameObject? SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		GameObject gameObject = instance ?? new GameObject("Teleport");
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;
		gameObject.transform.SetLocalPositionAndRotation(position, rotation);
        BoxCollider collider;

        if (instance == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            gameObject.AddComponent<TeleportObject>();
        }
        else
        {
            collider = gameObject.GetComponent<BoxCollider>();
        }

        if (collider != null)
        {
            collider.size = Scale;
        }

        return gameObject;
	}

	public GameObject SpawnOrUpdateIndicator(Room room, GameObject? instance = null)
	{
		PrimitiveObjectToy root;
		PrimitiveObjectToy trigger;
		PrimitiveObjectToy arrowY;
		PrimitiveObjectToy arrowX;
		PrimitiveObjectToy arrow;

		Vector3 position = room.GetAbsolutePosition(Position);
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
			trigger.transform.position = position;
			trigger.transform.parent = root.transform;

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
		trigger.transform.localEulerAngles = rotation.eulerAngles;
		arrowY.transform.localPosition = Vector3.up * 0.6f;
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
