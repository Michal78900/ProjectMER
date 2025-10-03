using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using MapGeneration.RoomConnectors;
using MapGeneration.RoomConnectors.Spawners;
using Mirror;
using ProjectMER.Commands.Modifying.Scale;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using UnityEngine;

namespace ProjectMER.Features.Serializable;

public class SerializableClutter : SerializableObject
{
    public SpawnableRoomConnectorType ConnectorType { get; set; } = SpawnableRoomConnectorType.ClutterSimpleBoxes;

    public override GameObject SpawnOrUpdateObject(Room? room = null, GameObject? instance = null)
    {
        SpawnableRoomConnector connectorSpawnpoint;
        Vector3 position = room.GetAbsolutePosition(Position);
        Quaternion rotation = room.GetAbsoluteRotation(Rotation);
        _prevIndex = Index;

        if (instance == null)
        {
            connectorSpawnpoint = GameObject.Instantiate(ClutterPrefab);
        }
        else
        {
            connectorSpawnpoint = instance.GetComponent<SpawnableRoomConnector>();
        }

        connectorSpawnpoint.transform.SetPositionAndRotation(position, rotation);
        connectorSpawnpoint.transform.localScale = Scale;

        _prevType = ConnectorType;

        NetworkServer.UnSpawn(connectorSpawnpoint.gameObject);
        NetworkServer.Spawn(connectorSpawnpoint.gameObject);

        return connectorSpawnpoint.gameObject;
    }

    private SpawnableRoomConnector ClutterPrefab
    {
        get
        {
            SpawnableRoomConnector prefab = ConnectorType switch
            {
                SpawnableRoomConnectorType.OpenHallway => PrefabManager.OpenHallway,
                SpawnableRoomConnectorType.ClutterBrokenElectricalBox => PrefabManager.BrokenElectricalBoxOpenConnector,
                SpawnableRoomConnectorType.ClutterSimpleBoxes => PrefabManager.SimpleBoxesOpenConnector,
                SpawnableRoomConnectorType.ClutterPipesShort => PrefabManager.PipesShortOpenConnector,
                SpawnableRoomConnectorType.ClutterBoxesLadder => PrefabManager.BoxesLadderOpenConnector,
                SpawnableRoomConnectorType.ClutterTankSupportedShelf => PrefabManager.TankSupportedShelfOpenConnector,
                SpawnableRoomConnectorType.ClutterAngledFences => PrefabManager.AngledFencesOpenConnector,
                SpawnableRoomConnectorType.ClutterHugeOrangePipes => PrefabManager.HugeOrangePipesOpenConnector,
                SpawnableRoomConnectorType.ClutterPipesLong => PrefabManager.PipesLongOpenConnector,
                _ => throw new InvalidOperationException($"No prefab defined for connector type {ConnectorType}")
            };

            return prefab;
        }
    }

    public override bool RequiresReloading => ConnectorType != _prevType || base.RequiresReloading;
    internal SpawnableRoomConnectorType _prevType;
}