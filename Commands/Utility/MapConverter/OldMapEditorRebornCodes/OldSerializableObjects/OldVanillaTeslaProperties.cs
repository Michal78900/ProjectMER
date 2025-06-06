using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldVanillaTeslaProperties
    {
        //List<string> is the original code. I have no idea why you can't use RoleTypeId
        public List<string> IgnoredRoles { get; set; } = new();
        public List<OldItemType> IgnoredItems { get; set; } = new();
        public bool InventoryItem { get; set; } = true;
        public float DamageMultiplier { get; set; } = 1f;
    }
}
