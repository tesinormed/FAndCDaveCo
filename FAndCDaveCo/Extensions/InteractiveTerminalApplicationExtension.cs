using System;
using InteractiveTerminalAPI.UI;
using InteractiveTerminalAPI.UI.Application;
using InteractiveTerminalAPI.UI.Cursor;
using InteractiveTerminalAPI.UI.Screen;

namespace tesinormed.FAndCDaveCo.Extensions;

public abstract class InteractiveTerminalApplicationExtension : InteractiveTerminalApplication
{
	protected abstract string Title { get; }

	protected IScreen MainScreen { get; set; } = null!;
	protected CursorMenu MainCursorMenu { get; set; } = null!;
	private new IScreen PreviousScreen { get; set; } = null!;
	private CursorMenu PreviousCursorMenu { get; set; } = null!;

	protected Action PreviousMainScreenAction => () => SwitchScreen(MainScreen, MainCursorMenu, previous: true);
	protected Action PreviousScreenAction => () => SwitchScreen(PreviousScreen, PreviousCursorMenu, previous: true);

	protected override void SwitchScreen(IScreen screen, CursorMenu cursorMenu, bool previous)
	{
		PreviousScreen = currentScreen;
		PreviousCursorMenu = currentCursorMenu;
		base.SwitchScreen(screen, cursorMenu, previous);
	}

	protected (IScreen, CursorMenu) Selection(string prompt, params CursorElement[] options)
	{
		var cursorMenu = CursorMenu.Create(elements: options);

		ITextElement[] textElements =
		[
			TextElement.Create(prompt),
			TextElement.Create(" "),
			cursorMenu
		];
		var screen = BoxedScreen.Create(Title, textElements);

		SwitchScreen(screen, cursorMenu, previous: false);
		return (screen, cursorMenu);
	}

	protected void SelectionWithBack(string prompt, Action backAction, params CursorElement[] options)
	{
		options =
		[
			..options,
			CursorElement.Create(
				"Back",
				action: backAction,
				active: _ => true,
				selectInactive: false
			)
		];
		var cursorMenu = CursorMenu.Create(elements: options);

		ITextElement[] textElements =
		[
			TextElement.Create(prompt),
			TextElement.Create(" "),
			cursorMenu
		];
		var screen = BoxedScreen.Create(Title, textElements);

		SwitchScreen(screen, cursorMenu, previous: false);
	}

	protected void Confirm(Action confirmAction, params ITextElement[] text)
	{
		var cursorMenu = CursorMenu.Create(elements:
		[
			CursorElement.Create("Confirm", action: confirmAction)
		]);

		ITextElement[] textElements = [..text, TextElement.Create(" "), cursorMenu];
		var screen = BoxedScreen.Create(Title, textElements);

		SwitchScreen(screen, cursorMenu, previous: false);
	}

	protected void Confirm(Action confirmAction, Action declineAction, params ITextElement[] text)
	{
		var cursorMenu = CursorMenu.Create(elements:
		[
			CursorElement.Create("Confirm", action: confirmAction),
			CursorElement.Create("Decline", action: declineAction)
		]);

		ITextElement[] textElements = [..text, TextElement.Create(" "), cursorMenu];
		var screen = BoxedScreen.Create(Title, textElements);

		SwitchScreen(screen, cursorMenu, previous: false);
	}

	protected void Notification(Action backAction, params ITextElement[] text)
	{
		var cursorMenu = CursorMenu.Create(elements:
		[
			CursorElement.Create("Back", action: backAction)
		]);

		ITextElement[] textElements = [..text, TextElement.Create(" "), cursorMenu];
		var screen = BoxedScreen.Create(Title, textElements);

		SwitchScreen(screen, cursorMenu, previous: false);
	}

	protected void LockedNotification(params ITextElement[] text)
	{
		var screen = BoxedScreen.Create(Title, text);
		var cursorMenu = CursorMenu.Create();

		SwitchScreen(screen, cursorMenu, previous: false);
	}
}
