using AdminToys;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using ProjectMER.Features.Serializable;
using UnityEngine;

namespace ProjectMER.Features.Objects;

public class MapEditorObject : MonoBehaviour
{
	public SerializableObject Base;

	public string MapName { get; protected set; }

	public string Id { get; protected set; }

	public Room Room { get; protected set; }

	public virtual MapEditorObject Init(SerializableObject serializableObject, string mapName, string id, Room room)
	{
		Base = serializableObject;
		MapName = mapName;
		Id = id;
		Room = room;

		return this;
	}

	public virtual void UpdateObjectAndCopies()
	{
		if (Base.RequiresReloading)
		{
			Base._prevIndex = Base.Index;
			
			if (MapUtils.LoadedMaps.TryGetValue(MapName, out MapSchematic map))
			{
				map.DestroyObject(Id);
			}

			Timing.CallDelayed(0.1f, () => map.SpawnObject(Id, Base));
			return;
		}

		foreach (MapEditorObject copy in MapUtils.LoadedMaps[MapName].SpawnedObjects.ToList())
		{
			if (copy.Id != Id)
				continue;

			copy.UpdateCopy();
		}
	}

	private void UpdateCopy()
	{
		IndicatorObject.TrySpawnOrUpdateIndicator(this);
		Base.SpawnOrUpdateObject(Room, gameObject);
	}

	/// <summary>
	/// Destroys the object.
	/// </summary>
	public void Destroy()
	{
		IndicatorObject.TryDestroyIndicator(this);
		Destroy(gameObject);
	}
}
