namespace ProjectMER.Features.Enums;

/// <summary>
/// All available blocks for schematic object.
/// </summary>
public enum BlockType
{
	/// <summary>
	/// Represents an empty transform.
	/// </summary>
	Empty = 0,

	/// <summary>
	/// Represents a primitive.
	/// </summary>
	Primitive = 1,

	/// <summary>
	/// Represents a light.
	/// </summary>
	Light = 2,

	/// <summary>
	/// Represents a pickup.
	/// </summary>
	Pickup = 3,

	/// <summary>
	/// Represents a workstation.
	/// </summary>
	Workstation = 4,

	/// <summary>
	/// Represents a sub-schematic.
	/// </summary>
	Schematic = 5,

	/// <summary>
	/// Represents a teleporter.
	/// </summary>
	Teleport = 6,

	/// <summary>
	/// Represents a locker.
	/// </summary>
	Locker = 7,
	
	/// <summary>
	/// Represents a Interactable.
	/// </summary>
	Interactable = 8,
}
