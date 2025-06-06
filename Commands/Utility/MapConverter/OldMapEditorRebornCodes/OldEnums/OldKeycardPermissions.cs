namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums
{
    [Flags]
    public enum OldKeycardPermissions : ushort
    {
        None = 0,
        Checkpoints = 1,
        ExitGates = 2,
        Intercom = 4,
        AlphaWarhead = 8,
        ContainmentLevelOne = 0x10,
        ContainmentLevelTwo = 0x20,
        ContainmentLevelThree = 0x40,
        ArmoryLevelOne = 0x80,
        ArmoryLevelTwo = 0x100,
        ArmoryLevelThree = 0x200,
        ScpOverride = 0x400
    }
}
