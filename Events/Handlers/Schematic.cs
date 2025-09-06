using LabApi.Events;
using ProjectMER.Events.Arguments;

namespace ProjectMER.Events.Handlers;

public static class Schematic
{
	public static event LabEventHandler<SchematicSpawningEventArgs> SchematicSpawning;

	public static event LabEventHandler<SchematicSpawnedEventArgs> SchematicSpawned;

	public static event LabEventHandler<ButtonInteractedEventArgs> ButtonInteracted;

	public static event LabEventHandler<SchematicDestroyedEventArgs> SchematicDestroyed;

	internal static void OnSchematicSpawning(SchematicSpawningEventArgs ev) => SchematicSpawning.InvokeEvent(ev);

	internal static void OnSchematicSpawned(SchematicSpawnedEventArgs ev) => SchematicSpawned.InvokeEvent(ev);

	internal static void OnButtonInteracted(ButtonInteractedEventArgs ev) => ButtonInteracted.InvokeEvent(ev);

	internal static void OnSchematicDestroyed(SchematicDestroyedEventArgs ev) => SchematicDestroyed.InvokeEvent(ev);
}
