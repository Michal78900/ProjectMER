using AdminToys;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using MapGeneration.Distributors;
using Mirror;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using UnityEngine;
namespace ProjectMER.Features.Serializable;
using ClutterType = Enums.ClutterType;
public class SerializeableClutter : SerializableObject
{
    public ClutterType ClutterType { get; set; } = ClutterType.TankSupportedShelf;

    
    public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
    {
	    GameObject clutter;
	    Vector3 position = room.GetAbsolutePosition(Position);
        Quaternion rotation = room.GetAbsoluteRotation(Rotation);
        
        
        if (instance == null)
        {
	        clutter = GameObject.Instantiate(Prefab);
        }
        else
        {
	        clutter = instance.gameObject;
        }

        
        _prevIndex = Index;
        _prevType = ClutterType;

        clutter.transform.SetPositionAndRotation(position, rotation);
        clutter.transform.localScale = Scale;
            
        if (clutter.TryGetComponent(out StructurePositionSync structurePositionSync))
        { 
            structurePositionSync.Network_position = clutter.transform.position;
            structurePositionSync.Network_rotationY =
                (sbyte)Mathf.RoundToInt(clutter.transform.rotation.eulerAngles.y / 5.625f);
        }

        NetworkServer.UnSpawn(clutter);
        NetworkServer.Spawn(clutter);
        return clutter;
    }
    internal ClutterType _prevType;

    private GameObject Prefab
	{
		get
		{
			GameObject prefab = ClutterType switch
			{
				ClutterType.Fence => PrefabManager.Fence,
				ClutterType.LadderBoxes => PrefabManager.LadderBoxes,
				ClutterType.ShortPipes => PrefabManager.ShortPipes,
				ClutterType.SimpleBoxes => PrefabManager.SimpleBoxes,
				ClutterType.HugePipes => PrefabManager.HugePipes,
				ClutterType.LongPipes => PrefabManager.LongPipes,
				ClutterType.TankSupportedShelf => PrefabManager.TankSupportedShelf,
				_ => throw new InvalidOperationException(),
			};

			return prefab;
		}
	}
}





