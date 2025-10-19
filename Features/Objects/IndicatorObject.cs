using AdminToys;
using Mirror;
using NorthwoodLib.Pools;
using ProjectMER.Features.Interfaces;
using ProjectMER.Features.Serializable;
using UnityEngine;

namespace ProjectMER.Features.Objects;

public class IndicatorObject : MapEditorObject
{
	public static Dictionary<IndicatorObject, MapEditorObject> Dictionary = [];

	public static bool TrySpawnOrUpdateIndicator(MapEditorObject mapEditorObject)
	{
		if (mapEditorObject.Base is not IIndicatorDefinition indicatorDefinition)
			return false;

		if (TryGetIndicator(mapEditorObject, out IndicatorObject indicator))
		{
			indicatorDefinition.SpawnOrUpdateIndicator(mapEditorObject.Room, indicator.gameObject);
		}
		else
		{
			indicator = indicatorDefinition.SpawnOrUpdateIndicator(mapEditorObject.Room).AddComponent<IndicatorObject>();
			BoxCollider collider = indicator.gameObject.AddComponent<BoxCollider>();
			collider.isTrigger = true;
			indicator.GetComponentsInChildren<NetworkIdentity>().ForEach(x => NetworkServer.Spawn(x.gameObject));
			Dictionary.Add(indicator, mapEditorObject);
		}

		if (Dictionary[indicator].TryGetComponent(out WaypointToy waypoint))
		{
			waypoint.NetworkVisualizeBounds = true;
		}

		return true;
	}

	public static bool TryGetIndicator(MapEditorObject mapEditorObject, out IndicatorObject indicator)
	{
		indicator = null!;
		if (mapEditorObject.Base is not IIndicatorDefinition _)
			return false;

		if (!Dictionary.ContainsValue(mapEditorObject))
			return false;

		indicator = Dictionary.First(x => x.Value == mapEditorObject).Key;
		return true;
	}

	public static bool TryDestroyIndicator(MapEditorObject mapEditorObject)
	{
		if (!TryGetIndicator(mapEditorObject, out IndicatorObject indicator))
			return false;

		if (Dictionary[indicator].TryGetComponent(out WaypointToy waypoint))
		{
			waypoint.NetworkVisualizeBounds = false;
		}

		Dictionary.Remove(indicator);
		indicator.Destroy();
		return true;
	}

	public static void ClearIndicators()
	{
		List<MapEditorObject> values = ListPool<MapEditorObject>.Shared.Rent(Dictionary.Values);
		foreach (MapEditorObject mapEditorObject in values)
			TryDestroyIndicator(mapEditorObject);

		ListPool<MapEditorObject>.Shared.Return(values);
		Dictionary.Clear();
	}

	public static void RefreshIndicators()
	{
		ClearIndicators();

		foreach (MapSchematic map in MapUtils.LoadedMaps.Values)
		{
			foreach (MapEditorObject mapEditorObject in map.SpawnedObjects)
			{
				TrySpawnOrUpdateIndicator(mapEditorObject);
			}
		}
	}

	public void Update() => transform.position = Dictionary[this].transform.position;
}
