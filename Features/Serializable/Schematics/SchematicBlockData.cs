using AdminToys;
using InventorySystem.Items.Firearms.Attachments;
using LabApi.Features.Wrappers;
using ProjectMER.Events.Handlers.Internal;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using UnityEngine;
using LightSourceToy = AdminToys.LightSourceToy;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;
using TextToy = AdminToys.TextToy;
using WaypointToy = AdminToys.WaypointToy;

namespace ProjectMER.Features.Serializable.Schematics;

public class SchematicBlockData
{
	public virtual string Name { get; set; }

	public virtual int ObjectId { get; set; }

	public virtual int ParentId { get; set; }

	public virtual string AnimatorName { get; set; }

	public virtual Vector3 Position { get; set; }

	public virtual Vector3 Rotation { get; set; }

	public virtual Vector3 Scale { get; set; }

	public virtual BlockType BlockType { get; set; }

	public virtual Dictionary<string, object> Properties { get; set; }

	/// <summary>
	/// Returns true if this block has Static=true in its Properties.
	/// Used to apply NetworkIsStatic AFTER NetworkServer.Spawn() to avoid Mirror SyncVar initialization order issues.
	/// </summary>
	public bool IsStatic => Properties != null && Properties.TryGetValue("Static", out object isStatic) && Convert.ToBoolean(isStatic);

	public GameObject Create(SchematicObject schematicObject, Transform parentTransform)
	{
		GameObject gameObject = BlockType switch
		{
			BlockType.Empty => CreateEmpty(),
			BlockType.Primitive => CreatePrimitive(),
			BlockType.Light => CreateLight(),
			BlockType.Pickup => CreatePickup(schematicObject),
			BlockType.Workstation => CreateWorkstation(),
			BlockType.Text => CreateText(),
			BlockType.Interactable => CreateInteractable(schematicObject),
			BlockType.Waypoint => CreateWaypoint(),
			_ => CreateEmpty(true)
		};

		gameObject.name = Name;

		Transform transform = gameObject.transform;
		transform.SetParent(parentTransform);
		transform.SetLocalPositionAndRotation(Position, Quaternion.Euler(Rotation));
		
		if (BlockType == BlockType.Waypoint)
			gameObject.GetComponent<WaypointToy>().NetworkBoundsSize = Scale;
		else
			transform.localScale = BlockType switch
			{
				BlockType.Empty when Scale == Vector3.zero => Vector3.one,
				_ => Scale
			};

		if (gameObject.TryGetComponent(out AdminToyBase adminToyBase))
		{
			adminToyBase.NetworkIsStatic = IsStatic;
			if (!IsStatic)
				adminToyBase.NetworkMovementSmoothing = 60;
		}

		return gameObject;
	}

	private GameObject CreateEmpty(bool fallback = false)
	{
		if (fallback)
			Logger.Warn($"{BlockType} is not yet implemented. Object will be an empty GameObject instead.");

		PrimitiveObjectToy primitive = GameObject.Instantiate(PrefabManager.PrimitiveObject);
		primitive.NetworkPrimitiveFlags = PrimitiveFlags.None;

		return primitive.gameObject;
	}

	private GameObject CreatePrimitive()
	{
		PrimitiveObjectToy primitive = GameObject.Instantiate(PrefabManager.PrimitiveObject);

		primitive.NetworkPrimitiveType = (PrimitiveType)Convert.ToInt32(Properties["PrimitiveType"]);
		primitive.NetworkMaterialColor = Properties["Color"].ToString().GetColorFromString();

		PrimitiveFlags primitiveFlags;
		if (Properties.TryGetValue("PrimitiveFlags", out object flags))
		{
			primitiveFlags = (PrimitiveFlags)Convert.ToByte(flags);
		}
		else
		{
			// Backward compatibility
			primitiveFlags = PrimitiveFlags.Visible;
			if (Scale.x >= 0f)
				primitiveFlags |= PrimitiveFlags.Collidable;
		}

		primitive.NetworkPrimitiveFlags = primitiveFlags;

		return primitive.gameObject;
	}

	private GameObject CreateLight()
	{
		LightSourceToy light = GameObject.Instantiate(PrefabManager.LightSource);

		light.NetworkLightType = Properties.TryGetValue("LightType", out object lightType) ? (LightType)Convert.ToInt32(lightType) : LightType.Point;
		light.NetworkLightColor = Properties["Color"].ToString().GetColorFromString();
		light.NetworkLightIntensity = Convert.ToSingle(Properties["Intensity"]);
		light.NetworkLightRange = Convert.ToSingle(Properties["Range"]);

		if (Properties.TryGetValue("Shadows", out object shadows))
		{
			// Backward compatibility
			light.NetworkShadowType = Convert.ToBoolean(shadows) ? LightShadows.Soft : LightShadows.None;
		}
		else
		{
			light.NetworkShadowType = (LightShadows)Convert.ToInt32(Properties["ShadowType"]);
			light.NetworkLightShape = (LightShape)Convert.ToInt32(Properties["Shape"]);
			light.NetworkSpotAngle = Convert.ToSingle(Properties["SpotAngle"]);
			light.NetworkInnerSpotAngle = Convert.ToSingle(Properties["InnerSpotAngle"]);
			light.NetworkShadowStrength = Convert.ToSingle(Properties["ShadowStrength"]);
		}

		return light.gameObject;
	}

