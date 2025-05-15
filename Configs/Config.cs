using System.ComponentModel;

namespace ProjectMER.Configs;

public class Config
{
	[Description("Whether the object will be auto selected when spawning it.")]
	public bool AutoSelect { get; set; } = true;

	public List<string> OnWaitingForPlayers { get; set; } = new();
	public List<string> OnRoundStarted { get; set; } = new();
	public List<string> OnLczDecontaminationStarted { get; set; } = new();
	public List<string> OnWarheadStarted { get; set; } = new();
	public List<string> OnWarheadStopped { get; set; } = new();
	public List<string> OnWarheadDetonated { get; set; } = new();
}
