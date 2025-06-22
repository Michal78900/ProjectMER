using InventorySystem.Items;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using ProjectMER.Features.Objects;
using FirearmPickup = LabApi.Features.Wrappers.FirearmPickup;

namespace ProjectMER.Events.Handlers.Internal;

public class PickupEventsHandler : CustomEventsHandler
{
	internal static readonly Dictionary<ushort, SchematicObject> ButtonPickups = [];
	internal static readonly Dictionary<ushort, int> PickupUsesLeft = [];

	public override void OnPlayerSearchingPickup(PlayerSearchingPickupEventArgs ev)
	{
		if (!ButtonPickups.TryGetValue(ev.Pickup.Serial, out SchematicObject schematic))
			return;

		ev.IsAllowed = false;
		Schematic.OnButtonInteracted(new(ev.Pickup, ev.Player, schematic));
	}

	public override void OnPlayerPickingUpItem(PlayerPickingUpItemEventArgs ev)
	{
		if (!ev.Pickup.Transform.TryGetComponentInParent(out MapEditorObject _))
			return;

		if (!PickupUsesLeft.ContainsKey(ev.Pickup.Serial))
			return;

		if (--PickupUsesLeft[ev.Pickup.Serial] == 0)
		{
			PickupUsesLeft.Remove(ev.Pickup.Serial);
			return;
		}

		ev.IsAllowed = false;
		ev.Pickup.IsInUse = false;

		Item item = ev.Player.AddItem(ev.Pickup.Type, ItemAddReason.PickedUp)!;
		if (ev.Pickup is not FirearmPickup firearmPickup || item is not FirearmItem firearmItem)
			return;

		firearmItem.Base.ApplyAttachmentsCode(firearmPickup.AttachmentCode, false);
		if (firearmItem.Base.TryGetModule(out MagazineModule magazineModule))
		{
			magazineModule.MagazineInserted = true;
			magazineModule.AmmoStored = magazineModule.AmmoMax;
			magazineModule.ServerResyncData();
		}
		else if (firearmItem.Base.TryGetModule(out CylinderAmmoModule cylinderAmmoModule))
		{
			cylinderAmmoModule.ServerModifyAmmo(cylinderAmmoModule.AmmoMax);
			cylinderAmmoModule.ServerResync();
		}
	}

	public override void OnPlayerPickingUpAmmo(PlayerPickingUpAmmoEventArgs ev)
	{
		if (!ev.Pickup.Transform.TryGetComponentInParent(out MapEditorObject _))
			return;

		if (!PickupUsesLeft.ContainsKey(ev.Pickup.Serial))
			return;

		if (--PickupUsesLeft[ev.Pickup.Serial] == 0)
		{
			PickupUsesLeft.Remove(ev.Pickup.Serial);
			return;
		}

		ev.IsAllowed = false;
		ev.Pickup.IsInUse = false;
		ev.Player.AddAmmo(ev.AmmoType, ev.AmmoAmount);
	}
}
