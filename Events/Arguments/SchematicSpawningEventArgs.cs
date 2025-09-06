using LabApi.Events.Arguments.Interfaces;
using ProjectMER.Features.Serializable.Schematics;

namespace ProjectMER.Events.Arguments;

public class SchematicSpawningEventArgs : EventArgs, ICancellableEvent
{
	public SchematicSpawningEventArgs(SchematicObjectDataList data, string name)
	{
		Data = data;
		Name = name;
		IsAllowed = true;
	}

	public SchematicObjectDataList Data { get; set; }

	public string Name { get; }

	public bool IsAllowed { get; set; }
}
