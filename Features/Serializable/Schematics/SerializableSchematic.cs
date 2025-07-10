using AdminToys;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Objects;
using UnityEngine;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace ProjectMER.Features.Serializable.Schematics;

public class SerializableSchematic : SerializableObject
{
	public string SchematicName { get; set; } = "None";

	public override GameObject? SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		if (Data == null)
			return null;

		PrimitiveObjectToy schematic = instance == null ? UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObject) : instance.GetComponent<PrimitiveObjectToy>();
		schematic.NetworkPrimitiveFlags = PrimitiveFlags.None;
		schematic.NetworkMovementSmoothing = 60;

		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		schematic.name = $"CustomSchematic-{SchematicName}";
		schematic.transform.SetPositionAndRotation(position, rotation);
		schematic.transform.localScale = Scale;

		if (instance == null)
		{
			NetworkServer.Spawn(schematic.gameObject);
			schematic.gameObject.AddComponent<SchematicObject>().Init(Data);
		}

		return schematic.gameObject;
  }
  public SchematicObjectDataList? Data
  {
    get
    {
      return _data ??= MapUtils.TryGetSchematicDataByName(SchematicName, out SchematicObjectDataList data) ? data : null;
    }
    set
    {
      _data = value;
    }
  }

	private SchematicObjectDataList? _data;
}
