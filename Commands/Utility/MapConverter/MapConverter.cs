using AdminToys;
using CommandSystem;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Permissions;
using PlayerRoles;
using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes;
using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;
using ProjectMER.Features;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Extensions;
using ProjectMER.Features.Serializable;
using ProjectMER.Features.Serializable.Schematics;
using UnityEngine;

namespace ProjectMER.Commands.Utility.MapConverter;

public class MapConverter : ICommand
{
    public string Command => "convert";

    public string[] Aliases { get; } = [];

    public string Description => "Converts old map config (MapEditorReborn) to new view (ProjectMER).";

    private ConvertLogger logger;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.HasAnyPermission($"mpr.{Command}"))
        {
            response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
            return false;
        }
        if (arguments.Count < 2)
        {
            response = "\nUsage:\n" +
                "mp convert pathToOldMapCfg nameNewMapCfg";
            return false;
        }

        var fullPathToOldMapCfg = Path.GetFullPath(arguments.At(0).Replace("\"", ""));
        var nameNewMapCfg = arguments.At(1);

        if (!File.Exists(fullPathToOldMapCfg))
        {
            response = $"There are no files in the path you specified. The path should look something like this: \"C:\\Users\\Admin\\AppData\\Roaming\\EXILED\\Configs\\MapEditorReborn\\Maps\\on_round_started.yml\"";
            return false;
        }
        logger = new ConvertLogger();
        try
        {
            var originalYmlText = File.ReadAllText(fullPathToOldMapCfg);

            var oldMapSchematic = OldYamlParser.Deserializer.Deserialize<OldMapSchematic>(originalYmlText);
            logger.AddDeserializationCompleteInfo();

            var serializedOldMapSchematic = OldYamlParser.Serializer.Serialize(oldMapSchematic);
            logger.AddSerializationCompleteInfo();

            if (string.Equals(originalYmlText, serializedOldMapSchematic))
                logger.AddConfigsEqualsWarning();
            else
            {
                logger.AddConfigsNotEqualsWarning();
                var pathToOldMapCfg = Path.GetDirectoryName(fullPathToOldMapCfg);
                var nameOldMapCfg = Path.GetFileNameWithoutExtension(fullPathToOldMapCfg);
                var newNameReserializableCfg = nameOldMapCfg + "_reserializable_for_compare.yml";
                var pathToOldMapCfgWithNewFilename = Path.Combine(pathToOldMapCfg, newNameReserializableCfg);
                File.WriteAllText(pathToOldMapCfgWithNewFilename, serializedOldMapSchematic);
            }
            logger.AddWarningAboutImperfectRoomNaming();
            InterpretOldConfigToNewVersion(oldMapSchematic, nameNewMapCfg);
            Logger.Info(logger.GetTextWithoutRich());
        }
        catch (Exception ex)
        {
            Logger.Error(logger.GetTextWithoutRich() + "\n" + ex);
        }
        response = logger.LogText;
        return true;
    }

    private void InterpretOldConfigToNewVersion(OldMapSchematic oldMapSchematic, string nameOldMapCfg)
    {
        var mapSchematic = new MapSchematic(nameOldMapCfg);
        CreatePrimitives(oldMapSchematic, mapSchematic);
        CreateLights(oldMapSchematic, mapSchematic);
        CreateDoors(oldMapSchematic, mapSchematic);
        CreateWorkStations(oldMapSchematic, mapSchematic);
        CreateItemSpawnPoints(oldMapSchematic, mapSchematic);
        CreatePlayerSpawnPoints(oldMapSchematic, mapSchematic);
        CreateShootingTargets(oldMapSchematic, mapSchematic);
        CreateSchematics(oldMapSchematic, mapSchematic);
        CreateTeleports(oldMapSchematic, mapSchematic);
        string path = Path.Combine(ProjectMER.MapsDir, $"{nameOldMapCfg}.yml");
        File.WriteAllText(path, YamlParser.Serializer.Serialize(mapSchematic));
        logger.AddNewConfigWasSaved(path);
    }

    private void CreatePrimitives(OldMapSchematic oldMapSchematic, MapSchematic mapSchematic)
    {
        foreach (var oldPrimitive in oldMapSchematic.Primitives)
        {
            string id = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newPrimitive = new SerializablePrimitive();

            if (Enum.TryParse(oldPrimitive.PrimitiveType.ToString(), out PrimitiveType pt))
                newPrimitive.PrimitiveType = pt;
            else
                logger.AddEnumConvertWarning(id, oldPrimitive.GetType().Name, oldPrimitive.PrimitiveType.GetType().Name);
            newPrimitive.Color = oldPrimitive.Color;
            if (Enum.TryParse(oldPrimitive.PrimitiveFlags.ToString(), out PrimitiveFlags pf))
                newPrimitive.PrimitiveFlags = pf;
            else
                logger.AddEnumConvertWarning(id, oldPrimitive.GetType().Name, oldPrimitive.PrimitiveFlags.GetType().Name);
            if (oldPrimitive.RoomType == OldRoomType.Surface)
                newPrimitive.Position = new Vector3(oldPrimitive.Position.x, oldPrimitive.Position.y - 700, oldPrimitive.Position.z);
            else
                newPrimitive.Position = oldPrimitive.Position;
            newPrimitive.Rotation = oldPrimitive.Rotation;
            newPrimitive.Scale = oldPrimitive.Scale;
            if (AssociationsForOldRoomType.TryGetValue(oldPrimitive.RoomType, out string merRoomName))
            {
                newPrimitive.Room = merRoomName;
                var roomsWithSameNameMER = AssociationsForOldRoomType.Where(kvp => kvp.Value == merRoomName).Select(kvp => kvp.Key.ToString()).ToList();
                if (AssociationsForOldRoomType.Values.Count(mrn => mrn == merRoomName) > 1)
                    logger.AddSomeRoomsWithSameMERNameWarning(id, oldPrimitive.RoomType.ToString(), merRoomName, roomsWithSameNameMER);
            }
            else
                logger.AddRoomTypeWarning(id, oldPrimitive.GetType().Name, oldPrimitive.RoomType.ToString());
            mapSchematic.Primitives.TryAdd(id, newPrimitive);
        }
    }

    private void CreateLights(OldMapSchematic oldMapSchematic, MapSchematic mapSchematic)
    {
        foreach (var oldLights in oldMapSchematic.LightSources)
        {
            string id = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newLight = new SerializableLight();

            newLight.Color = oldLights.Color;
            newLight.Intensity = oldLights.Intensity;
            newLight.Range = oldLights.Range;
            if (!oldLights.Shadows) newLight.Shadows = LightShadows.None;
            if (oldLights.RoomType == OldRoomType.Surface)
                newLight.Position = new Vector3(oldLights.Position.x, oldLights.Position.y - 700, oldLights.Position.z);
            else
                newLight.Position = oldLights.Position;
            if (AssociationsForOldRoomType.TryGetValue(oldLights.RoomType, out string merRoomName))
            {
                newLight.Room = merRoomName;
                var roomsWithSameNameMER = AssociationsForOldRoomType.Where(kvp => kvp.Value == merRoomName).Select(kvp => kvp.Key.ToString()).ToList();
                if (AssociationsForOldRoomType.Values.Count(mrn => mrn == merRoomName) > 1)
                    logger.AddSomeRoomsWithSameMERNameWarning(id, oldLights.RoomType.ToString(), merRoomName, roomsWithSameNameMER);
            }
            else
                logger.AddRoomTypeWarning(id, oldLights.GetType().Name, oldLights.RoomType.ToString());

            mapSchematic.Lights.TryAdd(id, newLight);
        }
    }

    private void CreateDoors(OldMapSchematic oldMapSchematic, MapSchematic mapSchematic)
    {
        foreach (var oldDoor in oldMapSchematic.Doors)
        {
            string id = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newDoor = new SerializableDoor();

            if (Enum.TryParse(oldDoor.DoorType.ToString(), out DoorType dt))
                newDoor.DoorType = dt;
            else
                logger.AddEnumConvertWarning(id, oldDoor.GetType().Name, oldDoor.DoorType.GetType().Name);
            newDoor.IsOpen = oldDoor.IsOpen;
            newDoor.IsLocked = oldDoor.IsLocked;
            if (Enum.TryParse(oldDoor.KeycardPermissions.ToString(), out DoorPermissionFlags dpf))
                newDoor.RequiredPermissions = dpf;
            else
                logger.AddEnumConvertWarning(id, oldDoor.GetType().Name, oldDoor.KeycardPermissions.GetType().Name);
            if (oldDoor.RoomType == OldRoomType.Surface)
                newDoor.Position = new Vector3(oldDoor.Position.x, oldDoor.Position.y - 700, oldDoor.Position.z);
            else
                newDoor.Position = oldDoor.Position;
            newDoor.Rotation = oldDoor.Rotation;
            newDoor.Scale = oldDoor.Scale;
            if (AssociationsForOldRoomType.TryGetValue(oldDoor.RoomType, out string merRoomName))
            {
                newDoor.Room = merRoomName;
                var roomsWithSameNameMER = AssociationsForOldRoomType.Where(kvp => kvp.Value == merRoomName).Select(kvp => kvp.Key.ToString()).ToList();
                if (AssociationsForOldRoomType.Values.Count(mrn => mrn == merRoomName) > 1)
                    logger.AddSomeRoomsWithSameMERNameWarning(id, oldDoor.RoomType.ToString(), merRoomName, roomsWithSameNameMER);
            }
            else
                logger.AddRoomTypeWarning(id, oldDoor.GetType().Name, oldDoor.RoomType.ToString());

            mapSchematic.Doors.TryAdd(id, newDoor);
        }
    }

    private void CreateWorkStations(OldMapSchematic oldMapSchematic, MapSchematic mapSchematic)
    {
        foreach (var oldWorkStation in oldMapSchematic.WorkStations)
        {
            string id = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newWorkStation = new SerializableWorkstation();

            newWorkStation.IsInteractable = oldWorkStation.IsInteractable;
            if (oldWorkStation.RoomType == OldRoomType.Surface)
                newWorkStation.Position = new Vector3(oldWorkStation.Position.x, oldWorkStation.Position.y - 700, oldWorkStation.Position.z);
            else
                newWorkStation.Position = oldWorkStation.Position;
            newWorkStation.Rotation = oldWorkStation.Rotation;
            newWorkStation.Scale = oldWorkStation.Scale;
            if (AssociationsForOldRoomType.TryGetValue(oldWorkStation.RoomType, out string merRoomName))
            {
                newWorkStation.Room = merRoomName;
                var roomsWithSameNameMER = AssociationsForOldRoomType.Where(kvp => kvp.Value == merRoomName).Select(kvp => kvp.Key.ToString()).ToList();
                if (AssociationsForOldRoomType.Values.Count(mrn => mrn == merRoomName) > 1)
                    logger.AddSomeRoomsWithSameMERNameWarning(id, oldWorkStation.RoomType.ToString(), merRoomName, roomsWithSameNameMER);
            }
            else
                logger.AddRoomTypeWarning(id, oldWorkStation.GetType().Name, oldWorkStation.RoomType.ToString());

            mapSchematic.Workstations.TryAdd(id, newWorkStation);
        }
    }

    private void CreateItemSpawnPoints(OldMapSchematic oldMapSchematic, MapSchematic mapSchematic)
    {
        foreach (var oldItemSpawnPoint in oldMapSchematic.ItemSpawnPoints)
        {
            string id = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newItemSpawnPoint = new SerializableItemSpawnpoint();

            if (Enum.TryParse(oldItemSpawnPoint.Item.ToString(), out ItemType it))
                newItemSpawnPoint.ItemType = it;
            else
                logger.AddEnumConvertWarning(id, oldItemSpawnPoint.GetType().Name, oldItemSpawnPoint.Item.GetType().Name);
            newItemSpawnPoint.AttachmentsCode = oldItemSpawnPoint.AttachmentsCode;
            if (oldItemSpawnPoint.AttachmentsCode != "-1")
                logger.AddAttachmentsWarning(id, oldItemSpawnPoint.GetType().Name, newItemSpawnPoint.ItemType.GetType().Name);
            newItemSpawnPoint.NumberOfItems = oldItemSpawnPoint.NumberOfItems;
            newItemSpawnPoint.NumberOfUses = oldItemSpawnPoint.NumberOfUses;
            newItemSpawnPoint.UseGravity = oldItemSpawnPoint.UseGravity;
            newItemSpawnPoint.CanBePickedUp = oldItemSpawnPoint.CanBePickedUp;
            if (oldItemSpawnPoint.RoomType == OldRoomType.Surface)
                newItemSpawnPoint.Position = new Vector3(oldItemSpawnPoint.Position.x, oldItemSpawnPoint.Position.y - 700, oldItemSpawnPoint.Position.z);
            else
                newItemSpawnPoint.Position = oldItemSpawnPoint.Position;
            newItemSpawnPoint.Rotation = oldItemSpawnPoint.Rotation;
            newItemSpawnPoint.Scale = oldItemSpawnPoint.Scale;
            if (AssociationsForOldRoomType.TryGetValue(oldItemSpawnPoint.RoomType, out string merRoomName))
            {
                newItemSpawnPoint.Room = merRoomName;
                var roomsWithSameNameMER = AssociationsForOldRoomType.Where(kvp => kvp.Value == merRoomName).Select(kvp => kvp.Key.ToString()).ToList();
                if (AssociationsForOldRoomType.Values.Count(mrn => mrn == merRoomName) > 1)
                    logger.AddSomeRoomsWithSameMERNameWarning(id, oldItemSpawnPoint.RoomType.ToString(), merRoomName, roomsWithSameNameMER);
            }
            else
                logger.AddRoomTypeWarning(id, oldItemSpawnPoint.GetType().Name, oldItemSpawnPoint.RoomType.ToString());

            mapSchematic.ItemSpawnpoints.TryAdd(id, newItemSpawnPoint);
        }
    }

    private void CreatePlayerSpawnPoints(OldMapSchematic oldMapSchematic, MapSchematic mapSchematic)
    {
        foreach (var oldPlayerSpawnPoint in oldMapSchematic.PlayerSpawnPoints)
        {
            string id = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newPlayerSpawnPoint = new SerializablePlayerSpawnpoint();

            if (Enum.TryParse(oldPlayerSpawnPoint.SpawnableTeam.ToString(), out RoleTypeId rt))
                newPlayerSpawnPoint.Roles = new List<RoleTypeId>() { rt };
            else
                logger.AddEnumConvertWarning(id, oldPlayerSpawnPoint.GetType().Name, oldPlayerSpawnPoint.SpawnableTeam.GetType().Name);
            if (oldPlayerSpawnPoint.RoomType == OldRoomType.Surface)
                newPlayerSpawnPoint.Position = new Vector3(oldPlayerSpawnPoint.Position.x, oldPlayerSpawnPoint.Position.y - 700, oldPlayerSpawnPoint.Position.z);
            else
                newPlayerSpawnPoint.Position = oldPlayerSpawnPoint.Position;
            if (AssociationsForOldRoomType.TryGetValue(oldPlayerSpawnPoint.RoomType, out string merRoomName))
            {
                newPlayerSpawnPoint.Room = merRoomName;
                var roomsWithSameNameMER = AssociationsForOldRoomType.Where(kvp => kvp.Value == merRoomName).Select(kvp => kvp.Key.ToString()).ToList();
                if (AssociationsForOldRoomType.Values.Count(mrn => mrn == merRoomName) > 1)
                    logger.AddSomeRoomsWithSameMERNameWarning(id, oldPlayerSpawnPoint.RoomType.ToString(), merRoomName, roomsWithSameNameMER);
            }
            else
                logger.AddRoomTypeWarning(id, oldPlayerSpawnPoint.GetType().Name, oldPlayerSpawnPoint.RoomType.ToString());

            mapSchematic.PlayerSpawnpoints.TryAdd(id, newPlayerSpawnPoint);
        }
    }

    private void CreateShootingTargets(OldMapSchematic oldMapSchematic, MapSchematic mapSchematic)
    {
        foreach (var oldShootingTarget in oldMapSchematic.ShootingTargets)
        {
            string id = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newShootingTarget = new SerializableShootingTarget();

            if (Enum.TryParse(oldShootingTarget.TargetType.ToString(), out TargetType tt))
                newShootingTarget.TargetType = tt;
            else
                logger.AddEnumConvertWarning(id, oldShootingTarget.GetType().Name, oldShootingTarget.TargetType.GetType().Name);
            if (oldShootingTarget.RoomType == OldRoomType.Surface)
                newShootingTarget.Position = new Vector3(oldShootingTarget.Position.x, oldShootingTarget.Position.y - 700, oldShootingTarget.Position.z);
            else
                newShootingTarget.Position = oldShootingTarget.Position;
            newShootingTarget.Rotation = oldShootingTarget.Rotation;
            newShootingTarget.Scale = oldShootingTarget.Scale;
            if (AssociationsForOldRoomType.TryGetValue(oldShootingTarget.RoomType, out string merRoomName))
            {
                newShootingTarget.Room = merRoomName;
                var roomsWithSameNameMER = AssociationsForOldRoomType.Where(kvp => kvp.Value == merRoomName).Select(kvp => kvp.Key.ToString()).ToList();
                if (AssociationsForOldRoomType.Values.Count(mrn => mrn == merRoomName) > 1)
                    logger.AddSomeRoomsWithSameMERNameWarning(id, oldShootingTarget.RoomType.ToString(), merRoomName, roomsWithSameNameMER);
            }
            else
                logger.AddRoomTypeWarning(id, oldShootingTarget.GetType().Name, oldShootingTarget.RoomType.ToString());

            mapSchematic.ShootingTargets.TryAdd(id, newShootingTarget);
        }
    }

    private void CreateSchematics(OldMapSchematic oldMapSchematic, MapSchematic mapSchematic)
    {
        foreach (var oldSchematic in oldMapSchematic.Schematics)
        {
            string id = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newSchematic = new SerializableSchematic();

            newSchematic.SchematicName = oldSchematic.SchematicName;
            if (oldSchematic.RoomType == OldRoomType.Surface)
                newSchematic.Position = new Vector3(oldSchematic.Position.x, oldSchematic.Position.y - 700, oldSchematic.Position.z);
            else
                newSchematic.Position = oldSchematic.Position;
            newSchematic.Rotation = oldSchematic.Rotation;
            newSchematic.Scale = oldSchematic.Scale;
            if (AssociationsForOldRoomType.TryGetValue(oldSchematic.RoomType, out string merRoomName))
            {
                newSchematic.Room = merRoomName;
                var roomsWithSameNameMER = AssociationsForOldRoomType.Where(kvp => kvp.Value == merRoomName).Select(kvp => kvp.Key.ToString()).ToList();
                if (AssociationsForOldRoomType.Values.Count(mrn => mrn == merRoomName) > 1)
                    logger.AddSomeRoomsWithSameMERNameWarning(id, oldSchematic.RoomType.ToString(), merRoomName, roomsWithSameNameMER);
            }
            else
                logger.AddRoomTypeWarning(id, oldSchematic.GetType().Name, oldSchematic.RoomType.ToString());

            mapSchematic.Schematics.TryAdd(id, newSchematic);
        }
    }

    private void CreateTeleports(OldMapSchematic oldMapSchematic, MapSchematic mapSchematic)
    {
        var oldToNewTeleportIds = new Dictionary<int, string>();
        foreach (var oldTeleport in oldMapSchematic.Teleports)
        {
            string id = Guid.NewGuid().ToString("N").Substring(0, 8);
            var newTeleport = new SerializableTeleport();

            newTeleport.Cooldown = oldTeleport.Cooldown;
            if (oldTeleport.RoomType == OldRoomType.Surface)
                newTeleport.Position = new Vector3(oldTeleport.Position.x, oldTeleport.Position.y - 700, oldTeleport.Position.z);
            else
                newTeleport.Position = oldTeleport.Position;
            newTeleport.Rotation = oldTeleport.Rotation;
            newTeleport.Scale = oldTeleport.Scale;
            if (AssociationsForOldRoomType.TryGetValue(oldTeleport.RoomType, out string merRoomName))
            {
                newTeleport.Room = merRoomName;
                var roomsWithSameNameMER = AssociationsForOldRoomType.Where(kvp => kvp.Value == merRoomName).Select(kvp => kvp.Key.ToString()).ToList();
                if (AssociationsForOldRoomType.Values.Count(mrn => mrn == merRoomName) > 1)
                    logger.AddSomeRoomsWithSameMERNameWarning(id, oldTeleport.RoomType.ToString(), merRoomName, roomsWithSameNameMER);
            }
            else
                logger.AddRoomTypeWarning(id, oldTeleport.GetType().Name, oldTeleport.RoomType.ToString());

            oldToNewTeleportIds[oldTeleport.ObjectId] = id;
            mapSchematic.Teleports.TryAdd(id, newTeleport);
        }

        var countUniqueTeleports = oldMapSchematic.Teleports.Select(ot => ot.ObjectId).Distinct().Count();
        if (countUniqueTeleports != oldMapSchematic.Teleports.Count)
        {
            logger.AddSomeTeleportSameObjectIdWarning();
            mapSchematic.Teleports.Clear();
            return;
        }

        foreach (var oldTeleport in oldMapSchematic.Teleports)
        {
            string id = oldToNewTeleportIds[oldTeleport.ObjectId];
            var newTeleport = mapSchematic.Teleports[id];
            foreach (var target in oldTeleport.TargetTeleporters)
            {
                if (oldToNewTeleportIds.TryGetValue(target.Id, out string value))
                {
                    if (value == id)
                    {
                        logger.AddTeleportReferItselfWarning(id);
                        continue;
                    }
                    newTeleport.Targets.Add(value);
                }
                else
                    logger.AddSomeTeleportsReferNonExistentIdWarning(id);
            }
        }
    }

    //14.1 This list can be obtained from seed: 1820502615 and 1022079800. 63/65 unique rooms without HczStraightVariant and Unknown.
    //Moreover, convenient room naming from Exiled is simply absent in LabApi. For this reason, the author of MER came up with the idea of ​​combining 3 enums: FacilityZone, RoomShape and RoomName using an underscore. It looks something like this: "Surface_Undefined_Outside". Trying to uniqueize rooms using 3 variables is a very bad idea. Because a very large number of rooms, different as "objects and entities", in fact have the same string representation. For example: the name "Entrance_Straight_Unnamed" has as many as 6 unique rooms: "EzCafeteria", "EzChef", "EzConference", "EzSmallrooms", "EzStraight" and "EzStraightColumn". This is a problem with LabApi and Northwood.We just have to wait another 5 years until this game finally has a single standard for naming and combining rooms. However, for now we have what we have. For now this code will remain as it is. Complain to the game developers about this every day, then they will do something about it.Because in fact, MER cannot function properly due to the imperfection of room naming.
    private Dictionary<OldRoomType, string> AssociationsForOldRoomType { get; set; } = new Dictionary<OldRoomType, string>()
    {
        { OldRoomType.EzCafeteria, "Entrance_Straight_Unnamed" },
        { OldRoomType.EzCheckpointHallwayA, "Entrance_Straight_HczCheckpointToEntranceZone" },
        { OldRoomType.EzCheckpointHallwayB, "Entrance_Straight_HczCheckpointToEntranceZone" },
        { OldRoomType.EzChef, "Entrance_Straight_Unnamed" },
        { OldRoomType.EzCollapsedTunnel, "Entrance_Endroom_EzCollapsedTunnel" },
        { OldRoomType.EzConference, "Entrance_Straight_Unnamed" },
        { OldRoomType.EzCrossing, "Entrance_XShape_Unnamed" },
        { OldRoomType.EzCurve, "Entrance_Curve_Unnamed" },
        { OldRoomType.EzDownstairsPcs, "Entrance_Straight_EzOfficeSmall" },
        { OldRoomType.EzGateA, "Entrance_Endroom_EzGateA" },
        { OldRoomType.EzGateB, "Entrance_Endroom_EzGateB" },
        { OldRoomType.EzIntercom, "Entrance_Curve_EzIntercom" },
        { OldRoomType.EzPcs, "Entrance_Straight_EzOfficeLarge" },
        { OldRoomType.EzShelter, "Entrance_Endroom_EzEvacShelter" },
        { OldRoomType.EzSmallrooms, "Entrance_Straight_Unnamed" },
        { OldRoomType.EzStraight, "Entrance_Straight_Unnamed" },
        { OldRoomType.EzStraightColumn, "Entrance_Straight_Unnamed" },
        { OldRoomType.EzTCross, "Entrance_TShape_Unnamed" },
        { OldRoomType.EzUpstairsPcs, "Entrance_Straight_EzOfficeStoried" },
        { OldRoomType.EzVent, "Entrance_Endroom_EzRedroom" },
        { OldRoomType.Hcz049, "HeavyContainment_Straight_Hcz049" },
        { OldRoomType.Hcz079, "HeavyContainment_Endroom_Hcz079" },
        { OldRoomType.Hcz096, "HeavyContainment_Endroom_Hcz096" },
        { OldRoomType.Hcz106, "HeavyContainment_Endroom_Hcz106" },
        //{ OldRoomType.Hcz127, "HeavyContainment_TShape_Hcz127" }, This room did not exist at the last support date of map editor reborn (9.5.0). It was only added in 9.6.0.
        { OldRoomType.Hcz939, "HeavyContainment_Curve_Hcz939" },
        { OldRoomType.HczArmory, "HeavyContainment_TShape_HczArmory" },
        { OldRoomType.HczCornerDeep, "HeavyContainment_Curve_Unnamed" },
        { OldRoomType.HczCrossRoomWater, "HeavyContainment_XShape_Unnamed" },
        { OldRoomType.HczCrossing, "HeavyContainment_XShape_Unnamed" },
        { OldRoomType.HczCurve, "HeavyContainment_Curve_Unnamed" },
        { OldRoomType.HczElevatorA, "HeavyContainment_Endroom_HczCheckpointA" },
        { OldRoomType.HczElevatorB, "HeavyContainment_Endroom_HczCheckpointB" },
        { OldRoomType.HczEzCheckpointA, "HeavyContainment_Straight_HczCheckpointToEntranceZone" },
        { OldRoomType.HczEzCheckpointB, "HeavyContainment_Straight_HczCheckpointToEntranceZone" },
        { OldRoomType.HczHid, "HeavyContainment_Straight_HczMicroHID" },
        { OldRoomType.HczIntersection, "HeavyContainment_TShape_Unnamed" },
        { OldRoomType.HczIntersectionJunk, "HeavyContainment_TShape_Unnamed" },
        { OldRoomType.HczNuke, "HeavyContainment_TShape_HczWarhead" },
        //{ OldRoomType.HczServerRoom, "HeavyContainment_Straight_HczServers" }, This room did not exist at the last support date of map editor reborn (9.5.0). It was only added in 9.6.0.
        { OldRoomType.HczStraight, "HeavyContainment_Straight_Unnamed" },
        { OldRoomType.HczStraightC, "HeavyContainment_Straight_Unnamed" },
        { OldRoomType.HczStraightPipeRoom, "HeavyContainment_Straight_Unnamed" },
        { OldRoomType.HczTesla, "HeavyContainment_Straight_HczTesla" },
        { OldRoomType.HczTestRoom, "HeavyContainment_Straight_HczTestroom" },
        { OldRoomType.Lcz173, "LightContainment_Endroom_Lcz173" },
        { OldRoomType.Lcz330, "LightContainment_Endroom_Lcz330" },
        { OldRoomType.Lcz914, "LightContainment_Endroom_Lcz914" },
        { OldRoomType.LczAirlock, "LightContainment_Straight_LczAirlock" },
        { OldRoomType.LczArmory, "LightContainment_Endroom_LczArmory" },
        { OldRoomType.LczCafe, "LightContainment_Endroom_LczComputerRoom" },
        { OldRoomType.LczCheckpointA, "LightContainment_Endroom_LczCheckpointA" },
        { OldRoomType.LczCheckpointB, "LightContainment_Endroom_LczCheckpointB" },
        { OldRoomType.LczClassDSpawn, "LightContainment_Endroom_LczClassDSpawn" },
        { OldRoomType.LczCrossing, "LightContainment_XShape_Unnamed" },
        { OldRoomType.LczCurve, "LightContainment_Curve_Unnamed" },
        { OldRoomType.LczGlassBox, "LightContainment_Endroom_LczGlassroom" },
        { OldRoomType.LczPlants, "LightContainment_Straight_LczGreenhouse" },
        { OldRoomType.LczStraight, "LightContainment_Straight_Unnamed" },
        { OldRoomType.LczTCross, "LightContainment_TShape_Unnamed" },
        { OldRoomType.LczToilets, "LightContainment_Straight_LczToilets" },
        { OldRoomType.Pocket, "Other_Undefined_Pocket" },
        { OldRoomType.Surface, "Surface_Undefined_Outside" },
        //{ OldRoomType.HczStraightVariant, "???" }, At the time of version exiled 9.6.0 this room is impossible to get. I did over 50 restarts and couldn't get this room.
        //{ OldRoomType.Unknown, "???" }, This room does not actually exist.
    };
}
