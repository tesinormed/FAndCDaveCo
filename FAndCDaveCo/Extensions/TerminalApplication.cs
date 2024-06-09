using System;

namespace tesinormed.FAndCDaveCo.Extensions;

public abstract class TerminalApplication : InteractiveTerminalAPI.UI.Application.TerminalApplication
{
	protected override string GetApplicationText() => currentScreen.GetText(availableLength: 51);

	protected override Action PreviousScreen() => () => { };
}
