using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Features.Wrappers;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using ProjectMER.Features.Serializable.Lockers;
using ProjectMER.Features.Serializable.Schematics;
using UserSettings.ServerSpecific;

namespace ProjectMER.Features.ToolGun;

public class ToolGunItem
{
	public static Dictionary<ushort, ToolGunItem> ItemDictionary { get; private set; } = [];

	public static Dictionary<ToolGunObjectType, Type> TypesDictionary { get; private set; } = new()
	{
		{ ToolGunObjectType.Primitive, typeof(SerializablePrimitive) },
		{ ToolGunObjectType.Light, typeof(SerializableLight) },
		{ ToolGunObjectType.Door, typeof(SerializableDoor) },
		{ ToolGunObjectType.Workstation, typeof(SerializableWorkstation) },
		{ ToolGunObjectType.ItemSpawnpoint, typeof(SerializableItemSpawnpoint)},
		{ ToolGunObjectType.PlayerSpawnpoint, typeof(SerializablePlayerSpawnpoint) },
		{ ToolGunObjectType.Capybara, typeof(SerializableCapybara) },
		{ ToolGunObjectType.Text, typeof(SerializableText)},
		{ ToolGunObjectType.Schematic, typeof(SerializableSchematic) },
		{ ToolGunObjectType.Scp079Camera, typeof(SerializableScp079Camera) },
		{ ToolGunObjectType.ShootingTarget, typeof(SerializableShootingTarget) },
		{ ToolGunObjectType.Locker, typeof(SerializableLocker) },
		{ ToolGunObjectType.Teleport, typeof(SerializableTeleport) },
		{ ToolGunObjectType.Interactable, typeof(SerializableInteractable) },
		{ ToolGunObjectType.Waypoint, typeof(SerializableWaypoint) },
	};

	private ToolGunObjectType _selectedObjectToSpawn;
	public ToolGunObjectType SelectedObjectToSpawn
	{
		get => _selectedObjectToSpawn;
		set
		{
			_selectedObjectToSpawn = value;
			if (TypesDictionary.Count <= (int)_selectedObjectToSpawn)
			{
				_selectedObjectToSpawn = 0;
				return;
			}

			if (_selectedObjectToSpawn < 0)
			{
				_selectedObjectToSpawn = (ToolGunObjectType)(TypesDictionary.Count - 1);
				return;
			}
		}
	}

	public static bool TryAdd(Player player)
	{
		Item? item = player.AddItem(ItemType.GunCOM18);
		if (item == null)
			return false;

		Firearm toolgun = (Firearm)item.Base;
		toolgun.ApplyAttachmentsCode(454, false);
		if (!toolgun.TryGetModules(out MagazineModule magazineModule, out AutomaticActionModule automaticActionModule))
		{
			Logger.Error("Modules not found. This error should never occur.");
			return false;
		}

		magazineModule.AmmoStored = 0;
		magazineModule.ServerResyncData();

		automaticActionModule.Cocked = true;
		automaticActionModule.ServerResync();

		player.AddAmmo(ItemType.Ammo9x19, 1);

		ItemDictionary.Add(toolgun.ItemSerial, new ToolGunItem(toolgun));

		ServerSpecificSettingsSync.SendOnJoinFilter = (_) => false; // Prevent all users from receiving the tools after joining the server.
		ServerSpecificSettingsSync.DefinedSettings =
		[
			new SSGroupHeader("MapEditorReborn"),
			new SSDropdownSetting(0, "Schematic Name", MapUtils.GetAvailableSchematicNames(), isServerOnly: true)
		];

		ServerSpecificSettingsSync.SendToPlayersConditionally(x => x.inventory.UserInventory.Items.Values.Any(x => x.IsToolGun(out ToolGunItem _)));

		return true;
	}

	public static bool Remove(Player player)
	{
		foreach (ItemBase itemBase in player.Inventory.UserInventory.Items.Values)
		{
			if (ItemDictionary.ContainsKey(itemBase.ItemSerial))
			{
				ItemDictionary.Remove(itemBase.ItemSerial);
				player.RemoveItem(itemBase);
				return true;
			}
		}

		return false;
	}

	public bool CreateMode => Firearm.IsEmittingLight && !AdsModule.AdsTarget;
	public bool DeleteMode => !Firearm.IsEmittingLight && !AdsModule.AdsTarget;
	public bool SelectMode => Firearm.IsEmittingLight && AdsModule.AdsTarget;

	public void Shot(Player player)
	{
		if (CreateMode)
		{
			ServerSpecificSettingsSync.TryGetSettingOfUser(player.ReferenceHub, 0, out SSDropdownSetting dropdownSetting);
			dropdownSetting.TryGetSyncSelectionText(out string schematicName);

			ToolGunHandler.CreateObject(player, SelectedObjectToSpawn, schematicName);
			return;
		}

		if (ToolGunHandler.TryGetMapObject(player, out MapEditorObject mapEditorObject) && DeleteMode)
		{
			ToolGunHandler.DeleteObject(mapEditorObject);
			return;
		}

		if (SelectMode)
			ToolGunHandler.SelectObject(player, mapEditorObject);
	}

	private ToolGunItem(Firearm firearm)
	{
		Firearm = firearm;
		if (!firearm.TryGetModule(out AdsModule))
		{
			throw new Exception("Module not found. This error should never occur.");
		}
	}

	private readonly Firearm Firearm;
	private readonly IAdsModule AdsModule;
}