	private GameObject CreatePickup(SchematicObject schematicObject)
	{
		if (Properties.TryGetValue("Chance", out object property) && UnityEngine.Random.Range(0, 101) > Convert.ToSingle(property))
			return new("Empty Pickup");

		Pickup pickup = Pickup.Create((ItemType)Convert.ToInt32(Properties["ItemType"]), Vector3.zero)!;

		if (pickup is FirearmPickup firearmPickup && Properties.TryGetValue("AttachmentsCode", out object attachmentsCodeObj))
		{
			string codeStr = Convert.ToString(attachmentsCodeObj);
			if (uint.TryParse(codeStr, out uint code) && code != uint.MaxValue)
			{
				firearmPickup.AttachmentCode = code;
			}
		}

		int uses = Properties.TryGetValue("Uses", out object usesObj) ? Convert.ToInt32(usesObj) : 1;
		if (uses != 1)
		{
			PickupEventsHandler.PickupUsesLeft[pickup.Serial] = uses;
		}

		if (Properties.ContainsKey("Locked"))
			PickupEventsHandler.ButtonPickups.Add(pickup.Serial, schematicObject);

		return pickup.GameObject;
	}

	private GameObject CreateWorkstation()
	{
		WorkstationController workstation = GameObject.Instantiate(PrefabManager.Workstation);
		workstation.NetworkStatus = (byte)(Properties.TryGetValue("IsInteractable", out object isInteractable) && Convert.ToBoolean(isInteractable) ? 0 : 4);

		return workstation.gameObject;
	}

	private GameObject CreateText()
	{
		TextToy text = GameObject.Instantiate(PrefabManager.Text);

		text.TextFormat = Convert.ToString(Properties["Text"]);
		text.DisplaySize = Properties["DisplaySize"].ToVector2() * 20f;

		return text.gameObject;
	}

	private GameObject CreateInteractable(SchematicObject schematicObject)
	{
		InvisibleInteractableToy interactable = GameObject.Instantiate(PrefabManager.Interactable);
		interactable.NetworkShape = (InvisibleInteractableToy.ColliderShape)Convert.ToInt32(Properties["Shape"]);
		interactable.NetworkInteractionDuration = Convert.ToSingle(Properties["InteractionDuration"]);
		interactable.NetworkIsLocked = Properties.TryGetValue("IsLocked", out object isLocked) && Convert.ToBoolean(isLocked);

		if (Properties.TryGetValue("TargetAnimatorId", out object targetIdObj) && Properties.TryGetValue("AnimationStateName", out object stateNameObj))
		{
			int targetId = Convert.ToInt32(targetIdObj);
			string stateName = Convert.ToString(stateNameObj);

			if (targetId != 0 && !string.IsNullOrEmpty(stateName))
			{
				bool isToggled = false;
				Action<ReferenceHub> action = (ReferenceHub hub) =>
				{
					if (ProjectMER.Singleton.Config.debug)
					{
						Player? player = Player.Get(hub.gameObject);
						Logger.Info($"[DEBUG] Player {(player != null ? player.Nickname : "Unknown")} interacted with Interactable block '{Name}' (ID: {ObjectId}).");
					}

					if (schematicObject.ObjectFromId.TryGetValue(targetId, out Transform targetTransform))
					{
						Animator animator = targetTransform.gameObject.GetComponent<Animator>() ?? 
											targetTransform.gameObject.GetComponentInChildren<Animator>() ?? 
											targetTransform.gameObject.GetComponentInParent<Animator>();

						if (animator != null)
						{
							string stateToPlay = stateName;
							if (Properties.TryGetValue("AnimationStateName2", out object stateName2Obj) && !string.IsNullOrEmpty(Convert.ToString(stateName2Obj)))
							{
								string stateName2 = Convert.ToString(stateName2Obj);
								stateToPlay = isToggled ? stateName : stateName2;
								isToggled = !isToggled;
							}

							if (ProjectMER.Singleton.Config.debug)
							{
								Logger.Info($"[DEBUG] Playing animation state '{stateToPlay}' on Animator '{animator.name}' (Target: '{targetTransform.name}', ID: {targetId}).");
							}

							animator.Play(stateToPlay);
						}
						else if (ProjectMER.Singleton.Config.debug)
						{
							Logger.Info($"[DEBUG] Animator NOT found on target '{targetTransform.name}' (ID: {targetId}) or its parents/children.");
						}
					}
					else if (ProjectMER.Singleton.Config.debug)
					{
						Logger.Info($"[DEBUG] Target object ID {targetId} NOT found in schematic's ObjectFromId dictionary.");
					}
				};

				interactable.OnInteracted += action;
				interactable.OnSearched += action;
			}
		}

		return interactable.gameObject;
	}

	private GameObject CreateWaypoint()
	{
		WaypointToy waypoint = GameObject.Instantiate(PrefabManager.Waypoint);

		if (Properties != null && Properties.TryGetValue("Priority", out object priorityObj))
		{
			int val = Convert.ToInt32(priorityObj);
			waypoint.NetworkPriority = (byte)Math.Max(0, Math.Min(255, val));
		}
		else
		{
			waypoint.NetworkPriority = byte.MaxValue; // backwards compat: default 255
		}

		return waypoint.gameObject;
	}
}
