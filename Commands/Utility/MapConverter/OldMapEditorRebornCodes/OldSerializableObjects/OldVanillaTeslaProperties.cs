using ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldEnums;

namespace ProjectMER.Commands.Utility.MapConverter.OldMapEditorRebornCodes.OldSerializableObjects
{
    [Serializable]
    public class OldVanillaTeslaProperties
    {
        public List<string> IgnoredRoles { get; set; }//List<string> is the original code. I have no idea why you can't use RoleTypeId
        public List<OldItemType> IgnoredItems { get; set; }
        public bool InventoryItem { get; set; }
        public float DamageMultiplier { get; set; }
    }
}
