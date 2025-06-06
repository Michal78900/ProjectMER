namespace ProjectMER.Commands.Utility.MapConverter;

public class ConvertLogger
{
    public string LogText { get; private set; } = "\n";

    private readonly string _repeatingWarning = $"<color=orange>The object was added to the new yml config anyway, but we recommend manually configuring it using the id specified above.</color>\n\n";

    public void AddSerializationCompleteInfo()
    {
        LogText += $"Serialization of OldMapSchematic object to old config was successful!\n";
    }

    public void AddDeserializationCompleteInfo()
    {
        LogText += $"Deserialization of the old config into the OldMapSchematic object was successful!\n";
    }

    public void AddConfigsEqualsWarning()
    {
        LogText += $"The text from the yml config is completely equivalent to the text after deserialization and reverse serialization. Simply put, your config is perfect and did not cause problems when interpreting it on the new version of MER.\n";
    }

    public void AddConfigsNotEqualsWarning()
    {
        LogText += $"Serialization and deserialization were successful. However, the text from the old yml config is not equivalent to the text after deserialization and reverse serialization (hereinafter reserialization) of your old config. Simply put, although the reserialization of your old config was successful, some information could have been lost and not transferred to the new version of MER. For example, if your Lockers contained some firearms with certain attachments, they could have lost their attachments during reserialization. Or perhaps your config lacks default comments and because of this these configs are not equivalent. In any case, this should not prevent your configuration from being transferred to the new version and MER will save your reserialized config in the same directory where your original old config was located so that you can compare them and find the differences and MANUALLY make the necessary edits to the new yml config for the new version of MER. To automate the comparison of configs in order to quickly find the differences, I highly recommend using Notepad++. After installing this program, find the Plugins button at the top, and there through the \"plugin management\" find and install the \"Compare\" plugin. Now you will have buttons for comparing documents on your control panel. Good luck! <u>https://notepad-plus-plus.org/downloads/</u>\n";
    }

    public void AddWarningAboutImperfectRoomNaming()
    {
        LogText += $"<color=orange>We have to warn you that convenient room naming from Exiled <u>is simply absent in LabApi</u>. For this reason, the author of MER came up with the idea of ​​combining 3 enums: <color=green>\"FacilityZone\"</color>, <color=green>\"RoomShape\"</color> and <color=green>\"RoomName\"</color> using an underscore. It looks something like this: <color=green>\"Surface_Undefined_Outside\"</color>. Trying to uniqueize rooms using 3 variables is a very bad idea. Because a very large number of rooms, different as \"objects and entities\", in fact have the same string representation. For example: the name <color=green>\"Entrance_Straight_Unnamed\"</color> has as many as 6 unique rooms: <color=green>\"EzCafeteria\"</color>, <color=green>\"EzChef\"</color>, <color=green>\"EzConference\"</color>, <color=green>\"EzSmallrooms\"</color>, <color=green>\"EzStraight\"</color> and <color=green>\"EzStraightColumn\"</color>. <u>This is a problem with LabApi and Northwood</u>. We just have to wait another <u>5 years</u> until this game finally has a single standard for naming and combining rooms. However, for now we have what we have. For now this code will remain as it is. <u>Complain to the game developers about this every day</u>, then they will do something about it. Because in fact, MER cannot function properly due to the imperfection of room naming.</color>\n\n";
    }

    public void AddEnumConvertWarning(string id, string objectTypeName, string oldType)
    {
        LogText += $"<color=orange>The object with id: <color=green>{id}</color>, which was previously <color=green>{objectTypeName}</color>, has a property that cannot be obtained from <color=green>{oldType}</color>. </color>" + _repeatingWarning;
    }

    public void AddRoomTypeWarning(string id, string objectTypeName, string oldRoomType)
    {
        LogText += $"<color=orange>Object with id: <color=green>{id}</color> which is <color=green>{objectTypeName}</color> could not get a new room type from <color=green>{oldRoomType}</color>. </color>" + _repeatingWarning;
    }

    public void AddSomeRoomsWithSameMERNameWarning(string id, string oldRoomType, string merRoomName, List<string> equalExiledRoomNames)
    {
        LogText += $"<color=orange>The object with id: <color=green>{id}</color> uses room <color=green>{oldRoomType}</color>. In MER, the name of this room is <color=green>{merRoomName}</color>. The problem is that several more rooms have exactly the same name: <color=green>{string.Join(", ", equalExiledRoomNames)}</color>. The problem description and its solution are described above. </color>" + _repeatingWarning;
    }

    public void AddAttachmentsWarning(string id, string objectTypeName, string itemType)
    {
        LogText += $"<color=orange>We must warn you that the object with id: <color=green>{id}</color>, which is <color=green>{objectTypeName}</color>, may have lost its originally embedded logic associated with <color=green>{itemType}</color>. </color>" + _repeatingWarning;
    }

    public void AddSomeTeleportSameObjectIdWarning()
    {
        LogText += $"<color=red>Critical error found in old config. Some teleports with the same <color=green>ObjectId</color>. This is a very serious error that breaks the entire teleport convert system. The phase with teleporter conversion will be skipped entirely.</color>\n\n";
    }

    public void AddSomeTeleportsReferNonExistentIdWarning(string id)
    {
        LogText += $"<color=orange>An error was found in the old configuration. Teleport with id: <color=green>{id}</color> refers to a non-existent <color=green>TargetTeleporters(Id)</color>. This makes absolutely no sense, so we removed the id of this teleporter from the <color=green>\"Targets\"</color> list. </color>" + _repeatingWarning;
    }

    public void AddTeleportReferItselfWarning(string id)
    {
        LogText += $"<color=orange>A teleporter with id: <color=green>{id}</color> was detected, referencing itself. Referencing itself makes absolutely no sense, so we removed the id of this teleporter from the <color=green>\"Targets\"</color> list. </color>" + _repeatingWarning;
    }

    public void AddNewConfigWasSaved(string path)
    {
        LogText += $"<color=green>The config has been converted and saved to the path: \"{path}\"</color>\n";
    }

    public string GetTextWithoutRich()
    {
        return LogText
            .Replace("<color=green>", "")
            .Replace("<color=orange>", "")
            .Replace("<color=red>", "")
            .Replace("</color>", "")
            .Replace("<u>", "")
            .Replace("</u>", "");
    }
}