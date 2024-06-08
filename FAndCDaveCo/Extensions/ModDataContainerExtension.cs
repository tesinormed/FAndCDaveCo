using LethalModDataLib.Base;

namespace tesinormed.FAndCDaveCo.Extensions;

public abstract class ModDataContainerExtension : ModDataContainer
{
	protected virtual void PreReset()
	{
	}

	protected virtual void ResetAll()
	{
	}

	public void Reset()
	{
		PreReset();
		ResetAll();
		PostReset();
	}

	protected virtual void PostReset()
	{
	}
}
