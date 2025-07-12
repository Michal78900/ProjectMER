using AdminToys;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Firearms.Attachments;
using MapGeneration.Distributors;
using Mirror;
using UnityEngine;
using LightSourceToy = AdminToys.LightSourceToy;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace ProjectMER.Features;

public static class PrefabManager
{
	public static PrimitiveObjectToy PrimitiveObject { get; private set; }

	public static LightSourceToy LightSource { get; private set; }

	public static DoorVariant DoorLcz { get; private set; }
	public static DoorVariant DoorHcz { get; private set; }
	public static DoorVariant DoorEz { get; private set; }
	public static DoorVariant DoorHeavyBulk { get; private set; }

	public static WorkstationController Workstation { get; private set; }

	public static CapybaraToy Capybara { get; private set; }

	public static TextToy Text { get; private set; }

	public static Scp079CameraToy CameraLcz { get; private set; }
	public static Scp079CameraToy CameraHcz { get; private set; }
	public static Scp079CameraToy CameraSz { get; private set; }
	public static Scp079CameraToy CameraEzArm { get; private set; }
	public static Scp079CameraToy CameraEz { get; private set; }

	public static ShootingTarget ShootingTargetSport { get; private set; }
	public static ShootingTarget ShootingTargetDBoy { get; private set; }
	public static ShootingTarget ShootingTargetBinary { get; private set; }
	
	public static Scp079Generator Generator { get; private set; }
	public static Locker PedestalScp018 { get; private set; }
	public static Locker PedstalScp207 { get; private set; }
	public static Locker PedestalScp244 { get; private set; }
	public static Locker PedestalScp268 { get; private set; }
	public static Locker LockerLargeGun { get; private set; }
	public static Locker LockerRifleRack { get; private set; }
	public static Locker LockerMisc { get; private set; }
	public static Locker LockerRegularMedkit { get; private set; }
	public static Locker LockerAdrenalineMedkit { get; private set; }
	public static Locker PedestalScp500 { get; private set; }
	public static Locker PedstalScp1853 { get; private set; }
	public static Locker PedestalScp2176 { get; private set; }
	public static Locker PedestalScp1576 { get; private set; }
	public static Locker PedestalAntiScp207 { get; private set; }
	public static Locker PedestalScp1344 { get; private set; }
	public static Locker LockerExperimentalWeapon { get; private set; }

	public static void RegisterPrefabs()
	{
		foreach (GameObject gameObject in NetworkClient.prefabs.Values)
		{
			if (gameObject.TryGetComponent(out PrimitiveObjectToy primitiveObjectToy))
			{
				PrimitiveObject = primitiveObjectToy;
				continue;
			}

			if (gameObject.TryGetComponent(out LightSourceToy lightSourceToy))
			{
				LightSource = lightSourceToy;
				continue;
			}

			if (gameObject.TryGetComponent(out DoorVariant doorVariant))
			{
				switch (gameObject.name)
				{
					case "LCZ BreakableDoor":
						DoorLcz = doorVariant;
						continue;
					case "HCZ BreakableDoor":
						DoorHcz = doorVariant;
						continue;
					case "EZ BreakableDoor":
						DoorEz = doorVariant;
						continue;
					case "HCZ BulkDoor":
						DoorHeavyBulk = doorVariant;
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
				Workstation = workstationController;
				continue;
			}

			if (gameObject.TryGetComponent(out CapybaraToy capybaraToy))
			{
				Capybara = capybaraToy;
				continue;
			}
			
			if (gameObject.TryGetComponent(out Scp079Generator generator))
			{
				Generator = generator;
				continue;
			}

			if (gameObject.TryGetComponent(out Scp079CameraToy cameraToy))
			{
				switch (gameObject.name)
				{
					case "LczCameraToy":
						CameraLcz = cameraToy;
						continue;
					case "HczCameraToy":
						CameraHcz = cameraToy;
						continue;
					case "SzCameraToy":
						CameraSz = cameraToy;
						continue;
					case "EzArmCameraToy":
						CameraEzArm = cameraToy;
						continue;
					case "EzCameraToy":
						CameraEz = cameraToy;
						continue;
				}
			}

			if (gameObject.TryGetComponent(out TextToy textToy))
			{
				Text = textToy;
				continue;
			}

			if (gameObject.TryGetComponent(out Locker locker))
			{
				switch (gameObject.name)
				{
					case "Scp018PedestalStructure Variant":
						PedestalScp018 = locker;
						continue;
					case "Scp207PedestalStructure Variant":
						PedstalScp207 = locker;
						continue;
					case "Scp244PedestalStructure Variant":
						PedestalScp244 = locker;
						continue;
					case "Scp268PedestalStructure Variant":
						PedestalScp268 = locker;
						continue;
					case "LargeGunLockerStructure":
						LockerLargeGun = locker;
						continue;
					case "RifleRackStructure":
						LockerRifleRack = locker;
						continue;
					case "MiscLocker":
						LockerMisc = locker;
						continue;
					case "RegularMedkitStructure":
						LockerRegularMedkit = locker;
						continue;
					case "AdrenalineMedkitStructure":
						LockerAdrenalineMedkit = locker;
						continue;
					case "Scp500PedestalStructure Variant":
						PedestalScp500 = locker;
						continue;
					case "Scp1853PedestalStructure Variant":
						PedstalScp1853 = locker;
						continue;
					case "Scp2176PedestalStructure Variant":
						PedestalScp2176 = locker;
						continue;
					case "Scp1576PedestalStructure Variant":
						PedestalScp1576 = locker;
						continue;
					case "AntiScp207PedestalStructure Variant":
						PedestalAntiScp207 = locker;
						continue;
					case "Scp1344PedestalStructure Variant":
						PedestalScp1344 = locker;
						continue;
					case "Experimental Weapon Locker":
						LockerExperimentalWeapon = locker;
						continue;
				}
			}
		}
	}
}
