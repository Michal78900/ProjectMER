using HarmonyLib;
using Interactables.Interobjects;
using MapGeneration;
using UnityEngine;

namespace ProjectMER.Patches;

[HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CanBeDetonated))]
public static class AlphaWarheadCanBeDetonatedFix
{
	public static bool Prefix(Vector3 pos, bool includeOnlyLifts, ref bool __result)
	{
		FacilityZone zone = pos.GetZone();
		if (zone != FacilityZone.Surface && zone != FacilityZone.None && !includeOnlyLifts)
		{
			__result = true;
			return false;
		}

		foreach (ElevatorChamber chamber in ElevatorChamber.AllChambers)
		{
			if (!chamber.WorldspaceBounds.Contains(pos))
				continue;

			__result = true;
			return false;
		}

		__result = false;
		return false;
	}
}
