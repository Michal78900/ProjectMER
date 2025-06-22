using Interactables.Interobjects.DoorUtils;

namespace ProjectMER.Features.Serializable.Lockers;

public class SerializableLockerChamber
{
	public SerializableLockerChamber() { }

	public SerializableLockerChamber(ItemType[] acceptableItems, bool isOpen, DoorPermissionFlags requiredPermissions)
	{
		AcceptableItems = acceptableItems.ToList();
		IsOpen = isOpen;
		RequiredPermissions = requiredPermissions;
	}

	public List<ItemType> AcceptableItems { get; set; }

	public bool IsOpen { get; set; }

	public DoorPermissionFlags RequiredPermissions { get; set; }
}
