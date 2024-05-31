using LethalNetworkAPI;
using System;

namespace tesinormed.FAndCDaveCo.Misc
{
	public static class LethalNetworkCreator
	{
		public static LethalNetworkVariable<T> CreateVariable<T>(string identifier, Action<T> onValueChanged)
		{
			LethalNetworkVariable<T> variable = new(identifier);
			variable.OnValueChanged += onValueChanged;
			return variable;
		}
	}
}
