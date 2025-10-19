using GameCore;
using ProjectMER.Configs;
using ProjectMER.Events.Handlers;
using ProjectMER.Features.Enums;
using ProjectMER.Features.Serializable.Schematics;
using Site11_Revised;
using Utf8Json;

namespace ProjectMER.Features.Serializable;

public class GetCosmeticDataByNameAndPosition
{
    public static bool TryGetSchematicDataByName(string schematicName, cosmeticPosition cpos, out SchematicObjectDataList data)
    {
        try
        {
            data = GetCosmeticDataByNameAndPosition.GetSchematicDataByName(schematicName, cpos);
            return true;
        }
        catch (Exception)
        {
            data = null!;
            return false;
        }
    }
    public static SchematicObjectDataList GetSchematicDataByName(string schematicName, cosmeticPosition cpos)
    {
        SchematicObjectDataList data;
            string schematicDirPath = string.Empty;
            switch (cpos)
            {
                case cosmeticPosition.Head:
                    Logger.Debug("HeadCosmetic");
                    schematicDirPath = Path.Combine(ProjectMER.HatsDir, schematicName);
                    break;
                case cosmeticPosition.Body:
                    Logger.Debug("BodyCosmetic");
                    schematicDirPath = Path.Combine(ProjectMER.BodyDir, schematicName);
                    break;
                case cosmeticPosition.LeftArm:
                    Logger.Debug("LeftArmCosmetic");
                    schematicDirPath = Path.Combine(ProjectMER.LeftArmDir, schematicName);
                    break;
                case cosmeticPosition.RightArm:
                    Logger.Debug("RightArmCosmetic");
                    schematicDirPath = Path.Combine(ProjectMER.RightArmDir, schematicName);
                    break;
            }
            string schematicJsonPath = Path.Combine(schematicDirPath, $"{schematicName}.json");
            string misplacedSchematicJsonPath = schematicDirPath + ".json";

            if (!Directory.Exists(schematicDirPath))
            {
                // Some users may throw a single JSON file into Schematics folder, this automatically creates and moved the file to the correct schematic directory.
                if (File.Exists(misplacedSchematicJsonPath))
                {
                    Directory.CreateDirectory(schematicDirPath);
                    File.Move(misplacedSchematicJsonPath, schematicJsonPath);
                    return GetSchematicDataByName(schematicName, cpos);
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
                    return GetSchematicDataByName(schematicName, cpos);
                }

                string error = $"Failed to load schematic data: File {schematicName}.json does not exist!";
                Logger.Error(error);
                throw new FileNotFoundException(error);
            }

            try
            {
                Logger.Debug("Jsonpath: " + schematicJsonPath);
                data = JsonSerializer.Deserialize<SchematicObjectDataList>(File.ReadAllText(schematicJsonPath));
                if (data != null)
                {
                    Logger.Debug("SchematicObjectDataList is not null");
                }
                data.Path = schematicDirPath;
            }
            catch (JsonParsingException e)
            {
                string error =
                    $"Failed to load schematic data: File {schematicName}.json has JSON errors!\n{e.ToString().Split('\n')[0]}";
                Logger.Error(error);
                throw new JsonParsingException(error);
            }

            return data;
        }
}