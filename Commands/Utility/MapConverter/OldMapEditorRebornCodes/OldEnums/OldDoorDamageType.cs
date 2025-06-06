namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums
{
    [Flags]
    public enum OldDoorDamageType : byte
    {
        None = 1,
        ServerCommand = 2,
        Grenade = 4,
        Weapon = 8,
        Scp096 = 0x10,
        ParticleDisruptor = 0x20
    }
}
