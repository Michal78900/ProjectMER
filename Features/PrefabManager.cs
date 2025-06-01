using AdminToys;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using UnityEngine;

namespace ProjectMER.Features;

public static class PrefabManager
{
	public static PrimitiveObjectToy PrimitiveObjectPrefab { get; private set; }

	public static LightSourceToy LightSourcePrefab { get; private set; }

	public static DoorVariant LczDoorPrefab { get; private set; }
	public static DoorVariant HczDoorPrefab { get; private set; }
	public static DoorVariant EzDoorPrefab { get; private set; }
	public static DoorVariant BulkDoorPrefab { get; private set; }

	public static WorkstationController WorkstationPrefab { get; private set; }

	public static CapybaraToy CapybaraPrefab { get; private set; }
		
	public static TextToy TextPrefab {get; private set;}

	public static Scp079CameraToy LczCameraToy { get; private set; }
	public static Scp079CameraToy HczCameraToy { get; private set; }
	public static Scp079CameraToy SzCameraToy { get; private set; }
	public static Scp079CameraToy EzArmCameraToy { get; private set; }
	public static Scp079CameraToy EzCameraToy { get; private set; }
	
	public static ShootingTarget ShootingTargetSport { get; private set; }
	public static ShootingTarget ShootingTargetDBoy { get; private set; }
	public static ShootingTarget ShootingTargetBinary { get; private set; }

	public static void RegisterPrefabs()
	{
		foreach (GameObject gameObject in NetworkClient.prefabs.Values)
		{
			if (gameObject.TryGetComponent(out PrimitiveObjectToy primitiveObjectToy))
			{
				PrimitiveObjectPrefab = primitiveObjectToy;
				continue;
			}

			if (gameObject.TryGetComponent(out LightSourceToy lightSourceToy))
			{
				LightSourcePrefab = lightSourceToy;
				continue;
			}

			if (gameObject.TryGetComponent(out DoorVariant doorVariant))
			{
				switch (gameObject.name)
				{
					case "LCZ BreakableDoor":
						LczDoorPrefab = doorVariant;
						continue;
					case "HCZ BreakableDoor":
						HczDoorPrefab = doorVariant;
						continue;
					case "EZ BreakableDoor":
						EzDoorPrefab = doorVariant;
						continue;
					case "HCZ BulkDoor":
						BulkDoorPrefab = doorVariant;
						continue;
				}
			}
			
			if (gameObject.TryGetComponent(out ShootingTarget shootingTarget))
			{
				switch (gameObject.name)
				{
					case "sportTargetPrefab":
						ShootingTargetSport = shootingTarget;
						continue;
					case "dboyTargetPrefab":
						ShootingTargetDBoy = shootingTarget;
						continue;
					case "binaryTargetPrefab":
						ShootingTargetBinary = shootingTarget;
						continue;
				}
			}

			if (gameObject.TryGetComponent(out WorkstationController workstationController))
			{
				WorkstationPrefab = workstationController;
				continue;
			}

			if (gameObject.TryGetComponent(out CapybaraToy capybaraToy))
			{
				CapybaraPrefab = capybaraToy;
				continue;
			}
			
			if (gameObject.TryGetComponent(out Scp079CameraToy cameraToy))
			{
				switch (gameObject.name)
				{
					case "LczCameraToy":
						LczCameraToy = cameraToy;
						continue;
					case "HczCameraToy":
						HczCameraToy = cameraToy;
						continue;
					case "SzCameraToy":
						SzCameraToy = cameraToy;
						continue;
					case "EzArmCameraToy":
						EzArmCameraToy = cameraToy;
						continue;
					case "EzCameraToy":
						EzCameraToy = cameraToy;
						continue;
				}
			}

			if (gameObject.TryGetComponent(out TextToy textToy))
			{
				TextPrefab = textToy;
				continue;
			}
		}
	}
}
