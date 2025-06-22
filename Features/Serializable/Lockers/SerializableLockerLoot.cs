namespace ProjectMER.Features.Serializable.Lockers;

public class SerializableLockerLoot
{
	public SerializableLockerLoot() { }

	public SerializableLockerLoot(ItemType targetItem, int remainingUses, int maxPerChamber, int probabilityPoints, int minPerChamber)
	{
		TargetItem = targetItem;
		RemainingUses = remainingUses;
		MaxPerChamber = maxPerChamber;
		ProbabilityPoints = probabilityPoints;
		MinPerChamber = minPerChamber;
	}

	public ItemType TargetItem { get; set; }

	public int RemainingUses { get; set; }

	public int MaxPerChamber { get; set; }

	public int ProbabilityPoints { get; set; }

	public int MinPerChamber { get; set; } = 1;
}
