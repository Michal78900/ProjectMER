
using LabApi.Features.Wrappers;
using UnityEngine;

namespace ProjectMER.Features.Serializable;

public abstract class SerializableObject
{
	/// <summary>
	/// Gets or sets the objects's position.
	/// </summary>
	public virtual string Position { get; set; } = Vector3.zero.ToString("G");

	/// <summary>
	/// Gets or sets the objects's rotation.
	/// </summary>
	public virtual string Rotation { get; set; } = Vector3.zero.ToString("G");

	/// <summary>
	/// Gets or sets the objects's scale.
	/// </summary>
	public virtual string Scale { get; set; } = Vector3.one.ToString("G");

	public virtual string Room { get; set; } = "Unknown";

    public virtual GameObject SpawnOrUpdateObject(Room room, GameObject? instance = null) => throw new NotSupportedException();
}
