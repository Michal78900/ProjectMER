using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Interfaces;
using ProjectMER.Features.Serializable;
using UnityEngine;

namespace ProjectMER.Features.Objects;

public class MapEditorObject : MonoBehaviour
{
	public SerializableObject Base;

	public string MapName { get; protected set; }

	public string Id { get; protected set; }

	public Room Room { get; protected set; }

	public MapEditorObject Init(SerializableObject serializableObject, string mapName, string id, Room room)
	{
		Base = serializableObject;
		MapName = mapName;
		Id = id;
		Room = room;

		NetworkServer.Spawn(gameObject);
		return this;
	}

	public virtual void UpdateObjectAndCopies()
	{
		foreach (MapEditorObject copy in MapUtils.LoadedMaps[MapName].SpawnedObjects)
		{
			if (copy.Id != Id)
				continue;

			copy.UpdateCopy();
		}
	}

    private void UpdateCopy()
    {
        Base.SpawnOrUpdateObject(Room, gameObject);

		if (Base is IIndicatorDefinition indicatorDefinition)
			indicatorDefinition.SpawnOrUpdateIndicator(Room, IndicatorObject.Dictionary.First(x => x.Value == this).Key.gameObject);
    }

    public Vector3 RelativePosition
	{
		get => Base.Position.ToVector3();
		set => Base.Position = value.ToString("G");
	}

    /// <summary>
    /// Destroys the object.
    /// </summary>
    public void Destroy()
    {
		if (Base is IIndicatorDefinition _)
		{
            IndicatorObject indicator = IndicatorObject.Dictionary.First(x => x.Value == this).Key;
			IndicatorObject.Dictionary.Remove(indicator);
			indicator.Destroy();
		}

        Destroy(gameObject);
    }
}
