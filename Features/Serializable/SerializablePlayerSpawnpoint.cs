using AdminToys;
using LabApi.Features.Wrappers;
using PlayerRoles;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Interfaces;
using Respawning.Objectives;
using UnityEngine;
using YamlDotNet.Serialization;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace ProjectMER.Features.Serializable;

public class SerializablePlayerSpawnpoint : SerializableObject, IIndicatorDefinition
{
	public List<RoleTypeId> Roles { get; set; } = [];

	[YamlIgnore]
	public override Vector3 Scale { get; set; }

	public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		GameObject spawnpoint = instance ?? new GameObject("PlayerSpawnpoint");
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		spawnpoint.transform.SetPositionAndRotation(position, rotation);

		return spawnpoint.gameObject;
	}

	public GameObject SpawnOrUpdateIndicator(Room room, GameObject? instance = null)
	{
		PrimitiveObjectToy root;
		PrimitiveObjectToy cylinder;
		PrimitiveObjectToy arrowY;
		PrimitiveObjectToy arrowX;
		PrimitiveObjectToy arrow;

		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);

		if (instance == null)
		{
			root = UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObject);
			root.NetworkPrimitiveFlags = PrimitiveFlags.None;
			root.name = "Indicator";
			root.transform.position = position;

			cylinder = GameObject.Instantiate(PrefabManager.PrimitiveObject, root.transform);
			cylinder.transform.localPosition = Vector3.zero;
			cylinder.NetworkPrimitiveType = PrimitiveType.Cylinder;
			cylinder.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
			cylinder.transform.localScale = new Vector3(1f, 0.001f, 1f);

			arrowY = UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObject);
			arrowY.NetworkPrimitiveFlags = PrimitiveFlags.None;
			arrowY.name = "Arrow Y Axis";
			arrowY.transform.parent = root.transform;

			arrowX = UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObject);
			arrowX.NetworkPrimitiveFlags = PrimitiveFlags.None;
			arrowX.name = "Arrow X Axis";
			arrowX.transform.parent = arrowY.transform;

			arrow = GameObject.Instantiate(PrefabManager.PrimitiveObject, arrowX.transform);
			arrow.transform.localPosition = root.transform.forward;
			arrow.NetworkPrimitiveType = PrimitiveType.Cube;
			arrow.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
			arrow.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
		}
		else
		{
			root = instance.GetComponent<PrimitiveObjectToy>();
			
			arrowY = root.transform.Find("Arrow Y Axis").GetComponent<PrimitiveObjectToy>();
			arrowX = arrowY.transform.Find("Arrow X Axis").GetComponent<PrimitiveObjectToy>();
		}

		root.transform.position = position;
		arrowY.transform.localPosition = Vector3.up * 1.6f;
		arrowY.transform.localEulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
		arrowX.transform.localPosition = Vector3.zero;
		arrowX.transform.localEulerAngles = new Vector3(-rotation.eulerAngles.x, 0f, 0f);

		foreach (PrimitiveObjectToy primitive in root.GetComponentsInChildren<PrimitiveObjectToy>())
		{
			if (Roles.Count > 0)
			{
				Color colorSum = new(0f, 0f, 0f, 1f);
				foreach (RoleTypeId roleType in Roles)
				{
					Color roleColor = roleType.GetRoleColor();
					colorSum.r += roleColor.r;
					colorSum.g += roleColor.g;
					colorSum.b += roleColor.b;
				}

				primitive.NetworkMaterialColor = new Color(colorSum.r / Roles.Count, colorSum.g / Roles.Count, colorSum.b / Roles.Count, colorSum.a);
			}
			else
			{
				primitive.NetworkMaterialColor = new Color(1f, 1f, 1f, 0.25f);
			}
		}

		return root.gameObject;
	}
}
