using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;

namespace tesinormed.FAndCDaveCo.Extensions;

public static class InteractiveTerminalManagerExtensions
{
	public static void RegisterApplication<T>(params string[] commands) where T : TerminalApplication, new()
	{
		foreach (var command in commands) InteractiveTerminalManager.RegisterApplication<T>(command);
	}
}
