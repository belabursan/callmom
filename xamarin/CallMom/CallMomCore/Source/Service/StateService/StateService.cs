using System;

namespace CallMomCore
{
	public class StateService : IStateService
	{
		private int _State;


		public StateService ()
		{
			_State = State.Initialized;
		}

		#region IStateService implementation

		public void Reset ()
		{
			_State = State.Initialized;
		}

		public int GetState ()
		{
			return _State;
		}


		public void SetState (int state)
		{
			_State = state;
		}

		#endregion
	}
}

