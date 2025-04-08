using LabApi.Features.Wrappers;
using MonoMod.Utils;
using NorthwoodLib.Pools;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable.Schematics;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace ProjectMER.Features.Serializable;

public class MapSchematic
{
	public MapSchematic() { }

	public MapSchematic(string mapName)
	{
		Name = mapName;
	}

	public string Name;

	public Dictionary<string, SerializablePrimitive> Primitives { get; set; } = [];

	public Dictionary<string, SerializableLight> Lights { get; set; } = [];

	public Dictionary<string, SerializableDoor> Doors { get; set; } = [];

	public Dictionary<string, SerializableWorkstation> Workstations { get; set; } = [];

	public Dictionary<string, SerializablePlayerSpawnpoint> PlayerSpawnpoints { get; set; } = [];

	public Dictionary<string, SerializableCapybara> Capybaras { get; set; } = [];

	public Dictionary<string, SerializableSchematic> Schematics { get; set; } = [];

	public List<MapEditorObject> SpawnedObjects = [];

	public MapSchematic Merge(MapSchematic other)
	{
		Primitives.AddRange(other.Primitives);
		Lights.AddRange(other.Lights);
		Doors.AddRange(other.Doors);
		Workstations.AddRange(other.Workstations);
		PlayerSpawnpoints.AddRange(other.PlayerSpawnpoints);
		Capybaras.AddRange(other.Capybaras);
		Schematics.AddRange(other.Schematics);

		return this;
	}

	public void Reload()
	{
		foreach (MapEditorObject mapEditorObject in SpawnedObjects)
			mapEditorObject.Destroy();

		SpawnedObjects.Clear();

		Primitives.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Lights.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Doors.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Workstations.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		PlayerSpawnpoints.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Capybaras.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
		Schematics.ForEach(kVP => SpawnObject(kVP.Key, kVP.Value));
	}

	public void SpawnObject<T>(string id, T serializableObject) where T : SerializableObject
	{
		List<Room> rooms;
		if (serializableObject is SerializableSchematic serializableSchematic)
		{
			if (!MapUtils.TryGetSchematicDataByName(serializableSchematic.SchematicName, out SchematicObjectDataList data))
				return;

			rooms = serializableObject.GetRooms();
			foreach (Room room in rooms)
			{
				if (serializableObject.Index < 0 || serializableObject.Index == room.GetRoomIndex())
				{
					GameObject gameObject = serializableObject.SpawnOrUpdateObject(room);
					SchematicObject schematicObject = gameObject.AddComponent<SchematicObject>().Init(serializableSchematic, data, Name, id, room);
					SpawnedObjects.Add(schematicObject);
				}
			}

			ListPool<Room>.Shared.Return(rooms);
			return;
		}

		rooms = serializableObject.GetRooms();
		foreach (Room room in rooms)
		{
			if (serializableObject.Index < 0 || serializableObject.Index == room.GetRoomIndex())
			{
				GameObject gameObject = serializableObject.SpawnOrUpdateObject(room);
				MapEditorObject mapEditorObject = gameObject.AddComponent<MapEditorObject>().Init(serializableObject, Name, id, room);
				SpawnedObjects.Add(mapEditorObject);
			}
		}

		ListPool<Room>.Shared.Return(rooms);
	}

	public void DestroyObject(string id)
	{
		foreach (MapEditorObject mapEditorObject in SpawnedObjects.ToList())
		{
			if (mapEditorObject.Id != id)
				continue;

			SpawnedObjects.Remove(mapEditorObject);
			mapEditorObject.Destroy();
		}
	}

	public bool TryAddElement<T>(string id, T serializableObject) where T : SerializableObject
	{
		if (Primitives.TryAdd(id, serializableObject))
			return true;

		if (Lights.TryAdd(id, serializableObject))
			return true;

		if (Doors.TryAdd(id, serializableObject))
			return true;

		if (Workstations.TryAdd(id, serializableObject))
			return true;

		if (PlayerSpawnpoints.TryAdd(id, serializableObject))
			return true;

		if (Capybaras.TryAdd(id, serializableObject))
			return true;

		if (Schematics.TryAdd(id, serializableObject))
			return true;

		return false;
	}

	public bool TryRemoveElement(string id)
	{
		if (Primitives.Remove(id))
			return true;

		if (Lights.Remove(id))
			return true;

		if (Doors.Remove(id))
			return true;

		if (Workstations.Remove(id))
			return true;

		if (PlayerSpawnpoints.Remove(id))
			return true;

		if (Capybaras.Remove(id))
			return true;

		if (Schematics.Remove(id))
			return true;

		return false;
	}
}
