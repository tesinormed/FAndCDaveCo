using System;
using InteractiveTerminalAPI.UI.Application;

namespace tesinormed.FAndCDaveCo.Extensions;

public abstract class TerminalApplicationExtension : TerminalApplication
{
	protected override string GetApplicationText() => currentScreen.GetText(availableLength: 51);

	protected override Action PreviousScreen() => () => { };
}
