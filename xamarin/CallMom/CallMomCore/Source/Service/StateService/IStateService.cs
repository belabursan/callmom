using System;

namespace CallMomCore
{
	public interface IStateService
	{
		void Reset ();

		int GetState ();

		void SetState (int state);
	}
}

