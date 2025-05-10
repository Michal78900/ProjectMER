using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using ProjectMER.Features.Serializable.Schematics;
using Utf8Json;
using YamlDotNet.Core;

namespace ProjectMER.Features;

public static class MapUtils
{
	public const string UntitledMapName = "Untitled";

	public static MapSchematic UntitledMap => LoadedMaps.GetOrAdd(UntitledMapName, () => new(UntitledMapName));

	public static Dictionary<string, MapSchematic> LoadedMaps { get; private set; } = [];

	public static void SaveMap(string mapName)
	{
		if (LoadedMaps.TryGetValue(mapName, out MapSchematic map))
			map.Merge(UntitledMap);
		else
			map = UntitledMap;

		string path = Path.Combine(ProjectMER.MapsDir, $"{mapName}.yml");
		File.WriteAllText(path, YamlParser.Serializer.Serialize(map));
	}

	public static void LoadMap(string mapName)
	{
		MapSchematic map = GetMapData(mapName);
		UnloadMap(mapName);
		map.Reload();

		LoadedMaps.Add(mapName, map);
	}

	public static bool UnloadMap(string mapName)
	{
		if (!LoadedMaps.ContainsKey(mapName))
			return false;

		foreach (MapEditorObject mapEditorObject in LoadedMaps[mapName].SpawnedObjects)
			mapEditorObject.Destroy();

		LoadedMaps.Remove(mapName);
		return true;
	}

	public static MapSchematic GetMapData(string mapName)
	{
		MapSchematic map;

		string path = Path.Combine(ProjectMER.MapsDir, $"{mapName}.yml");
		if (!File.Exists(path))
		{
			string error = $"Failed to load map data: File {mapName}.yml does not exist!";
			Logger.Error(error);
			throw new FileNotFoundException(error);
		}

		try
		{
			map = YamlParser.Deserializer.Deserialize<MapSchematic>(File.ReadAllText(path));
			map.Name = mapName;
		}
		catch (YamlException e)
		{
			string error = $"Failed to load map data: File {mapName}.yml has YAML errors!\n{e.ToString().Split('\n')[0]}";
			Logger.Error(error);
			throw new YamlException(error);
		}

		return map;
	}

	public static bool TryGetSchematicDataByName(string schematicName, out SchematicObjectDataList data)
	{
		try
		{
			data = GetSchematicDataByName(schematicName);
			return true;
		}
		catch (Exception)
		{
			data = null!;
			return false;
		}
	}

	public static SchematicObjectDataList GetSchematicDataByName(string schematicName)
	{
		SchematicObjectDataList data;
		string schematicDirPath = Path.Combine(ProjectMER.SchematicsDir, schematicName);
		string schematicJsonPath = Path.Combine(schematicDirPath, $"{schematicName}.json");
		string misplacedSchematicJsonPath = schematicDirPath + ".json";

		if (!Directory.Exists(schematicDirPath))
		{
			// Some users may throw a single JSON file into Schematics folder, this automatically creates and moved the file to the correct schematic directory.
			if (File.Exists(misplacedSchematicJsonPath))
			{
				Directory.CreateDirectory(schematicDirPath);
				File.Move(misplacedSchematicJsonPath, schematicJsonPath);
				return GetSchematicDataByName(schematicName);
			}

			string error = $"Failed to load schematic data: Directory {schematicName} does not exist!";
			Logger.Error(error);
			throw new DirectoryNotFoundException(error);
		}

		if (!File.Exists(schematicJsonPath))
		{
			// Same as above but with the folder existing and file not being there for some reason.
			if (File.Exists(misplacedSchematicJsonPath))
			{
				File.Move(misplacedSchematicJsonPath, schematicJsonPath);
				return GetSchematicDataByName(schematicName);
			}

			string error = $"Failed to load schematic data: File {schematicName}.json does not exist!";
			Logger.Error(error);
			throw new FileNotFoundException(error);
		}

		try
		{
			data = JsonSerializer.Deserialize<SchematicObjectDataList>(File.ReadAllText(schematicJsonPath));
			data.Path = schematicDirPath;
		}
		catch (JsonParsingException e)
		{
			string error = $"Failed to load schematic data: File {schematicName}.json has JSON errors!\n{e.ToString().Split('\n')[0]}";
			Logger.Error(error);
			throw new JsonParsingException(error);
		}

		return data;
	}

	public static string[] GetAvailableSchematicNames() => Directory.GetFiles(ProjectMER.SchematicsDir, "*.json", SearchOption.AllDirectories).Select(Path.GetFileNameWithoutExtension).Where(x => !x.Contains('-')).ToArray();
}
