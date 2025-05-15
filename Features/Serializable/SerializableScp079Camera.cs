using AdminToys;
using LabApi.Features.Wrappers;
using MapGeneration;
using Mirror;
using ProjectMER.Features.Extensions;
using UnityEngine;
using CameraType = ProjectMER.Features.Enums.CameraType;

namespace ProjectMER.Features.Serializable;

public class SerializableScp079Camera : SerializableObject
{
	public CameraType CameraType { get; set; } = CameraType.Lcz;
	public string Label { get; set; } = "CustomCamera";

	public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		Scp079CameraToy cameraVariant;
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		if (instance == null)
		{
			cameraVariant = GameObject.Instantiate(CameraPrefab);
		}
		else
		{
			cameraVariant = instance.GetComponent<Scp079CameraToy>();
		}

		cameraVariant.transform.SetPositionAndRotation(position, rotation);
		cameraVariant.transform.localScale = Scale;
		cameraVariant.NetworkScale = cameraVariant.transform.localScale;

		_prevIndex = Index;
		_prevType = CameraType;

		cameraVariant.NetworkMovementSmoothing = 60;
		cameraVariant.NetworkLabel = Label;
		cameraVariant.NetworkRoom = room == null ? LabApi.Features.Wrappers.Room.Get(RoomName.Outside).First().Base : room.Base;

		if (instance == null)
			NetworkServer.Spawn(cameraVariant.gameObject);

		return cameraVariant.gameObject;
	}


	private Scp079CameraToy CameraPrefab
	{
		get
		{
			Scp079CameraToy prefab = CameraType switch
			{
				CameraType.Lcz => PrefabManager.LczCameraToy,
				CameraType.Hcz => PrefabManager.HczCameraToy,
				CameraType.Ez => PrefabManager.EzCameraToy,
				CameraType.EzArm => PrefabManager.EzArmCameraToy,
				CameraType.Sz => PrefabManager.SzCameraToy,
				_ => throw new InvalidOperationException(),
			};

			return prefab;
		}
	}

	public override bool RequiresReloading => CameraType != _prevType || base.RequiresReloading;

	internal CameraType _prevType;
}
