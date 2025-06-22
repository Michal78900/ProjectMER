using AdminToys;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Interfaces;
using UnityEngine;
using YamlDotNet.Serialization;
using LightSourceToy = AdminToys.LightSourceToy;
using PrimitiveObjectToy = AdminToys.PrimitiveObjectToy;

namespace ProjectMER.Features.Serializable;

public class SerializableLight : SerializableObject, IIndicatorDefinition
{
	public string Color { get; set; } = "white";

	public float Intensity { get; set; } = 1f;

	public float Range { get; set; } = 1f;

	public LightShadows Shadows { get; set; } = LightShadows.Hard;

	public float Strength { get; set; } = 0f;

	public LightType LightType { get; set; } = LightType.Point;

	public LightShape Shape { get; set; } = LightShape.Cone;

	public float SpotAngle { get; set; } = 30f;

	public float InnerSpotAngle { get; set; } = 0f;

	[YamlIgnore]
	public override Vector3 Scale { get; set; }

	public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
	{
		LightSourceToy light = instance == null ? UnityEngine.Object.Instantiate(PrefabManager.LightSource) : instance.GetComponent<LightSourceToy>();
		Vector3 position = room.GetAbsolutePosition(Position);
		Quaternion rotation = room.GetAbsoluteRotation(Rotation);
		_prevIndex = Index;

		light.transform.SetPositionAndRotation(position, rotation);
		light.NetworkMovementSmoothing = 60;

		light.NetworkLightColor = ColorUtility.TryParseHtmlString(Color, out Color color) ? color : UnityEngine.Color.magenta;
		light.NetworkLightIntensity = Intensity;
		light.NetworkLightRange = Range;
		light.NetworkShadowType = Shadows;
		light.NetworkShadowStrength = Strength;
		light.NetworkLightType = LightType;
		light.NetworkLightShape = Shape;
		light.NetworkSpotAngle = SpotAngle;
		light.NetworkInnerSpotAngle = InnerSpotAngle;

		if (instance == null)
			NetworkServer.Spawn(light.gameObject);

		return light.gameObject;
	}

	public GameObject SpawnOrUpdateIndicator(Room room, GameObject? instance = null)
	{
		PrimitiveObjectToy primitive = instance == null ? UnityEngine.Object.Instantiate(PrefabManager.PrimitiveObject) : instance.GetComponent<PrimitiveObjectToy>();
		Vector3 position = room.GetAbsolutePosition(Position);

		primitive.transform.position = position;
		primitive.NetworkPrimitiveType = PrimitiveType.Sphere;
		primitive.NetworkPrimitiveFlags = PrimitiveFlags.Visible;
		primitive.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

		_ = ColorUtility.TryParseHtmlString(Color, out Color color) ? color : UnityEngine.Color.magenta;
		Color transparentColor = new Color(color.r, color.g, color.b, 0.9f);
		primitive.NetworkMaterialColor = transparentColor;

		return primitive.gameObject;
	}
}
