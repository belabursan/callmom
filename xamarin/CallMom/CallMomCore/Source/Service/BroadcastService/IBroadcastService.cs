using System;

namespace CallMomCore
{
	public interface IBroadcastService
	{
		event EventHandler<CallMomEventArgs> CoreEvents;

		void RaiseNewEvent (object sender, CallMomEventArgs data, int delay = 0);
	}
}

