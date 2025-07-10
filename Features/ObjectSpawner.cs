using AdminToys;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using ProjectMER.Features.Serializable.Schematics;
using UnityEngine;

namespace ProjectMER.Features;

public static class ObjectSpawner
{
	public static PrimitiveObjectToy SpawnPrimitive(SerializablePrimitive serializablePrimitive)
	{
		GameObject gameObject = serializablePrimitive.SpawnOrUpdateObject();
		return gameObject.GetComponent<PrimitiveObjectToy>();
	}

	public static SchematicObject SpawnSchematic(SerializableSchematic serializableSchematic)
	{
		GameObject? gameObject = serializableSchematic.SpawnOrUpdateObject();
		if (gameObject == null)
			return null!;

		return gameObject.GetComponent<SchematicObject>();
	}

	public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, SchematicObjectDataList? data = null) =>
		SpawnSchematic(new() { SchematicName = schematicName, Position = position, Data = data });

	public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Quaternion rotation, SchematicObjectDataList? data = null) =>
		SpawnSchematic(new() { SchematicName = schematicName, Position = position, Rotation = rotation.eulerAngles, Data = data });

	public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Vector3 eulerAngles, SchematicObjectDataList? data = null) =>
		SpawnSchematic(new() { SchematicName = schematicName, Position = position, Rotation = eulerAngles, Data = data });

	public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Quaternion rotation, Vector3 scale, SchematicObjectDataList? data = null) =>
		SpawnSchematic(new() { SchematicName = schematicName, Position = position, Rotation = rotation.eulerAngles, Scale = scale, Data = data });

	public static SchematicObject SpawnSchematic(string schematicName, Vector3 position, Vector3 eulerAngles, Vector3 scale, SchematicObjectDataList? data = null) =>
		SpawnSchematic(new() { SchematicName = schematicName, Position = position, Rotation = eulerAngles, Scale = scale, Data = data });

	public static bool TrySpawnSchematic(SerializableSchematic serializableSchematic, out SchematicObject schematic)
	{
		try
		{
			schematic = SpawnSchematic(serializableSchematic);
			return schematic != null;
		}
		catch (Exception)
		{
			schematic = null!;
			return false;
		}
	}

	public static bool TrySpawnSchematic(string schematicName, Vector3 position, out SchematicObject schematic, SchematicObjectDataList? data = null) =>
		TrySpawnSchematic(new() { SchematicName = schematicName, Position = position, Data = data }, out schematic);

	public static bool TrySpawnSchematic(string schematicName, Vector3 position, Quaternion rotation, out SchematicObject schematic, SchematicObjectDataList? data = null) =>
		TrySpawnSchematic(new() { SchematicName = schematicName, Position = position, Rotation = rotation.eulerAngles, Data = data }, out schematic);

	public static bool TrySpawnSchematic(string schematicName, Vector3 position, Vector3 eulerAngles, out SchematicObject schematic, SchematicObjectDataList? data = null) =>
		TrySpawnSchematic(new() { SchematicName = schematicName, Position = position, Rotation = eulerAngles, Data = data }, out schematic);

	public static bool TrySpawnSchematic(string schematicName, Vector3 position, Quaternion rotation, Vector3 scale, out SchematicObject schematic, SchematicObjectDataList? data = null) =>
		TrySpawnSchematic(new() { SchematicName = schematicName, Position = position, Rotation = rotation.eulerAngles, Scale = scale, Data = data }, out schematic);

	public static bool TrySpawnSchematic(string schematicName, Vector3 position, Vector3 eulerAngles, Vector3 scale, out SchematicObject schematic, SchematicObjectDataList? data = null) =>
		TrySpawnSchematic(new() { SchematicName = schematicName, Position = position, Rotation = eulerAngles, Scale = scale, Data = data }, out schematic);
}
