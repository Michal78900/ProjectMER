using AdminToys;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using UnityEngine;

namespace ProjectMER.Features.Serializable;

public class SerializableShootingTarget : SerializableObject
{
	public TargetType TargetType { get; set; } = TargetType.ClassD;
	
	public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		ShootingTarget shootingTarget = instance == null ? UnityEngine.Object.Instantiate(TargetPrefab) : instance.GetComponent<ShootingTarget>();
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);

		shootingTarget.transform.SetPositionAndRotation(position, rotation);
		shootingTarget.transform.localScale = Scale;
		
		_prevType = TargetType;
		
		if (instance == null)
			NetworkServer.Spawn(shootingTarget.gameObject);

		return shootingTarget.gameObject;
	}
	
	private ShootingTarget TargetPrefab
	{
		get
		{
			ShootingTarget prefab = TargetType switch
			{
				TargetType.Binary => PrefabManager.ShootingTargetBinary,
				TargetType.ClassD => PrefabManager.ShootingTargetDBoy,
				TargetType.Sport => PrefabManager.ShootingTargetSport,
				_ => throw new InvalidOperationException(),
			};

			return prefab;
		}
	}
	
	public override bool RequiresReloading => TargetType != _prevType || base.RequiresReloading;

	internal TargetType _prevType;
}
