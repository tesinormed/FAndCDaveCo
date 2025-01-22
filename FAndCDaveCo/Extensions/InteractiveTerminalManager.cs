namespace tesinormed.FAndCDaveCo.Extensions;

public static class InteractiveTerminalManager
{
	public static void RegisterApplication<T>(params string[] commands) where T : InteractiveTerminalAPI.UI.Application.TerminalApplication, new()
	{
		foreach (var command in commands) InteractiveTerminalAPI.UI.InteractiveTerminalManager.RegisterApplication<T>(command);
	}
}
