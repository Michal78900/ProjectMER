using AdminToys;
using System.Text;
using CommandSystem;
using LabApi.Features.Permissions;
using NorthwoodLib.Pools;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace ProjectMER.Commands.Utility;

/// <summary>
/// TEST COMMAND: Reports how many AdminToy blocks in a loaded schematic
/// have NetworkIsStatic=true vs false. Used to verify the Static flag fix.
///
/// Usage: mp staticcheck [schematicName]
/// </summary>
public class StaticCheck : ICommand
{
	/// <inheritdoc/>
	public string Command => "staticcheck";

	/// <inheritdoc/>
	public string[] Aliases { get; } = ["sc", "statcheck"];

	/// <inheritdoc/>
	public string Description => "[TEST] Checks how many blocks in a loaded schematic have NetworkIsStatic=true. Used to verify the Static flag fix.";

	/// <inheritdoc/>
	public bool SanitizeResponse => false;

	/// <inheritdoc/>
	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission($"mpr.{Command}"))
		{
			response = $"You don't have permission to execute this command. Required permission: mpr.{Command}";
			return false;
		}

		StringBuilder sb = StringBuilderPool.Shared.Rent();
		sb.AppendLine();

		// Find all active SchematicObjects in the scene
		SchematicObject[] allSchematics = UnityEngine.Object.FindObjectsOfType<SchematicObject>();

		if (allSchematics.Length == 0)
		{
			response = "<color=yellow>No schematics are currently loaded/spawned.</color>";
			return false;
		}

		IEnumerable<SchematicObject> targets = arguments.Count >= 1
			? allSchematics.Where(s => s.Name.Equals(arguments.At(0), StringComparison.OrdinalIgnoreCase))
			: allSchematics;

		if (!targets.Any())
		{
			response = $"No loaded schematic found with name: {arguments.At(0)}\nLoaded: {string.Join(", ", allSchematics.Select(s => s.Name))}";
			return false;
		}

		int totalBlocks = 0;
		int staticTrue = 0;
		int staticFalse = 0;

		foreach (SchematicObject schematic in targets)
		{
			sb.AppendLine($"<color=orange><b>Schematic: {schematic.Name}</b></color>");

			int schStatic = 0;
			int schNonStatic = 0;

			foreach (GameObject block in schematic.AttachedBlocks)
			{
				if (!block.TryGetComponent(out AdminToyBase toy))
					continue;

				if (toy.IsStatic)
					schStatic++;
				else
					schNonStatic++;
			}

			int schTotal = schStatic + schNonStatic;
			totalBlocks += schTotal;
			staticTrue += schStatic;
			staticFalse += schNonStatic;

			sb.AppendLine($"  AdminToy blocks : <b>{schTotal}</b>");
			sb.AppendLine($"  <color=green>NetworkIsStatic = true  : {schStatic}</color>");
			sb.AppendLine($"  <color=red>NetworkIsStatic = false : {schNonStatic}</color>");

			if (schTotal > 0)
			{
				float pct = (float)schStatic / schTotal * 100f;
				string verdict = pct >= 99f
					? "<color=green><b>✅ FIX WORKING</b></color>"
					: pct == 0f
						? "<color=red><b>❌ FIX NOT WORKING — all blocks still non-static!</b></color>"
						: $"<color=yellow><b>⚠️ PARTIAL ({pct:F0}% static)</b></color>";
				sb.AppendLine($"  {verdict}");
			}

			sb.AppendLine();
		}

		sb.AppendLine($"<b>TOTAL: {staticTrue}/{totalBlocks} blocks have NetworkIsStatic=true</b>");

		response = StringBuilderPool.Shared.ToStringReturn(sb);
		return true;
	}

}
